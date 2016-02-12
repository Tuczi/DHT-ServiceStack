using System;
using Server.Logic.DHT;

namespace Server
{
	public static class DHTServerCtx
	{
		public static Sha1HashFunction HashFunction = new Sha1HashFunction ();
		public static DHT DHT = new DHT ();
	}
}

