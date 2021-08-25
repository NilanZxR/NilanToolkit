using System;
using System.Collections;
using System.Collections.Generic;

namespace NilanToolkit {
    public static class CSharpExtensions {

        #region Linq

        /// <summary>
        /// like linq.select, but only select item not null
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TRet> SelectNotNull<TSource,TRet>(this IEnumerable<TSource> source, Func<TSource,TRet> selector) 
        where TRet : class{
            var list = new List<TRet>();
            foreach (var item in source) {
                var selected = selector(item);
                if (selected == null) continue;
                list.Add(selected);
            }
            return list;
        }

        #endregion

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

        #region Collections

        public static bool IsInRange(this IList list, int index) {
            return index >= 0 && index < list.Count;
        }

        public static bool IsInRange(this Array arr, int index) {
            return index >= 0 && index < arr.Length;
        }


        #endregion

    }
}