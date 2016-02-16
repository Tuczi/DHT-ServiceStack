using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Server.Logic.DHT;
using Server.Logic.Value;
using Server.Services.ValueService;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System.Numerics;

namespace Server.Services.ServerService
{
	public class LeaveChildService: Service
	{
		public static ILog log = LogManager.GetLogger (typeof(LeaveChildService));

		//server want to leave DHT
		public object Post (LeaveChildDto req)
		{
			DHTServerCtx.DHT.mergeRange (new DHT {
				Child = req.Child,
				HashRange = new HashRange {
					Min = BigInteger.Parse (req.RangeMin),
					Max = BigInteger.Parse (req.RangeMax)
				}
			});

			var valueList = req.Data.Select (v => new Value {
				Key = v.Key,
				Data = v.Data,
				Hash = DHTServerCtx.HashFunction.apply (v.Key)
			}).ToList ();
			Db.InsertAll<Value> (valueList);

			log.Info (string.Format ("Old child left (new child {0}), new max range={1}", DHTServerCtx.DHT.Child, DHTServerCtx.DHT.HashRange.Max));
			log.Debug (DHTServerCtx.DHT);
			return new HttpResult { StatusCode = HttpStatusCode.NoContent };
		}
	}
}

