namespace Shared.Service;

public class TimezoneConvertService
{
    public static DateTime ConvertUtcToLocal(DateTime dateTime, string timezoneId)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timezoneId));
    }
    
    public static DateTime ConvertLocalToUtc(DateTime dateTime, string timezoneId)
    {
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timezoneId));
    }
}
