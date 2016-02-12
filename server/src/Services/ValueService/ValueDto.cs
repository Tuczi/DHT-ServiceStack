using System;
using ServiceStack.ServiceHost;
using Server.Logic.Value;

namespace Server.Services.ValueService
{
	[Route ("/value/{key}", "GET PUT DELETE")]
	public class ValueDto : IReturn<ValueDtoResponse>
	{
		public string Key { get; set; }
		public string Data { get; set; }

	}
}

