using System;
using ServiceStack.ServiceHost;
using System.Collections.Generic;
using Server.Services.ValueService;
using Server.Logic.DHT;

namespace Server.Services.ServerService
{
	[Route ("/dht/leave", "POST")]
	public class LeaveDto: IReturn<LeaveDtoResponse>
	{
		public HashRange HashRange;
		public string Child;
		public List<ValueDto> Data;
	}
}

