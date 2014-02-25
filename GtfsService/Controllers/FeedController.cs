using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Wsdot.Gtfs.Contract;
using Wsdot.Gtfs.IO;

namespace GtfsService.Controllers
{
	public class FeedController : ApiController
	{
		FeedManager _feedManager = FeedManager.GetInstance();

		[Route("api/feed/{agency}")]
		public HttpResponseMessage Get(string agency)
		{
			DateTimeOffset? ifModifiedSince = Request.Headers.IfModifiedSince;

			FeedRecord feedRecord = _feedManager[agency, ifModifiedSince];

			HttpResponseMessage output;

			if (ifModifiedSince.HasValue && feedRecord.DateLastUpdated <= ifModifiedSince.Value)
			{
				output = Request.CreateResponse(HttpStatusCode.NotModified);
			}
			else
			{
				output = Request.CreateResponse<GtfsFeed>(feedRecord.GtfsData);
				output.Headers.Add("Date-Last-Modified", feedRecord.DateLastUpdated.ToString());
			}

			output.Headers.CacheControl = new CacheControlHeaderValue
			{
				NoCache = false,
				Public = true
			};

			return output;
		}
	}
}