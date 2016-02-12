using System;

namespace Server.Logic.DHT
{
	public interface IHashFunction
	{
		string max ();
		string min ();
		string apply (string data);
	}
}

