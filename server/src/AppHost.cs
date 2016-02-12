using System;
using System.Configuration;
using Funq;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;
using ServiceStack.MiniProfiler;
using ServiceStack.MiniProfiler.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using System.Data;
using Server.Logic.Value;
using Server.Logic.DHT;

namespace Server
{
	public class AppHost : AppHostHttpListenerBase
	{
		private readonly bool m_debugEnabled = true;


		public AppHost ()
			: base ("Server HttpListener", typeof(AppHost).Assembly)
		{
		}

		//Create missing tables
		void SetUpDb (Container container)
		{
			var dbFactory = container.Resolve<IDbConnectionFactory> ();
			using (IDbConnection db = dbFactory.Open ()) {
				db.CreateTableIfNotExists<Value> ();
			}
		}

		public override void Configure (Container container)
		{
			LogManager.LogFactory = new NLogFactory ();
			this.RequestFilters.Add ((req, resp, requestDto) => {
				ILog log = LogManager.GetLogger (GetType ());
				log.Info (string.Format ("REQ {0}: {1} {2} {3} {4} {5}",
					DateTimeOffset.Now.Ticks, req.HttpMethod,
					req.OperationName, req.RemoteIp, req.RawUrl, req.UserAgent));
			});
			this.ResponseFilters.Add ((req, resp, dto) => {
				ILog log = LogManager.GetLogger (GetType ());
				log.Info (string.Format ("RES {0}: {1} {2}", DateTimeOffset.Now.Ticks,
					resp.StatusCode, resp.ContentType));
			});

			container.Register<IDbConnectionFactory> (
				new OrmLiteConnectionFactory (@"Data Source=db.sqlite;Version=3;",
					SqliteOrmLiteDialectProvider.Instance) {
					ConnectionFilter = x => new ProfiledDbConnection (x, Profiler.Current)
				});

			JsConfig.DateHandler = JsonDateHandler.ISO8601;
            
			Plugins.Add (new ValidationFeature ());
			container.RegisterValidators (typeof(AppHost).Assembly);

			var config = new EndpointHostConfig ();

			if (m_debugEnabled) {
				config.DebugMode = true; //Show StackTraces in service responses during development
				config.WriteErrorsToResponse = true;
				config.ReturnsInnerException = true;
			}

			SetConfig (config);

			SetUpDb (container);
		}
	}
}