using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Server.Logic.Value;
using Server.Services.ValueService;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;

namespace Server.Services.ServerService
{
	public class JoinParentService: Service
	{
		public static ILog log = LogManager.GetLogger (typeof(JoinParentService));

		//new Server want to join DHT
		public object Post (JoinParentDto req)
		{
			DHTServerCtx.DHT.Parent = req.Parent;

			log.Info (string.Format ("{0} joined as parent", DHTServerCtx.DHT.Parent));
			log.Debug (DHTServerCtx.DHT);
			return new HttpResult {
				StatusCode = HttpStatusCode.Accepted
			};
		}

	}
}

