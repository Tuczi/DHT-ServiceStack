using System;
using System.Collections.Generic;
using System.Numerics;
using Server.Logic.DHT;
using Server.Services.ValueService;

namespace Server.Services.ServerService
{
	public class JoinDtoResponse
	{
		public JoinDtoResponse ()
		{
		}

		public JoinDtoResponse (DHT dht, List<ValueDtoResponse> data)
		{
			RangeMin = dht.HashRange.Min.ToString();
			RangeMax = dht.HashRange.Max.ToString();
			Child = dht.Child;
			Data = data;
		}

		public string RangeMin { get; set; }

		public string RangeMax { get; set; }

		public string Child { get; set; }

		public List<ValueDtoResponse> Data { get; set; }
	}
}

