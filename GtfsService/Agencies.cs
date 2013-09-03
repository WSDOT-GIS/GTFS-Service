using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System.Configuration;
using System.Net;
using System.IO;

namespace GtfsService
{
	[Route("/agencies")]
	public class Agencies
	{
	}

	public class AgenciesService: Service
	{
		/// <summary>
		/// Redirects to the gtfs-data-exchange.com agencies list.
		/// </summary>
		/// <param name="request"></param>
		public void Any(Agencies request)
		{
			var url = ConfigurationManager.AppSettings["gtfs-url"].TrimEnd('/') + "/api/agencies";
			base.Response.RedirectToUrl(url);
		}
	}

}