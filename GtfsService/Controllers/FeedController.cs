using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Wsdot.Gtfs.Contract;
using Wsdot.Gtfs.IO;

namespace GtfsService.Controllers
{
	public class FeedController : ApiController
	{
		FeedManager _feedManager = FeedManager.GetInstance();

		[Route("api/feed/{agency}")]
		public GtfsFeed Get(string agency)
		{
			return _feedManager[agency];
			////// Construct the gtfs-data-exchange URL for the specified agency.
			////// E.g., http://www.gtfs-data-exchange.com/agency/intercity-transit/latest.zip
			////string url = string.Format("{0}/agency/{1}/latest.zip", ConfigurationManager.AppSettings["gtfs-url"].TrimEnd('/'), agency);
			////var client = new HttpClient();
			////HttpResponseMessage output = null;
			////var task = client.GetAsync(url).ContinueWith((t) => {
			////	if (!t.Result.IsSuccessStatusCode)
			////	{
			////		output = Request.CreateErrorResponse(t.Result.StatusCode, t.Result.ReasonPhrase);
			////	}
			////	else
			////	{
			////		GtfsFeed feed = null;
			////		var response = t.Result;
			////		var streamTask = response.Content.ReadAsStreamAsync().ContinueWith((st) =>
			////		{
			////			feed = GtfsReader.ReadGtfs(st.Result);
			////			output = Request.CreateResponse<GtfsFeed>(HttpStatusCode.OK, feed);
			////			output.Headers.ETag = response.Headers.ETag;
			////			output.Headers.Date = response.Headers.Date;
			////			output.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
			////			{
			////				Public = true,
			////				MaxAge = default(TimeSpan?)
			////			};
			////		});
			////		streamTask.Wait();
			////	}
			////});
			////task.Wait();
			////return output;
		}
	}
}