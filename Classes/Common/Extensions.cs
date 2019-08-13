using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Extensions
{
	public static class Extensions
	{
		public static int ToInt(this Enum enumValue)
		{
			return Convert.ToInt32(enumValue);
		}

		public static DateTime fromUnixTimestamp(this uint _timeStamp)
		{
			System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
			// Add the timestamp (number of seconds since the Epoch) to be converted
			return dateTime.AddSeconds(_timeStamp);			
		}
	}
}
