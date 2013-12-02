namespace OpenAA
{
    using System;

    public static class DateTimeUtility
    {
        public static DateTime UnixTimeToDateTime(double unixTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTime).ToLocalTime();
        }
    }
}

