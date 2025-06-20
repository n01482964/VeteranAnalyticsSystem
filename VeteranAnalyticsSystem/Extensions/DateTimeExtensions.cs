namespace VeteranAnalyticsSystem.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToEasternTime(this DateTime utcDateTime)
    {
        DateTime utcTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, easternZone);
    }
}

