using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Blogical.Shared.Adapters.Common.Schedules
{
    #region Enums and flag attributes
    /// <summary>
    /// Type of Scheduler such as DAily, Weekly etc
    /// </summary>
    public enum ScheduleType
    {
        /// <summary>
        /// Not used
        /// </summary>
        None = 0,
        /// <summary>
        /// Daily schedule type
        /// </summary>
        Daily = 1,
        /// <summary>
        /// Weekly schedule type
        /// </summary>
        Weekly = 2,
        /// <summary>
        /// Monthly schedule type
        /// </summary>
        Monthly = 3,
        /// <summary>
        /// Timly schedule type
        /// </summary>
        Timely
    };
    /// <summary>
    /// Time Unit definition
    /// </summary>
    [FlagsAttribute]
    public enum ScheduleTimeType
    {
        /// <summary>
        /// Timely (seconds) schedule type
        /// </summary>
        Seconds = 0,
        /// <summary>
        /// Timely (minutes) schedule type
        /// </summary>
        Minutes = 1,
        /// <summary>
        /// Timely /Hours) schedule type
        /// </summary>
        Hours = 2
    };
    /// <summary>
    /// Days Unit definition
    /// </summary>
    [FlagsAttribute]
    public enum ScheduleDay
    {
        /// <summary>
        /// Not used
        /// </summary>
        None = 0,
        /// <summary>
        /// Execute on Sunday
        /// </summary>
        Sunday = 1,
        /// <summary>
        /// Execute on Mondays
        /// </summary>
        Monday = 2,
        /// <summary>
        /// Execute on Tuesdays
        /// </summary>
        Tuesday = 4,
        /// <summary>
        /// Execute on Wednesdays
        /// </summary>
        Wednesday = 8,
        /// <summary>
        /// Execute on Thursdays
        /// </summary>
        Thursday = 16,
        /// <summary>
        /// Execute on Fridays
        /// </summary>
        Friday = 32,
        /// <summary>
        /// Execute on Weekdays
        /// </summary>
        Weekday = 62,
        /// <summary>
        /// Execute on Saturdays
        /// </summary>
        Saturday = 64,
        /// <summary>
        /// Execute on Weekends
        /// </summary>
        Weekend = 65,
        /// <summary>
        /// Execute allways
        /// </summary>
        All = 127
    };
    /// <summary>
    /// Ordinal Unit definition
    /// </summary>
    [FlagsAttribute]
    public enum ScheduleOrdinal
    {
        /// <summary>
        /// Not used
        /// </summary>
        None = 0,
        /// <summary>
        /// First of
        /// </summary>
        First = 1,
        /// <summary>
        /// Second of
        /// </summary>
        Second = 2,
        /// <summary>
        /// Third of...
        /// </summary>
        Third = 4,
        /// <summary>
        /// Fourth of...
        /// </summary>
        Fourth = 8,
        /// <summary>
        /// All of...
        /// </summary>
        All = 15,
        /// <summary>
        /// Last of...
        /// </summary>
        Last = 16
    };
    /// <summary>
    /// Monthly Unit definition
    /// </summary>
    [FlagsAttribute]
    public enum ScheduleMonth
    {
        /// <summary>
        /// Not used
        /// </summary>
        None = 0,
        /// <summary>
        /// January
        /// </summary>
        January = 1,
        /// <summary>
        /// February
        /// </summary>
        February = 2,
        /// <summary>
        /// March
        /// </summary>
        March = 4,
        /// <summary>
        /// April
        /// </summary>
        April = 8,
        /// <summary>
        /// May
        /// </summary>
        May = 16,
        /// <summary>
        /// June
        /// </summary>
        June = 32,
        /// <summary>
        /// July
        /// </summary>
        July = 64,
        /// <summary>
        /// August
        /// </summary>
        August = 128,
        /// <summary>
        /// September
        /// </summary>
        September = 256,
        /// <summary>
        /// October
        /// </summary>
        October = 512,
        /// <summary>
        /// November
        /// </summary>
        November = 1024,
        /// <summary>
        /// December
        /// </summary>
        December = 2048,
        /// <summary>
        /// StartofQuarter
        /// </summary>
        StartofQuarter = 585,
        /// <summary>
        /// EndofQuarter
        /// </summary>
        EndofQuarter = 2340,
        /// <summary>
        /// All
        /// </summary>
        All = 4095
    };
    #endregion
    
    /// <summary>
    /// Base Schedule that inherits from Microsoft.BizTalk.Scheduler.ISchedule.
    /// Every other Schedule derives from this Scheduler
    /// </summary>
    [Serializable()]
    public abstract class Schedule : Microsoft.BizTalk.Scheduler.ISchedule
    {
        // Events
        /// <summary>
        /// Event triggerd when properties are updated
        /// </summary>
        public event EventHandler Changed;
        // Fields
        /// <summary>
        /// Type of Scheduler such as Daily, Weekly etc
        /// </summary>
        protected ScheduleType type;
        /// <summary>
        /// DateTime when schedule should start
        /// </summary>
        protected object starttime;
        /// <summary>
        /// DateTime when schedule should end
        /// </summary>
        protected object startdate;

        //Properties
        /// <summary>
        /// Type of Scheduler such as Daily, Weekly etc
        /// </summary>
        public ScheduleType Type
        {
            get
            {
                return this.type;
            }
        }
        /// <summary>
        /// DateTime when schedule should start
        /// </summary>
        public virtual DateTime StartTime
        {
            get
            {
                return (DateTime)this.starttime;
            }
            set
            {
                DateTime newDate = new DateTime(1900, 1, 1, value.Hour, value.Minute, value.Second);
                if (newDate != (DateTime)Interlocked.Exchange(ref this.starttime, (object)newDate))
                {
                    this.FireChangedEvent();
                }
            }
        }
        /// <summary>
        /// DateTime when schedule should end
        /// </summary>
        public virtual DateTime StartDate
        {
            get
            {
                return (DateTime)this.startdate;
            }
            set
            {
                DateTime newDate = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                if (newDate != (DateTime)Interlocked.Exchange(ref this.startdate, (object)newDate))
                {
                    this.FireChangedEvent();
                }
            }
        }

        //Methods
        /// <summary>
        /// Constructor
        /// </summary>
        public Schedule()
        {
            this.type = ScheduleType.None;
            this.starttime = new DateTime(1900, 1, 1, 0, 0, 0);
            this.startdate = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void FireChangedEvent()
        {
            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Returns the next time the schedule will be triggerd
        /// </summary>
        /// <returns></returns>
        public abstract DateTime GetNextActivationTime();

        //
        //Configuration Xml Handling
        //
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static string Extract(XmlDocument document, string path, bool required)
        {
            XmlNode node = document.SelectSingleNode(path);
            if (!required && null == node)
                return String.Empty;
            if (null == node)
                throw new ApplicationException(string.Format("Schedule property missing: {0} ", path));
            return node.InnerText;
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static int ExtractInt(XmlDocument document, string path)
        {
            string s = Extract(document, path, true);
            return int.Parse(s);
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static int IfExistsExtractInt(XmlDocument document, string path, int defaultValue)
        {
            string s = Extract(document, path, false);
            if (0 == s.Length)
                return defaultValue;
            return int.Parse(s);
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static DateTime ExtractDate(XmlDocument document, string path, bool required)
        {
            string s = Extract(document, path, required);
            return DateTime.Parse(s);
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static DateTime IfExistsExtractDate(XmlDocument document, string path, DateTime defaultValue)
        {
            string s = Extract(document, path, false);
            if (0 == s.Length)
                return defaultValue;
            return DateTime.Parse(s);
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static DateTime ExtractTime(XmlDocument document, string path, bool required)
        {
            string s = Extract(document, path, required);
            return DateTime.Parse("1900-01-01T" + s.Substring(0, 5) + ":00");
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static DateTime IfExistsExtractTime(XmlDocument document, string path, DateTime defaultValue)
        {
            string s = Extract(document, path, false);
            if (0 == s.Length)
                return defaultValue;
            return DateTime.Parse("1900-01-01T" + s.Substring(0, 5) + ":00");
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static ScheduleType ExtractScheduleType(XmlDocument document)
        {
            string type = document.DocumentElement.GetAttribute("type");
            if (type == String.Empty)
                throw new ApplicationException(string.Format("Schedule Type missing: "));
            return (ScheduleType)Enum.Parse(typeof(ScheduleType), type);
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static ScheduleType IfExistsExtractScheduleType(XmlDocument document)
        {
            string type = document.DocumentElement.GetAttribute("type");
            if (type == String.Empty)
                return ScheduleType.None;
            return (ScheduleType)Enum.Parse(typeof(ScheduleType), type);
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static ScheduleOrdinal ExtractScheduleOrdinal(XmlDocument document, string path, bool required)
        {
            string s = Extract(document, path, required);
            return (ScheduleOrdinal)Enum.Parse(typeof(ScheduleOrdinal), s);
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static ScheduleDay ExtractScheduleDay(XmlDocument document, string path, bool required)
        {
            string s = Extract(document, path, required);
            return (Blogical.Shared.Adapters.Common.Schedules.ScheduleDay)Enum.Parse(typeof(Blogical.Shared.Adapters.Common.Schedules.ScheduleDay), s);
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static ScheduleTimeType ExtractScheduleTimeType(XmlDocument document, string path, bool required)
        {
            string s = Extract(document, path, required);
            if (String.IsNullOrEmpty( s ) )
                return ScheduleTimeType.Seconds;

            return (ScheduleTimeType)Enum.Parse(typeof(ScheduleTimeType), s);
        }
        /// <summary>
        /// Used internally to read schedule configuration from Xml document
        /// </summary>
        public static ScheduleMonth ExtractScheduleMonth(XmlDocument document, string path, bool required)
        {
            string s = Extract(document, path, required);
            return (Blogical.Shared.Adapters.Common.Schedules.ScheduleMonth)Enum.Parse(typeof(Blogical.Shared.Adapters.Common.Schedules.ScheduleMonth), s);
        }
        //
        // Utility Methods
        //
        /// <summary>
        /// Converts the DateTime.DayOfWeek into a ScheduleDay flag
        /// </summary>
        protected ScheduleDay GetScheduleDayFlag(DateTime date)
        {
            return (ScheduleDay)(int)(Math.Pow(2, (int)date.DayOfWeek));
        }
        /// <summary>
        /// Converts the DateTime.Month into a ScheduleMonth flag
        /// </summary>
        protected ScheduleMonth GetScheduleMonthFlag(DateTime date)
        {
            return (ScheduleMonth)(Math.Pow(2, (int)date.Month - 1));
        }
        /// <summary>
        /// Determines the previous Sunday, from the date parameter
        /// </summary>
        protected DateTime GetLastSunday(DateTime date)
        {
            DateTime lastSunday = date.Date;
            if (lastSunday.DayOfWeek != DayOfWeek.Sunday)
            {
                for (int i = 1; i < 8; i++)
                {
                    lastSunday = lastSunday.AddDays(-1);
                    if (lastSunday.DayOfWeek == DayOfWeek.Sunday)
                        break;
                }
            }
            return lastSunday;
        }
        /// <summary>
        /// Determines the ordinal week day for the month
        /// e.g first Monday, last weekday or second Tuesday 
        /// </summary>
        protected int GetOrdinalWeekDay(ScheduleOrdinal ordinal, ScheduleDay weekday, int month, int year)
        {
            int result = -1;
            int index = 0;
            int[] days = new int[32];

            if ((ordinal == ScheduleOrdinal.None) || (weekday == ScheduleDay.None))
                return result;

            DateTime date = new DateTime(year, month, 1);
            int limit = DateTime.DaysInMonth(date.Year, date.Month);
            for (int i = 0; i < limit; i++)
            {
                if ((GetScheduleDayFlag(date) & weekday) > 0)
                {
                    days[index++] = date.Day;
                }
                date = date.AddDays(1);
            }
            switch (ordinal)
            {
                case ScheduleOrdinal.First:
                    result = days[0];
                    break;
                case ScheduleOrdinal.Second:
                    result = days[1];
                    break;
                case ScheduleOrdinal.Third:
                    result = days[2];
                    break;
                case ScheduleOrdinal.Fourth:
                    result = days[3];
                    break;
                case ScheduleOrdinal.Last:
                    result = days[index - 1];
                    break;
                default:
                    break;
            }
            return result;
        }
        /// <summary>
        /// Writes a message to the trace listeners .
        /// </summary>
        /// <param name="message"></param>
        public void TraceMessage(string message)
        {
            Trace.WriteLine(message);
        }
    }
}
