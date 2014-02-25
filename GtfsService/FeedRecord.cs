using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Wsdot.Gtfs.Contract;
using Wsdot.Gtfs.IO;

namespace GtfsService
{
	/// <summary>
	/// A record of GTFS data, associated agency, and when it was last updated.
	/// </summary>
	internal class FeedRecord
	{
		public string AgencyId { get; set; }
		public double DateLastUpdated { get; set; }
		public GtfsFeed GtfsData { get; set; }
	}
}