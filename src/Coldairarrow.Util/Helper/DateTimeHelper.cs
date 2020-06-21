using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Util
{
    public static class DateTimeHelper
    {

        #region String扩展

        /// <summary>
        /// 字符串格式时间转化为（UTC）DateTime
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ChinaTimeToUtc(this string s)
        {
            var t = DateTime.SpecifyKind(DateTime.Parse(s), DateTimeKind.Utc);
            return t.AddHours(-8);
        }

        #endregion

        #region DateTime 扩展
        
        /// <summary>
        /// 获取（中国）DateTime
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime UtcToChinaTime(this DateTime s)
        {
            TimeZoneInfo targetZone = TimeZoneInfo.CreateCustomTimeZone("YCH Standard Time", new TimeSpan(8, 0, 0), "YCH Standard Time", "YCH Standard Time");
            return TimeZoneInfo.ConvertTime(s, targetZone);
        }

        /// <summary>
        /// 返回毫秒
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long UnixTimestamp(this DateTime s)
        {
            return (s.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// 返回毫秒
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long UnixTimestamp(this DateTimeOffset s)
        {
            return (s.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }
        #endregion


        #region long扩展

        /// <summary>
        /// 获取（中国）DateTime
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime UtcToChinaTime(this long ts)
        {
            return ParseDateTime(ts, TimeSpan.FromHours(8));
        }

        /// <summary>
        /// 获取（UTC）DateTime
        /// </summary>
        /// <param name="ts">时间戳</param>
        /// <returns></returns>
        public static DateTime ToUTCDateTime(this long ts)
        {
            return new DateTime(ts.Ticks(), DateTimeKind.Utc);
        }

        /// <summary>
        /// 获取日期时间戳
        /// 入参：1561707375000 = 2019/6/28 15:36:15，回参：1561651200000 = 2019/6/28 0:0:0
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static long ToChinaDate(this long ts)
        {
            var ds = new DateTimeOffset(ts.UtcToChinaTime().Date, TimeSpan.FromHours(8));
            return ds.UnixTimestamp();
        }
        

        /// <summary>
        /// 时间戳转换时间
        /// </summary>
        /// <param name="lt">时间（毫秒）</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long lt)
        {
            return lt.UtcToChinaTime();
        }

        /// <summary>
        /// 基于UTC偏移获取DateTime
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(this long ts, TimeSpan baseUtcOffset)
        {
            var ds = new DateTimeOffset(
                ts.OffsetTime(baseUtcOffset).Ticks(),
                baseUtcOffset);
            return ds.DateTime;
        }

        /// <summary>
        /// 时间戳偏移
        /// </summary>
        /// <param name="ts">时间戳</param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public static long OffsetTime(this long ts, TimeSpan offset)
        {
            return ts + (long)offset.TotalMilliseconds;
        }

        /// <summary>
        /// 获取ticks
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static long Ticks(this long ts)
        {
            return ts * 10000 + 621355968000000000;
        } 

        #endregion

    }
}