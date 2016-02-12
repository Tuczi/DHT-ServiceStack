using System;

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

		public DHT splitRange (string child)
		{
			var newMax = HashRange.Max;
			//TODO calculate

			var result = new DHT {
				Child = Child,
				HashRange = { Min = newMax, Max = newMax }
			};

			Child = child;
			HashRange.Max = newMax;
			//if parent is empty then set child as child and parent (to make cycle)
			if (string.IsNullOrEmpty (Parent)) {
				Parent = child;
			}

			return result;
		}

		public void mergeRange (DHT dht)
		{
			//TODO
		}
	}
}

