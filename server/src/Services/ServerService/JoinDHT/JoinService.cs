using System;
using System.Linq;
using System.Net;
using Server.Logic.Value;
using Server.Services.ValueService;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Collections.Generic;

namespace Server.Services.ServerService
{
	public class JoinService: Service
	{
		public static ILog log = LogManager.GetLogger (typeof(JoinService));

		//new Server want to join DHT
		public object Post (JoinDto req)
		{
			var childDht = DHTServerCtx.DHT.splitRange (req.Child);
			var found = Db.Select<Value> (q => q.Hash <= childDht.HashRange.Min);//TODO test
			var dataList = found.Select (v => new ValueDtoResponse (new ValueDto ().PopulateWith (v))).ToList();

			Db.Delete<Value> (q => q.Hash <= childDht.HashRange.Min);//TODO transaction

			log.Info(string.Format("{0} joined. New max range={1}", DHTServerCtx.DHT.Child, DHTServerCtx.DHT.HashRange.Max));
			return new JoinDtoResponse (childDht, dataList);
		}

	}
}

