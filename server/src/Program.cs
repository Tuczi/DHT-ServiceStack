using System;
using RestSharp;


namespace Server
{
	internal class Program
	{
		private static void Main ()
		{
			var parentServerUrl = Environment.GetEnvironmentVariable ("PARENT_URL");
			var appHost = new AppHost ();
			appHost.Init ();
			ushort port = 8888;
			string listeningOn = string.Format ("http://*:{0}/", port);
			appHost.Start (listeningOn);

			Console.WriteLine ("AppHost Created at {0}, listening on {1}, parent server URL is {2}", DateTime.Now, listeningOn, parentServerUrl);
			if(parentServerUrl != null)
				joinDHT (parentServerUrl);

			Console.WriteLine ("Press ENTER to exit...");
			Console.ReadLine ();
			if(parentServerUrl != null)
				leaveDHT (parentServerUrl);
		}

		//TODO find way to get context from app host - container field?
		private static void joinDHT (string parentServerUrl)
		{
			var client = new RestClient (parentServerUrl);

			var request = new RestRequest ("dht/join", Method.POST);
			//request.AddBody ({"Child": "TODO"}); //TODO my addr in Child Field

			RestResponse response = (RestResponse)client.Execute (request);
			var content = response.Content;
			//TODO add to DB
		}

		private static void leaveDHT (string parentServerUrl)
		{
			var client = new RestClient (parentServerUrl);

			var request = new RestRequest ("dht/leave", Method.POST);
			// request.AddBody //TODO my "Child", my "HashRange", my "Data" List<ValueDto>

			RestResponse response = (RestResponse)client.Execute (request);
			//TODO check response status
		}
	}
}