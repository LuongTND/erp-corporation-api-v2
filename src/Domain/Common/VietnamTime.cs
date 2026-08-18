namespace Domain;

public static class VietnamTime
{
    // Vietnam không có daylight saving, luôn UTC+7
    public static readonly TimeSpan Offset = TimeSpan.FromHours(7);

    public static DateTimeOffset Now => DateTimeOffset.UtcNow.ToOffset(Offset);
    public static DayOfWeek Today => Now.DayOfWeek;
    public static DateOnly TodayDate => DateOnly.FromDateTime(Now.DateTime);
}
