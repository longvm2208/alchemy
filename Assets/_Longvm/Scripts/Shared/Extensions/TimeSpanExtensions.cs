using System;

public static class TimeSpanExtensions
{
    public static string ToStringCountdown(this int seconds)
    {
        return ToStringCountdown(TimeSpan.FromSeconds(seconds));
    }

    public static string ToStringCountdown(this TimeSpan t)
    {
        if (t.TotalSeconds < 60)
        {
            return t.Seconds.ToString("00") + "s";
        }
        else if (t.TotalSeconds < 3600)
        {
            return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
        else if (t.TotalSeconds < 3600 * 24)
        {
            return string.Format("{0}h {1}m", t.Hours, t.Minutes);
        }
        else
        {
            return string.Format("{0}d {1}h", t.Days, t.Hours);
        }
    }

    public static string ToStringReward(this int seconds)
    {
        int days = seconds / (60 * 60 * 24);
        seconds -= days * (60 * 60 * 24);
        int hours = seconds / (60 * 60);
        seconds -= hours * (60 * 60);
        int minutes = seconds / 60;
        seconds -= minutes * 60;

        string s = "";
        if (days > 0) s += $"{days}d";
        if (hours > 0) s += $"{hours}h";
        if (minutes > 0) s += $"{minutes}m";
        if (seconds > 0) s += $"{seconds}s";

        return s;
    }

    public static string ToStringReward(this TimeSpan t)
    {
        return ToStringReward((int)t.TotalSeconds);
    }

    public static bool TryConvertToSeconds(this TimeSpan timeSpan, out int seconds)
    {
        double totalSeconds = timeSpan.TotalSeconds;
        if (totalSeconds > int.MaxValue || totalSeconds < int.MinValue)
        {
            seconds = 0;
            return false;
        }
        seconds = (int)totalSeconds;
        return true;
    }
}
