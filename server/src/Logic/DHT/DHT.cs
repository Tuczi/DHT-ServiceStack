using System;
using System.Numerics;

namespace Server.Logic.DHT
{
	//TODO make interface and use dependency injection
	public class DHT
	{
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
			HashRange.Max = dht.HashRange.Max;

			Child = dht.Child;

			//if this is last server
			if (Parent == Child) {
				Parent = Child = "";
			}
		}
	}
}

