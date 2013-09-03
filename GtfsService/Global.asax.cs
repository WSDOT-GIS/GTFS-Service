using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace GtfsService
{
	public class Global : System.Web.HttpApplication
	{
		public class AppHost: AppHostBase
		{
			public AppHost(): base("GTFS Services", Assembly.GetExecutingAssembly())
			{

			}

			public override void Configure(Funq.Container container)
			{
				container.Register<ICacheClient>(new MemoryCacheClient());
			}
		}


		protected void Application_Start(object sender, EventArgs e)
		{
			new AppHost().Init();
		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}