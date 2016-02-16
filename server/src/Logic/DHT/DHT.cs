using System;
using System.Numerics;
using System.Collections.Generic;

namespace Server.Logic.DHT
{
	//TODO make interface and use dependency injection
	public class DHT
	{
		public string MyPublicUrl;
		public string Parent = "";
		public string Child = "";

		public HashRange HashRange = new HashRange {
			Min = DHTServerCtx.HashFunction.min (),
			Max = DHTServerCtx.HashFunction.max ()
		};

		public DHT splitRange (string newChild)
		{
			var newMax = HashRange.Min + ((HashRange.Max - HashRange.Min) / 2);

			var result = new DHT {
				Child = Child,
				HashRange = { Min = newMax, Max = HashRange.Max }
			};

			Child = newChild;
			HashRange.Max = newMax;

			//if parent is empty then set child as child and parent (to make cycle)
			if (string.IsNullOrEmpty (Parent)) {
				Parent = newChild;
			}

			return result;
		}

		public void mergeRange (DHT dht)
		{
			var hashList = new List<BigInteger>{ HashRange.Max, HashRange.Min, dht.HashRange.Max, dht.HashRange.Min };
			hashList.Sort ();

			HashRange.Min = hashList[0];
			HashRange.Max = hashList[3];

			Child = dht.Child;

			//if this is last server
			if (Child == MyPublicUrl) {
				Parent = Child = "";
			}
		}

		public override string ToString ()
		{
			return string.Format ("DHT: {{ Parent: {0}, Child: {1}, Hash-min: {2}, Hash-max {3}}}", Parent, Child, HashRange.Min, HashRange.Max);
		}
	}
}

