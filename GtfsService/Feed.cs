using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Configuration;
using System.Net;
using Wsdot.Gtfs.Contract;
using Wsdot.Gtfs.IO;

namespace GtfsService
{
	/// <summary>
	/// Request for an agencies GTFS feed.
	/// </summary>
	[Route("/feed/{agency}")]
	public class Feed
	{
		public string agency { get; set; }
	}

	/// <summary>
	/// Retrieves a GTFS ZIP file and converts the CSV files to objects.
	/// </summary>
	public class FeedService : Service
	{
		public GtfsFeed Any(Feed request)
		{
			// Test to make sure that an agency was specified.
			if (string.IsNullOrWhiteSpace(request.agency))
			{
				throw new ArgumentNullException("Agency ID not provided.");
			}

			// Construct the gtfs-data-exchange URL for the specified agency.
			// E.g., http://www.gtfs-data-exchange.com/agency/intercity-transit/latest.zip
			string url = string.Format("{0}/agency/{1}/latest.zip", ConfigurationManager.AppSettings["gtfs-url"].TrimEnd('/'), request.agency);

			HttpWebRequest zipRequest = WebRequest.CreateHttp(url);

			GtfsFeed gtfs;

			using (var response = (HttpWebResponse)zipRequest.GetResponse())
			{
				using (var stream = response.GetResponseStream())
				{
					gtfs = stream.ReadGtfs();
				}
			}

			return gtfs;
		}
	}
}