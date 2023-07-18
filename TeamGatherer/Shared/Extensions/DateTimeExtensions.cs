using System;

namespace TeamGatherer.Shared.Extensions;

public static class DateTimeExtensions
{
    public static DateTime Truncate(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, date.Kind);
    }

    public static long ToUnixTimestamp(this DateTime date)
    {
        return ((DateTimeOffset) date).ToUnixTimeSeconds();
    }

    public static DateTime FromUnixTimestamp(this long timestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
    }
}

