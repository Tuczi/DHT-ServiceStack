using System;
using ServiceStack.DataAnnotations;
using Server.Services.ValueService;
using System.Numerics;

namespace Server.Logic.Value
{
	public class Value
	{
		[PrimaryKey]
		public BigInteger Hash { get; set; }

		public string Key { get; set; }

		public string Data { get; set; }
	}
}

