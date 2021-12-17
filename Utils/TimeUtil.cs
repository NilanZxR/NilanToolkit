using System;

namespace NilanToolkit.Utils {
    public static class TimeUtil {

        public static DateTime DayStartTime(this DateTime time) {
            return time.AddHours(-time.Hour).AddMinutes(-time.Minute).AddSeconds(-time.Second);
        }

        public static DateTime DayEndTime(this DateTime time) {
            return time.DayStartTime().AddDays(1).AddSeconds(-1);
        }
        
        public static DateTime WeekStartTime(this DateTime time) {
            int dayOfWeek = (int) time.Date.DayOfWeek;
            DateTime weekStartTime = time.AddDays(-dayOfWeek); 
            return weekStartTime.Date;
        }

        public static DateTime WeekEndTime(this DateTime time) {
            return WeekStartTime(time).AddDays(7);
        }

        public static DateTime MonthStartTime(this DateTime time) {
            return new DateTime(time.Year, time.Month, 0);
        }

        public static DateTime MonthEndTime(this DateTime time) {
            return new DateTime(time.Year, time.Month + 1, 0).AddDays(-1);
        }
        
    }
    
}
