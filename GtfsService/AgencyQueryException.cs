using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GtfsService
{
	[Serializable]
	public class AgencyQueryException : Exception
	{
		public int StatusCode { get; protected set; }

		public AgencyQueryException(AgencyResponse agencyResponse)
			: base(agencyResponse.status_txt)
		{
			StatusCode = agencyResponse.status_code;
		}
		public AgencyQueryException() { }
		public AgencyQueryException(string message) : base(message) { }
		public AgencyQueryException(string message, Exception inner) : base(message, inner) { }
		protected AgencyQueryException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}