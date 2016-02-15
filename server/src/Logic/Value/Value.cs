using System;
using System.Globalization;
using System.Numerics;
using Server.Services.ValueService;
using ServiceStack.DataAnnotations;

namespace Server.Logic.Value
{
	public class Value
	{
		[Ignore]
		public BigInteger Hash { get; set; }

		[PrimaryKey]
		public string HashHexString {
			get {
				var str = Hash.ToString ("X");
				int missingZeros = DHTServerCtx.HashFunction.MaxSizeInBytes () * 2 - str.Length;
				if (missingZeros > 0)
					return new string ('0', missingZeros) + str;
				else
					return str;
			}
			set {
				Hash = BigInteger.Parse ("0" + value, NumberStyles.AllowHexSpecifier);
			}
		}

		public string Key { get; set; }

		public string Data { get; set; }
	}
}

