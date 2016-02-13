using System;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Deserializers;
using Server.Services.ServerService;
using Server.Services.ValueService;
using System.Numerics;


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
			joinDHT (parentServerUrl, myPublicUrl);

			Console.WriteLine ("Hash From {0} to {1}. Child {2} parent {3}", DHTServerCtx.DHT.HashRange.Min, DHTServerCtx.DHT.HashRange.Max, DHTServerCtx.DHT.Child, DHTServerCtx.DHT.Parent);
			Console.WriteLine ("Press ENTER to exit...");
			Console.ReadLine ();
			leaveDHT ();
		}

		//TODO find way to get context from app host - container field?
		private static void joinDHT (string parentServerUrl, string myPublicUrl)
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
			DHTServerCtx.DHT.HashRange.Min = BigInteger.Parse(response.Data.RangeMin);
			DHTServerCtx.DHT.HashRange.Max = BigInteger.Parse(response.Data.RangeMax);
			//TODO add responseData.Data to DB
			Console.WriteLine ("Joined with code {0}", response.StatusCode);
		}

		private static void leaveDHT ()
		{
			if (string.IsNullOrEmpty (DHTServerCtx.DHT.Parent)) {
				Console.WriteLine ("Left with code 200 - No parent");
				return;
			}

			var client = new RestClient (DHTServerCtx.DHT.Parent);
			var request = new RestRequest ("dht/leave", Method.POST);
			request.RequestFormat = DataFormat.Json;
			var data = new LeaveDto {Child = DHTServerCtx.DHT.Child,
				RangeMin = DHTServerCtx.DHT.HashRange.Min.ToString(),
				RangeMax = DHTServerCtx.DHT.HashRange.Max.ToString(),
				Data = new List<ValueDto> ()
			};//TODO read Data from DB
			request.AddJsonBody (data);

			Console.WriteLine ("Sending dht/leave request to {0}", DHTServerCtx.DHT.Parent);
			RestResponse response = (RestResponse)client.Execute (request);

			Console.WriteLine ("Left with code {0}", response.StatusCode);
		}
	}
}
