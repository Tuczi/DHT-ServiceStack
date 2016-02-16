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
using ServiceStack.ServiceHost;

namespace Server
{
	internal class Program
	{
		private static void Main ()
		{
			DHTServerCtx.DHT.MyPublicUrl = Environment.GetEnvironmentVariable ("PUBLIC_URL");//http://localhost:8888/

			if (string.IsNullOrWhiteSpace (DHTServerCtx.DHT.MyPublicUrl) || !Uri.IsWellFormedUriString (DHTServerCtx.DHT.MyPublicUrl, UriKind.RelativeOrAbsolute))
				throw new MissingFieldException ("PUBLIC_URL have to be set");
			
			var parentServerUrl = Environment.GetEnvironmentVariable ("PARENT_URL");//http://localhost:8889/
			if (!string.IsNullOrWhiteSpace (parentServerUrl) && !Uri.IsWellFormedUriString (parentServerUrl, UriKind.RelativeOrAbsolute))
				throw new FormatException ("PARENT_URL have to be URL");

			var appHost = new AppHost ();
			appHost.Init ();
			Console.WriteLine (DHTServerCtx.DHT.MyPublicUrl);
			appHost.Start (DHTServerCtx.DHT.MyPublicUrl);

			Console.WriteLine ("AppHost Created at {0}, listening on {1}, parent server URL is {2}", DateTime.Now, DHTServerCtx.DHT.MyPublicUrl, parentServerUrl);
			joinDHT (parentServerUrl, DHTServerCtx.DHT.MyPublicUrl, appHost.Container);

			Console.Write (DHTServerCtx.DHT);
			Console.WriteLine ("Press ENTER to exit...");
			Console.ReadLine ();

			Console.Write (DHTServerCtx.DHT);
			leaveDHT (appHost.Container);
		}

		private static void joinDHT (string parentServerUrl, string myPublicUrl, Container container)
		{
			if (string.IsNullOrEmpty (parentServerUrl)) {
				Console.WriteLine ("Joined with code 200 - No parent");
				return;
			}
			
			var client = new RestClient (parentServerUrl);
			var requestPath = (RouteAttribute)Attribute.GetCustomAttribute (typeof(JoinChildDto), typeof(RouteAttribute));
			var request = new RestRequest (requestPath.Path, Method.POST);
			request.RequestFormat = DataFormat.Json;
			var data = new JoinChildDto{ Child = myPublicUrl };
			request.AddJsonBody (data);

			Console.WriteLine ("Sending join as child request to {0} {1}", client.BaseUrl, requestPath.Path);
			var response = client.Execute<JoinChildDtoResponse> (request);

			var des = new JsonDeserializer ();
			Console.WriteLine (response.Content);
			response.Data = des.Deserialize<JoinChildDtoResponse> (response);

			DHTServerCtx.DHT.Parent = parentServerUrl;
			DHTServerCtx.DHT.Child = response.Data.Child;
			DHTServerCtx.DHT.HashRange.Min = BigInteger.Parse (response.Data.RangeMin);
			DHTServerCtx.DHT.HashRange.Max = BigInteger.Parse (response.Data.RangeMax);

			if (string.IsNullOrEmpty (DHTServerCtx.DHT.Child)) {
				//Parent were stand alone server
				DHTServerCtx.DHT.Child = parentServerUrl;
			} else {
				//notify self new child
				joinMyNewChild (myPublicUrl);
			}

			insertIntoDb (response.Data.Data, container);
			Console.WriteLine ("Joined with code {0}", response.StatusCode);
		}

		private static void joinMyNewChild (string myPublicUrl)
		{
			var client = new RestClient (DHTServerCtx.DHT.Child);
			var requestPath = (RouteAttribute)Attribute.GetCustomAttribute (typeof(JoinParentDto), typeof(RouteAttribute));
			var request = new RestRequest (requestPath.Path, Method.POST);
			var data = new JoinParentDto{ Parent = myPublicUrl };
			request.AddJsonBody (data);

			Console.WriteLine ("Sending join as parent request to {0} {1}", client.BaseUrl, requestPath.Path);
			var response = client.Execute<JoinParentDtoResponse> (request);
			Console.WriteLine ("Joined as parent with code {0}", response.StatusCode);
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
			var requestPath = (RouteAttribute)Attribute.GetCustomAttribute (typeof(LeaveChildDto), typeof(RouteAttribute));
			var request = new RestRequest (requestPath.Path, Method.POST);
			request.RequestFormat = DataFormat.Json;
			var data = new LeaveChildDto {Child = DHTServerCtx.DHT.Child,
				RangeMin = DHTServerCtx.DHT.HashRange.Min.ToString (),
				RangeMax = DHTServerCtx.DHT.HashRange.Max.ToString (),
				Data = removeFromDb (container)
			};
			request.AddJsonBody (data);

			Console.WriteLine ("Sending leave as child request to {0} {1}", client.BaseUrl, requestPath.Path);
			RestResponse response = (RestResponse)client.Execute (request);

			if (DHTServerCtx.DHT.Parent != DHTServerCtx.DHT.Child) {
				//notify child
				leaveMyChild ();
			}

			Console.WriteLine ("Left as child with code {0}", response.StatusCode);
		}

		private static void leaveMyChild ()
		{
			var client = new RestClient (DHTServerCtx.DHT.Child);
			var requestPath = (RouteAttribute)Attribute.GetCustomAttribute (typeof(LeaveParentDto), typeof(RouteAttribute));
			var request = new RestRequest (requestPath.Path, Method.POST);
			var data = new LeaveParentDto{ Parent = DHTServerCtx.DHT.Parent };
			request.AddJsonBody (data);

			Console.WriteLine ("Sending leave as parent request to {0} {1}", client.BaseUrl, requestPath.Path);
			var response = client.Execute<LeaveParentDtoResponse> (request);
			Console.WriteLine ("Left as parent with code {0}", response.StatusCode);
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
