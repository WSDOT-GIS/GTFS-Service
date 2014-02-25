using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GtfsService
{
	public class AgencyResponse
	{
		public class AgencyData
		{
			public class AgencyInfo
			{
				public double date_last_updated { get; set; }
				public string feed_baseurl { get; set; }
				public string name { get; set; }
				public string area { get; set; }
				public string url { get; set; }
				public string country { get; set; }
				public string state { get; set; }
				public string license_url { get; set; }
				public string dataexchange_url { get; set; }
				public double date_added { get; set; }
				public bool is_official { get; set; }
				public string dataexchange_id { get; set; }
			}

			public class Datafile
			{
				public string description { get; set; }
				public string md5sum { get; set; }
				public string file_url { get; set; }
				public List<string> agencies { get; set; }
				public string filename { get; set; }
				public double date_added { get; set; }
				public string uploaded_by_user { get; set; }
				public long size { get; set; }
			}

			public AgencyInfo agency { get; set; }
			public List<Datafile> datafiles { get; set; }
		}



		public string status_txt { get; set; }
		public int status_code { get; set; }
		public AgencyData data { get; set; }
	}
}