using System;
using System.Collections.Generic;
using System.Numerics;
using Server.Logic.DHT;
using Server.Services.ValueService;
using ServiceStack.ServiceHost;

namespace Server.Services.ServerService
{
	[Route ("/dht/leave", "POST")]
	public class LeaveDto: IReturn<LeaveDtoResponse>
	{
		public string RangeMin { get; set; }

		public string RangeMax { get; set; }

		public string Child { get; set; }

		public List<ValueDto> Data { get; set; }
	}
}

