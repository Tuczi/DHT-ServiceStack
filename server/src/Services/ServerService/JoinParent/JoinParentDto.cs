using System;
using ServiceStack.ServiceHost;

namespace Server.Services.ServerService
{
	[Route ("/dht/join/as-parent", "POST")]
	public class JoinParentDto: IReturn<JoinParentDtoResponse>
	{
		public string Parent { get; set; }
	}
}

