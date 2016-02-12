using System;
using ServiceStack.ServiceHost;

namespace Server.Services.ServerService
{
	[Route ("/dht/join", "POST")]
	public class JoinDto: IReturn<JoinDtoResponse>
	{
		public string Child { get; set; }
	}
}

