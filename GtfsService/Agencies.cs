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
	/// <summary>
	/// Gets a list of agencies.
	/// </summary>
	[Route("/agencies")]
	[Route("/agencies/{dataexchange_id}")]
	[Route("/agency/{dataexchange_id}")]
	public class Agencies
	{
		public string dataexchange_id { get; set; }
	}

	public class AgenciesService: Service
	{
		/// <summary>
		/// Redirects to the gtfs-data-exchange.com agencies list.
		/// </summary>
		/// <param name="request"></param>
		public void Any(Agencies request)
		{
			Console.WriteLine(base.Request.GetFormatModifier());
			var url = ConfigurationManager.AppSettings["gtfs-url"].TrimEnd('/');
			var urlSuffix = string.IsNullOrWhiteSpace(request.dataexchange_id) ? "/api/agencies" : string.Format("/api/agency?agency={0}", request.dataexchange_id);
			url = url + urlSuffix;
			base.Response.RedirectToUrl(url);
		}
	}

}