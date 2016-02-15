using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using Funq;
using RestSharp;
using RestSharp.Deserializers;
using Server.Logic.Value;
using Server.Services.ServerService;
using Server.Services.ValueService;
using ServiceStack.OrmLite;

namespace Server
{
	internal class Program
	{
		private static void Main ()
		{
			var myPublicUrl = Environment.GetEnvironmentVariable ("PUBLIC_URL");//http://localhost:8888/
			if (string.IsNullOrWhiteSpace (myPublicUrl) || !Uri.IsWellFormedUriString (myPublicUrl, UriKind.RelativeOrAbsolute))
				throw new MissingFieldException ("PUBLIC_URL have to be set");
			
			var parentServerUrl = Environment.GetEnvironmentVariable ("PARENT_URL");//http://localhost:8889/
			if (!string.IsNullOrWhiteSpace (parentServerUrl) && !Uri.IsWellFormedUriString (parentServerUrl, UriKind.RelativeOrAbsolute))
				throw new FormatException ("PARENT_URL have to be URL");

			var appHost = new AppHost ();
			appHost.Init ();
			Console.WriteLine (myPublicUrl);
			appHost.Start (myPublicUrl);

			Console.WriteLine ("AppHost Created at {0}, listening on {1}, parent server URL is {2}", DateTime.Now, myPublicUrl, parentServerUrl);
			joinDHT (parentServerUrl, myPublicUrl, appHost.Container);

			Console.WriteLine ("Hash From {0} to {1}. Child {2} parent {3}", DHTServerCtx.DHT.HashRange.Min, DHTServerCtx.DHT.HashRange.Max, DHTServerCtx.DHT.Child, DHTServerCtx.DHT.Parent);
			Console.WriteLine ("Press ENTER to exit...");
			Console.ReadLine ();
			leaveDHT (appHost.Container);
		}

		//TODO find way to get context from app host - container field?
		private static void joinDHT (string parentServerUrl, string myPublicUrl, Container container)
		{
			if (string.IsNullOrEmpty (parentServerUrl)) {
				Console.WriteLine ("Joined with code 200 - No parent");
				return;
			}
			
			var client = new RestClient (parentServerUrl);
			var request = new RestRequest ("dht/join", Method.POST);
			request.RequestFormat = DataFormat.Json;
			var data = new JoinDto{ Child = myPublicUrl };
			request.AddJsonBody (data);

			Console.WriteLine ("Sending dht/join request to {0}", parentServerUrl);
			var response = client.Execute<JoinDtoResponse> (request);

			var des = new JsonDeserializer ();
			Console.WriteLine (response.Content);
			response.Data = des.Deserialize<JoinDtoResponse> (response);

			DHTServerCtx.DHT.Parent = parentServerUrl;
			DHTServerCtx.DHT.Child = response.Data.Child;
			DHTServerCtx.DHT.HashRange.Min = BigInteger.Parse (response.Data.RangeMin);
			DHTServerCtx.DHT.HashRange.Max = BigInteger.Parse (response.Data.RangeMax);

			insertIntoDb (response.Data.Data, container);
			Console.WriteLine ("Joined with code {0}", response.StatusCode);
		}

		private static void insertIntoDb (List<ValueDtoResponse> data, Container container)
		{
			var dbFactory = container.Resolve<IDbConnectionFactory> ();
			var valueList = data.Select (v => new Value {
				Key = v.Key,
				Data = v.Data,
				Hash = DHTServerCtx.HashFunction.apply (v.Key)
			}).ToList ();
			using (IDbConnection db = dbFactory.Open ()) {
				db.InsertAll<Value> (valueList);
			}
		}

		private static void leaveDHT (Container container)
		{
			if (string.IsNullOrEmpty (DHTServerCtx.DHT.Parent)) {
				Console.WriteLine ("Left with code 200 - No parent");
				return;
			}

			var client = new RestClient (DHTServerCtx.DHT.Parent);
			var request = new RestRequest ("dht/leave", Method.POST);
			request.RequestFormat = DataFormat.Json;
			var data = new LeaveDto {Child = DHTServerCtx.DHT.Child,
				RangeMin = DHTServerCtx.DHT.HashRange.Min.ToString (),
				RangeMax = DHTServerCtx.DHT.HashRange.Max.ToString (),
				Data = removeFromDb (container)
			};
			request.AddJsonBody (data);

			Console.WriteLine ("Sending dht/leave request to {0}", DHTServerCtx.DHT.Parent);
			RestResponse response = (RestResponse)client.Execute (request);

			Console.WriteLine ("Left with code {0}", response.StatusCode);
		}

		private static List<ValueDto> removeFromDb (Container container)
		{
			var dbFactory = container.Resolve<IDbConnectionFactory> ();

			var valueList = new List<Value> (0);
			using (IDbConnection db = dbFactory.Open ()) {
				valueList = db.Select<Value> ();
				db.DeleteAll<Value> ();
			}

			return valueList.Select (v => new ValueDto {
				Key = v.Key,
				Data = v.Data
			}).ToList ();
		}
	}
}
