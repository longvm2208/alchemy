using System;

public class TimePref : PrefBase<TimePref>
{
    public SerializableDateTime LastDate;
    
    public TimePref()
    {
        DateTime now = DateTime.Now;

        if (TimeManager.Ins != null && TimeManager.Ins.IsFetched)
        {
            now = TimeManager.Ins.Now;
        }

        LastDate = new SerializableDateTime(now.Date);
    }
}
