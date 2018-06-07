using NLog;
using System;

namespace WinServiceMon
{
    public class AlertMessage
    {
        public AlertMessage()
        {
            TimeStamp = new DateTime();
        }
        public string ServiceName { get; set; }
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public DateTime TimeStamp { get; protected set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return this == (AlertMessage)obj;

        }

        public static bool operator == (AlertMessage alert1, AlertMessage alert2)
        {
            return alert1.ServiceName == alert2.ServiceName &&
                alert1.Message == alert2.Message &&
                alert1.Level == alert2.Level;
        }

        public static bool operator !=(AlertMessage alert1, AlertMessage alert2)
        {
            return alert1.ServiceName != alert2.ServiceName ||
                alert1.Message != alert2.Message ||
                alert1.Level != alert2.Level;
        }
    }
}
