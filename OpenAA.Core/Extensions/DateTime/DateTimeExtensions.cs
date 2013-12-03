namespace OpenAA.Extensions.DateTime
{
    using System;

    /// <summary>
    /// System.DateTimeの拡張クラス
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 0分0秒000ミリ秒を取得する。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime BeginOfHour(this DateTime source)
        {
            return new DateTime(source.Year, source.Month, source.Day, source.Hour,  0,  0, 000);
        }

        /// <summary>
        /// 59分59秒999ミリ秒を取得する。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime EndOfHour(this DateTime source)
        {
            return new DateTime(source.Year, source.Month, source.Day, source.Hour, 59, 59, 999);
        }

        /// <summary>
        /// 0時0分0秒000ミリ秒を取得する。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime BeginOfDay(this DateTime source)
        {
            return new DateTime(source.Year, source.Month, source.Day,  0,  0,  0, 000);
        }

        /// <summary>
        /// 23時59分59秒999ミリ秒を取得する。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime source)
        {
            return new DateTime(source.Year, source.Month, source.Day, 23, 59, 59, 999);
        }

        /// <summary>
        /// 1日0時0分0秒000ミリ秒を取得する。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime BeginOfMonth(this DateTime source) 
        {  
            return new DateTime(source.Year, source.Month, 1).BeginOfDay();  
        }

        /// <summary>
        /// 末日23時59分59秒999ミリ秒を取得する。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime EndOfMonth(this DateTime source)
        {
            // 月の日数を取得する == 末日
            var day = DateTime.DaysInMonth(source.Year, source.Month);  
            return new DateTime(source.Year, source.Month, day).EndOfDay();
        }

        /// <summary>
        /// 締日の開始日を取得する。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cutOffDay"></param>
        /// <returns></returns>
        public static DateTime BeginOfCutOff(this DateTime source, int cutOffDay)
        {
            return source.AddMonths(-1).EndOfCutOff(cutOffDay).AddDays(1).BeginOfDay();
        }

        /// <summary>
        /// 締日の終了日を取得する。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cutOffDay"></param>
        /// <returns></returns>
        public static DateTime EndOfCutOff(this DateTime source, int cutOffDay)
        {
            DateTime work = source;

            int fixedCutOff;
            int endOfMonth = DateTime.DaysInMonth(work.Year, work.Month);
            if (cutOffDay <= 0)
            {
                fixedCutOff = endOfMonth;
            }
            else
            {
                fixedCutOff = System.Math.Min(cutOffDay, endOfMonth);
            }

            if (fixedCutOff < work.Day)
            {
                work = work.AddMonths(1);

                //
                int endOfNextMonth = DateTime.DaysInMonth(work.Year, work.Month);
                fixedCutOff = System.Math.Min(cutOffDay, endOfNextMonth);
            }

            return new DateTime(work.Year, work.Month, fixedCutOff).EndOfDay();
        }

        public static double ToUnixTime(this DateTime source)
        {
            var utc = source.ToUniversalTime();
            var elapsed = utc - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return elapsed.TotalSeconds;
        }
    }
}

