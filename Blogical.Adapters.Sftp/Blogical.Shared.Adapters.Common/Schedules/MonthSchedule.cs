using System;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace  Blogical.Shared.Adapters.Common.Schedules
{
	/// <summary>
	/// Monthly Schedule class supporting  Microsoft.Biztalk.Scheduler.ISchedule interface.
	/// Allows scheduling by day of month (e.g. 1st of January and June)  or ordinal week day (e.g. first Monday of August)
	/// </summary>
	[Serializable()]
	public class MonthSchedule: Schedule
	{
		// Fields
		private int day = 0;							//day of month
		private object ordinal = 0;				//ordinal week day (first, last..)
		private object weekday = 0;				//day of week ( Monday, Tuesday...)
		private object months = 0;				//months of year flag
		// Properties
        /// <summary>
        /// Every...
        /// </summary>
		public int Day 
		{
			get
			{
				return this.day;
			}
			set
			{
				if (value < 0 || value > 31)
				{
					throw (new ArgumentOutOfRangeException("value", "Day range: 0 - 31"));
				}
				if (value != Interlocked.Exchange(ref this.day, value))
				{
					this.FireChangedEvent();
				}
			}
		}
        /// <summary>
        /// Ordinal Unit definition
        /// </summary>
		public ScheduleOrdinal Ordinal
		{
			get
			{
				return (ScheduleOrdinal)this.ordinal;
			}
			set
			{
				if (value != (ScheduleOrdinal)Interlocked.Exchange(ref this.ordinal, value))
				{
					this.FireChangedEvent();
				}
			}		
		}
        /// <summary>
        /// Days Unit definition
        /// </summary>
		public ScheduleDay WeekDay 
		{
			get
			{
				return (ScheduleDay)this.weekday;
			}
			set
			{
				if (value != (ScheduleDay)Interlocked.Exchange(ref this.weekday, value))
				{
					this.FireChangedEvent();
				}
			}
		}
        /// <summary>
        /// Monthly Unit definition
        /// </summary>
		public ScheduleMonth ScheduledMonths 
		{
			get
			{
				return (ScheduleMonth)this.months;
			}
			set
			{
				if (value == ScheduleMonth.None)
				{
					throw (new ArgumentOutOfRangeException("months", "Must specify a month"));
				}
				if (value != (ScheduleMonth)Interlocked.Exchange(ref this.months, value))
				{
					this.FireChangedEvent();
				}
			}
		}
		// Methods
        /// <summary>
        /// Constructor
        /// </summary>
		public MonthSchedule()
		{
		}
        /// <summary>
        /// Constructor
        /// </summary>
		public MonthSchedule(string configxml)
		{
			XmlDocument configXml = new XmlDocument();
			configXml.LoadXml(configxml);
			base.type = ExtractScheduleType(configXml);
			if (base.type != ScheduleType.Monthly)
			{
				throw (new ApplicationException("Invalid Configuration Type"));
			}
			this.StartDate = ExtractDate(configXml, "/schedule/startdate", true);
			this.StartTime = ExtractTime(configXml, "/schedule/starttime", true);
			
			this.Day = IfExistsExtractInt(configXml, "/schedule/dayofmonth", 0);
			if (this.Day == 0)
			{
				this.Ordinal = ExtractScheduleOrdinal(configXml, "/schedule/ordinal", true);
				this.WeekDay = ExtractScheduleDay(configXml, "/schedule/weekday", true);
			}
			this.ScheduledMonths = ExtractScheduleMonth(configXml, "/schedule/months", true);	
		}
        /// <summary>
        /// Returns the next time the schedule will be triggerd
        /// </summary>
        /// <returns></returns>
		public override DateTime GetNextActivationTime()
		{
			if ((this.Day == 0) && ((this.Ordinal == ScheduleOrdinal.None) ||(this.WeekDay == ScheduleDay.None)))
			{
				throw(new ApplicationException("Uninitialized monthly schedule")); 
			}
			DateTime now = DateTime.Now;
			if (this.StartDate > now)
				now = new DateTime(this.StartDate.Year, this.StartDate.Month, this.StartDate.Day, 0,0,0);
			if (this.Day != 0)
			{
				if ((this.ScheduledMonths & GetScheduleMonthFlag(now)) > 0)
				{ // could be our lucky month
					if ((day <=  DateTime.DaysInMonth(now.Year, now.Month)))
					{
						if (((this.Day == now.Day) && (((StartTime.Hour == now.Hour) && (StartTime.Minute > now.Minute)) || (StartTime.Hour > now.Hour)))
										||(this.Day > now.Day))
						{
							return new DateTime(now.Year, now.Month, this.Day, StartTime.Hour, StartTime.Minute, 0);
						}
					}
				}
				for (int i = 1; i < 49; i++) //need to check for four years in case someone selects 29 February
				{
					now = now.AddMonths(1);
					if (((this.ScheduledMonths & GetScheduleMonthFlag(now)) > 0) && 
												(day <=  DateTime.DaysInMonth(now.Year, now.Month)))
						break;
				}
				return new DateTime(now.Year, now.Month, this.Day, StartTime.Hour, StartTime.Minute, 0);
			}
			//Ordinal day of week
			if ((this.ScheduledMonths & GetScheduleMonthFlag(now)) > 0)
			{
				int ordinalDay = GetOrdinalWeekDay(this.Ordinal, this.WeekDay, now.Month, now.Year);
				if (((ordinalDay == now.Day) && (((StartTime.Hour == now.Hour) && (StartTime.Minute > now.Minute)) || (StartTime.Hour > now.Hour))) 
									|| (ordinalDay > now.Day))
				{
					return new DateTime(now.Year, now.Month, ordinalDay, StartTime.Hour, StartTime.Minute, 0);
				}
			}
			for (int i = 1; i < 13; i++)
			{
				now = now.AddMonths(1);
				if ((this.ScheduledMonths & GetScheduleMonthFlag(now)) > 0)
				{
					int ordinalDay = GetOrdinalWeekDay(this.Ordinal, this.WeekDay, now.Month, now.Year);
					return new DateTime(now.Year, now.Month, ordinalDay, StartTime.Hour, StartTime.Minute, 0);
				}
			}
			return new DateTime(1900,1,1);
		}
	}
}
