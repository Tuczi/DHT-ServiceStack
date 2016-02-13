using System;
using RestSharp;
using System.Collections.Generic;


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
			var data = new Dictionary<string, Object> ();

			data.Add ("Child", myPublicUrl);

			request.AddJsonBody (data);
			Console.WriteLine ("Sending dht/join request to {0}", parentServerUrl);
			RestResponse response = (RestResponse)client.Execute (request);

			var content = response.Content;
			//TODO add to DB
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
			var data = new Dictionary<string, Object> ();

			Console.WriteLine ("Sending dht/leave request to {0}", DHTServerCtx.DHT.Parent);
			data.Add ("Child", DHTServerCtx.DHT.Child);
			data.Add ("HashRange", DHTServerCtx.DHT.HashRange);
			data.Add ("Data", "TODO");//TODO List<ValueDto>

			request.AddJsonBody (data);
			RestResponse response = (RestResponse)client.Execute (request);

			Console.WriteLine ("Left with code {0}", response.StatusCode);
		}
	}
}
