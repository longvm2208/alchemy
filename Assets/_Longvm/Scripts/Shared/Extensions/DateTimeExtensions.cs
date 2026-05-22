using System;

public static class DateTimeExtensions
{
    /// <summary>
    /// Returns the next occurrence of the specified day of the week from the given DateTime.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static DateTime Next(this DateTime dateTime, DayOfWeek dayOfWeek)
    {
        int daysToAdd = ((int)dayOfWeek - (int)dateTime.DayOfWeek + 7) % 7;
        daysToAdd = daysToAdd == 0 ? 7 : daysToAdd; // If today is the target day, go to next week
        return dateTime.AddDays(daysToAdd);
    }

    public static DateTime NextMonth(this DateTime dateTime)
    {
        int year = dateTime.Month == 12 ? dateTime.Year + 1 : dateTime.Year;
        int month = dateTime.Month == 12 ? 1 : dateTime.Month + 1;
        return new DateTime(year, month, 1);
    }
}
