using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Numerics;

namespace Server.Logic.DHT
{
	//TODO make interface and use dependency injection
	public class Sha1HashFunction
	{
		private SHA1 sha = new SHA1CryptoServiceProvider ();

		public BigInteger max ()
		{
			return new BigInteger (Enumerable.Repeat (0, sha.HashSize * 2 / 8).Select (e => (byte)0xFF).ToArray ());
		}

		public BigInteger min ()
		{
			return new BigInteger (Enumerable.Repeat (0, sha.HashSize * 2 / 8).Select (e => (byte)0x00).ToArray ());
		}

		public BigInteger apply (string data)
		{
			var unicodeEncoding = new UnicodeEncoding ();
			var unicodeDataBytes = unicodeEncoding.GetBytes (data);
			var hash = sha.ComputeHash (unicodeDataBytes);

			return new BigInteger (hash);
		}
	}
}

