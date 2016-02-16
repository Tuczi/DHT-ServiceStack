using System;
using ServiceStack.ServiceHost;

namespace Server.Services.ServerService
{
	[Route ("/dht/leave/as-parent", "POST")]
	public class LeaveParentDto: IReturn<LeaveParentDtoResponse>
	{
		public string Parent { get; set; }
	}
}

