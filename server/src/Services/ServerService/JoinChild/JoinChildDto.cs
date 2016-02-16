using System;
using ServiceStack.ServiceHost;

namespace Server.Services.ServerService
{
	[Route ("/dht/join/as-child", "POST")]
	public class JoinChildDto: IReturn<JoinChildDtoResponse>
	{
		public string Child { get; set; }
	}
}

