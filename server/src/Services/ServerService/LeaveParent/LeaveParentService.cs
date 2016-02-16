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
	public class LeaveParentService: Service
	{
		public static ILog log = LogManager.GetLogger (typeof(LeaveParentService));

		//new Server want to join DHT
		public object Post (LeaveParentDto req)
		{
			DHTServerCtx.DHT.Parent = req.Parent;

			log.Info (string.Format ("Old parent left (new parent {0})", DHTServerCtx.DHT.Child));
			log.Debug (DHTServerCtx.DHT);
			return new HttpResult {
				StatusCode = HttpStatusCode.Accepted
			};
		}

	}
}

