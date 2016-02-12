using System;
using System.Net;
using System.Linq;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using Server.Logic.Value;
using ServiceStack.OrmLite;
using System.Collections.Generic;
using ServiceStack.Common;
using Server.Services.ValueService;
using Server.Logic.DHT;

namespace Server.Services.ServerService
{
	public class LeaveService: Service
	{
		public static ILog log = LogManager.GetLogger (typeof(LeaveService));

		//server want to leave DHT
		public object Post (LeaveDto req)
		{
			DHTServerCtx.DHT.mergeRange (new DHT{ Child = req.Child, HashRange = req.HashRange });
			var valueList = req.Data.Select (v => new Value { Key = v.Key, Data = v.Data, Hash = DHTServerCtx.HashFunction.apply (v.Key) }).ToList ();
			Db.InsertAll<Value> (valueList);

			return new HttpResult { StatusCode = HttpStatusCode.NoContent };
		}
	}
}

