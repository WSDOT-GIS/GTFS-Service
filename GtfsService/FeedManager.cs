using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Wsdot.Gtfs.Contract;
using Wsdot.Gtfs.IO;

namespace GtfsService
{
	/// <summary>
	/// Manages the in-memory GTFS feeds. Retrieves remote feeds if there is no corresponding feed
	/// or if a newer feed is available than what is stored.
	/// </summary>
	internal class FeedManager
	{
		SynchronizedCollection<FeedRecord> _feedList = new SynchronizedCollection<FeedRecord>();
		static private FeedManager _instance = null;

		protected FeedManager()
		{
		}

		public static FeedManager GetInstance()
		{
			if (_instance == null)
				_instance = new FeedManager();
			return _instance;
		}

		/// <summary>
		/// Retrieves GTFS data for a specified agency.
		/// Data will be retrieved from an in-memory list if available and up-to-date.
		/// Otherwise the GTFS data will be requested from the GTFS Data Exchange website
		/// and stored for future use.
		/// </summary>
		/// <param name="agencyId">The GTFS-Data-Exchange agency identifier.</param>
		/// <returns></returns>
		public GtfsFeed this[string agencyId]
		{
			get {
				if (string.IsNullOrWhiteSpace(agencyId))
				{
					throw new ArgumentException("The agencyId was not provided.");
				}

				HttpClient client = null;
				// Get the record with the matching agency ID.
				var feedRecord = _feedList.FirstOrDefault(r => string.Compare(r.AgencyId, agencyId, true) == 0);
				GtfsFeed gtfs = feedRecord != null ? feedRecord.GtfsData : null;
				
				AgencyResponse agencyResponse = null;

				try
				{
					// Check for an existing record...

					// Check online to see if there is a newer feed available. 
					// This will be skipped if there is no matching GTFS feed stored for the specified agency.
					const string urlFmt = "http://www.gtfs-data-exchange.com/api/agency?agency={0}";
					Uri uri = new Uri(string.Format(urlFmt, agencyId));

					client = new HttpClient();

					client.GetStringAsync(uri).ContinueWith((t) =>
					{
						JsonConvert.DeserializeObjectAsync<AgencyResponse>(t.Result).ContinueWith(agencyResponseTask =>
						{
							agencyResponse = agencyResponseTask.Result;
						}).Wait();
					}).Wait();

					if (feedRecord == null || (agencyResponse.data.agency.date_last_updated > feedRecord.DateLastUpdated))
					{
						// Get the GTFS file...
						Uri zipUri = new Uri(String.Join("/", agencyResponse.data.agency.dataexchange_url.TrimEnd('/'), "latest.zip"));

						// TODO: make the request and parse the GTFS...
						if (client == null) client = new HttpClient();

						client.GetStreamAsync(zipUri).ContinueWith(t => {

							Task.Run(() =>
							{
								gtfs = GtfsReader.ReadGtfs(t.Result);
							}).ContinueWith((gtfsTask) =>
							{
								// Delete the existing feedRecord.
								if (feedRecord != null)
								{
									_feedList.Remove(feedRecord);
								}
								// Add the new GTFS feed data to the in-memory collection.
								_feedList.Add(new FeedRecord
								{
									GtfsData = gtfs,
									AgencyId = agencyId,
									DateLastUpdated = agencyResponse.data.agency.date_last_updated
								});
							}).Wait();
						}).Wait();
					}

				}
				finally
				{
					if (client != null)
					{
						client.Dispose();
					}
				}




				return gtfs;
			}
		}
		
	}
}