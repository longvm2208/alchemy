using System;

namespace Toolkit
{
    [Serializable]
    public class TimeFormatter : AmountFormatter
    {
        public override string Format(int seconds) => string.Format(format, seconds.ToStringReward());
    }
}
