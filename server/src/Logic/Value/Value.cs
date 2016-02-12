using System;
using ServiceStack.DataAnnotations;
using Server.Services.ValueService;

namespace Server.Logic.Value
{
	public class Value
	{
		[PrimaryKey]
		public string Hash { get; set; }

		public string Key { get; set; }

		public string Data { get; set; }
	}
}

