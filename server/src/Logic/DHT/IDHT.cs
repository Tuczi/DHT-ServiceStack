using System;

namespace Server.Logic.DHT
{
	public interface IDHT
	{
		IDHT splitRange (string child);
		void mergeRange (IDHT dht);
		string getParent();
		string getChild();
		HashRange getHashRange();
	}
}

