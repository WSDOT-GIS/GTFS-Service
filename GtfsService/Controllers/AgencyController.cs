using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace GtfsService.Controllers
{
	public class AgencyController : ApiController
	{
		[Route("api/agency/{dataexchange_id?}")]
		[Route("api/agencies/{dataexchange_id?}")]
		public HttpResponseMessage Get(string dataexchange_id=null)
		{
			var url = ConfigurationManager.AppSettings["gtfs-url"].TrimEnd('/');
			var urlSuffix = string.IsNullOrWhiteSpace(dataexchange_id) ? "/api/agencies" : string.Format("/api/agency?agency={0}", dataexchange_id);
			url = url + urlSuffix;
			HttpResponseMessage output = Request.CreateResponse(HttpStatusCode.Redirect);
			output.Headers.Location = new Uri(url);
			return output;
		}
	}
}
