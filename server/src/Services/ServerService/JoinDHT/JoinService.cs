using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Server.Logic.Value;
using Server.Services.ValueService;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;

namespace Server.Services.ServerService
{
	public class JoinService: Service
	{
		public static ILog log = LogManager.GetLogger (typeof(JoinService));

		//new Server want to join DHT
		public object Post (JoinDto req)
		{
			var childDht = DHTServerCtx.DHT.splitRange (req.Child);
			var hashHexString = new Value{ Hash = childDht.HashRange.Min }.HashHexString;
			Console.WriteLine (hashHexString);
			var found = Db.Select<Value> (string.Format("HashHexString >= '{0}'", hashHexString));

			var dataList = found.Select (v => new ValueDtoResponse (new ValueDto ().PopulateWith (v))).ToList ();

			Db.Delete<Value> (string.Format("HashHexString >= '{0}'", hashHexString));//TODO transaction

			log.Info (string.Format ("{0} joined. New max range={1}", DHTServerCtx.DHT.Child, DHTServerCtx.DHT.HashRange.Max));
			return new JoinDtoResponse (childDht, dataList);
		}

	}
}

