using System;
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
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); 
        long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; 
        return timeStamp;
    }

    public static DateTime ToDateTime(this long seconds) {
        long unixTimeStamp = 1478162177;
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); 
        DateTime dt = startTime.AddSeconds(unixTimeStamp);
        return dt;
    }

    #endregion
    
    
}
}