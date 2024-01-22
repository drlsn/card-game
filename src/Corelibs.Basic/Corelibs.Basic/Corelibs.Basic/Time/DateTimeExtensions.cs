namespace Corelibs.Basic.Time;

public static class DateTimeExtensions
{
    public static DateTime ToMorning(this DateTime date, int morningHour = 4) =>
        date.AddHours(morningHour - date.Hour);
}
