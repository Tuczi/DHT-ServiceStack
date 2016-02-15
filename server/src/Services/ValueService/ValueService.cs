using System;
using System.Linq;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using Server.Logic.Value;
using ServiceStack.OrmLite;
using ServiceStack.Common;
using System.Text;
using Server.Logic.DHT;

namespace Server.Services.ValueService
{
	public class ValueService : Service
	{
		public static ILog log = LogManager.GetLogger (typeof(ValueService));

		public ValueService ()
		{
			
		}

		//get value for key (name)
		public object Get (ValueDto req)
		{
			var value = new Value ().PopulateWith (req);
			value.Hash=DHTServerCtx.HashFunction.apply (req.Key);
			var found = Db.Select<Value> (q => q.HashHexString == value.HashHexString);

			if (found.Count == 0) {
				return new HttpResult { StatusCode = HttpStatusCode.NotFound };
			}

			var valueDto = found.Select (v => new ValueDto ().PopulateWith (v)).First ();
			return new ValueDtoResponse (valueDto);
		}

		//create or update value for key (name)
		public object Put (ValueDto req)
		{
			var value = new Value ().PopulateWith (req);
			value.Hash=DHTServerCtx.HashFunction.apply (req.Key);
			var found = Db.Select<Value> (q => q.HashHexString == value.HashHexString);
	
			if (found.Count == 0) {
				Db.Insert<Value> (value);
			} else {
				Db.Save<Value> (value);
			}

			return new HttpResult { StatusCode = HttpStatusCode.Accepted };
		}

		//delete value for key (name)
		public object Delete (ValueDto req)
		{
			var value = new Value ().PopulateWith (req);
			value.Hash=DHTServerCtx.HashFunction.apply (req.Key);
			var found = Db.Select<Value> (q => q.HashHexString == value.HashHexString);

			if (found.Count == 0) {
				return new HttpResult { StatusCode = HttpStatusCode.NotFound };
			}

			Db.Delete<Value> (q => q.HashHexString == value.HashHexString);
			return new HttpResult { StatusCode = HttpStatusCode.NoContent };
		}
	}
}

