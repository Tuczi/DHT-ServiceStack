using System;
using System.Collections.Generic;
using Server.Services.ValueService;
using Server.Logic.DHT;

namespace Server.Services.ServerService
{
	public class JoinDtoResponse
	{
		public JoinDtoResponse (DHT dht, List<ValueDtoResponse> data)
		{
			HashRange = dht.HashRange;
			Child = dht.Child;
			Data = data;
		}

		public HashRange HashRange;
		public string Child;
		public List<ValueDtoResponse> Data;
	}
}

