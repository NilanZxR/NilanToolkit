using System;
using System.Collections;
using System.Collections.Generic;

namespace NilanToolkit.CSharpExtensions {
    public static partial class CSExt {

        #region DeltaTime

        public static long ToTimeStamp(this DateTime dateTime) {
            DateTime UnixTimeStampStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - UnixTimeStampStart).TotalSeconds;
        }

        public static DateTime ToDateTime(this long seconds) {
            DateTime UnixTimeStampStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return UnixTimeStampStart.AddSeconds(seconds).ToLocalTime();
        }

        #endregion

        #region String

        public static bool NotNullOrEmpty(this string str) {
            return !string.IsNullOrEmpty(str);
        }
        
        #endregion

    }
}