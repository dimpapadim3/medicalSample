using System;
using System.Globalization;

namespace Business.Interfaces
{
    public class TimeSpan
    {
        public virtual bool InTimeIncluded(DateTime now)
        {
            return true;
        }

        public virtual string GetSpanDate()
        {
            return ServerDateTimeProvider.Controller.GetServerDateTime().ToString(CultureInfo.InvariantCulture);
        }

        public virtual double TotalDuration
        {
            get { return 0; }
        }
    }


}