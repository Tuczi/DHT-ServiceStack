using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Server.Logic.DHT
{
	//TODO make interface and use dependency injection
	public class Sha1HashFunction
	{
		private SHA1 sha = new SHA1CryptoServiceProvider ();

		public string max ()
		{
			return new string ('F', sha.HashSize * 2 / 8);
		}

		public string min ()
		{
			return new string ('0', sha.HashSize * 2 / 8);
		}

		public string apply (string data)
		{
			var unicodeEncoding = new UnicodeEncoding ();
			var unicodeDataBytes = unicodeEncoding.GetBytes (data);
			var hash = sha.ComputeHash (unicodeDataBytes);

			var sb = new StringBuilder (hash.Length * 2);
			foreach (var b in hash) {
				sb.Append (Convert.ToString (b, 16));
			}

			return sb.ToString ();
		}
	}
}

