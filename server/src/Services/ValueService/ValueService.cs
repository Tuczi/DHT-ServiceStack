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
using System.Numerics;

namespace Server.Services.ValueService
{
	public class ValueService : Service
	{
		public static ILog log = LogManager.GetLogger (typeof(ValueService));

		public ValueService () {}

		//get value for key (name)
		public object Get (ValueDto req) {
			var hash = DHTServerCtx.HashFunction.apply (req.Key);

			var redir = this.checkRedirect (hash, req.Key);
			if (redir != null) {
				return redir;
			}
				
			var found = Db.Select<Value> (q => q.Hash == hash);

			if (found.IsEmpty()) {
				return new HttpResult { StatusCode = HttpStatusCode.NotFound };
			}

			var valueDto = found.Select (v => new ValueDto ().PopulateWith (v)).First ();
			return new ValueDtoResponse (valueDto);
			
		}

		//create or update value for key (name)
		public object Put (ValueDto req) {
			var hash = DHTServerCtx.HashFunction.apply (req.Key);

			var redir = this.checkRedirect (hash, req.Key);
			if (redir != null) {
				return redir;
			}

			var found = Db.Select<Value> (q => q.Hash == hash);

			var value = new Value ().PopulateWith (req);
			value.Hash = hash;
	
			if (found.IsEmpty()) {
				Db.Insert<Value> (value);
			} else {
				Db.Save<Value> (value);
			}

			return new HttpResult { StatusCode = HttpStatusCode.Accepted };
		}

		//delete value for key (name)
		public object Delete (ValueDto req) {
			var hash = DHTServerCtx.HashFunction.apply (req.Key);
			var redir = this.checkRedirect (hash, req.Key);
			if (redir != null) {
				return redir;
			}

			var found = Db.Select<Value> (q => q.Hash == hash);

			if (found.IsEmpty()) {
				return new HttpResult { StatusCode = HttpStatusCode.NotFound };
			}

			Db.Delete<Value> (q => q.Hash == hash);
			return new HttpResult { StatusCode = HttpStatusCode.NoContent };
		}

		private HttpResult checkRedirect(BigInteger hash, string Key) {
			var hashRange = DHTServerCtx.DHT.HashRange;

			//Todo: check if Location is not empty
			if (BigInteger.Compare(hash, hashRange.Min) < 0) {
				log.Debug (String.Format ("redirect to parent: {2}, min: {0}, hash: {1}", hashRange.Min.ToString(), hash.ToString(), DHTServerCtx.DHT.Parent));
				//redirect to parent
				return new HttpResult {
					StatusCode = HttpStatusCode.RedirectKeepVerb,
					Location = DHTServerCtx.DHT.Parent + "value/" + Key
				};
			} else if (BigInteger.Compare(hash, hashRange.Max) >= 0) {
				log.Debug (String.Format ("redirect to child: {2}, max: {0}, hash: {1}", hashRange.Max.ToString(), hash.ToString(), DHTServerCtx.DHT.Child));

				//redirect to child
				return new HttpResult {
					StatusCode = HttpStatusCode.RedirectKeepVerb,
					Location = DHTServerCtx.DHT.Child + "value/" + Key
				};
			}

			return null;
		}
	}
}

