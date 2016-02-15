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
			var list = Enumerable.Repeat (0, sha.HashSize / 8).Select (e => (byte)0xFF).ToList ();
			list.Add ((byte)0x00);//BigInteger is singled value so append zero is necessary (little-endian)
			return new BigInteger (list.ToArray ());
		}

		public BigInteger min ()
		{
			return new BigInteger (0);
		}

		public BigInteger apply (string data)
		{
			var unicodeEncoding = new UnicodeEncoding ();
			var unicodeDataBytes = unicodeEncoding.GetBytes (data);
			var hash = sha.ComputeHash (unicodeDataBytes);
			byte[] zero = { 0x00 };//BigInteger is singled value so append zero is necessary (little-endian)

			return new BigInteger (hash.Concat (zero).ToArray ());
		}
	}
}

