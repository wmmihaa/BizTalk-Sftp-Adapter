using System;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using Blogical.Shared.Adapters.Common.Schedules;

namespace  Blogical.Shared.Adapters.Common.Schedules
{
	/// <summary>
	/// Daily Schedule class supporting  Microsoft.Biztalk.Scheduler.ISchedule interface.
	/// Allows scheduling by interval (e.g. every 3 days)  or by  weekday (e.g. on Mondays and Fridays)
	/// </summary>
	[Serializable()]
	public class DaySchedule: Schedule
	{
		//Fields
		private int interval = 0;					//day interval
		private object days = 0;				//days of week
		
        // Properties
        /// <summary>
        /// The number of units between polling request
        /// </summary>
		public int Interval 
		{
			get
			{
				return this.interval;
			}
			set
			{
				if ((this.ScheduledDays == ScheduleDay.None) && (value <= 1))
				{
					throw (new ArgumentOutOfRangeException("days", "Must specify scheduled days or interval"));
				}
				if (value != Interlocked.Exchange(ref this.interval, value))
				{
					this.FireChangedEvent();
				}
			}
		}
        /// <summary>
        /// Days Unit definition
        /// </summary>
		public ScheduleDay ScheduledDays 
		{
			get
			{
				return (ScheduleDay)this.days;
			}
			set
			{
				if ((value == ScheduleDay.None) && (this.Interval <= 1))
				{
					throw (new ArgumentOutOfRangeException("days", "Must specify scheduled days or interval"));
				}
				if (value != (ScheduleDay)Interlocked.Exchange(ref this.days, value))
				{
					this.FireChangedEvent();
				}
			}
		}		
		//Methods
        /// <summary>
        /// Constructor
        /// </summary>
		public DaySchedule()
		{
		}
        /// <summary>
        /// Constructor
        /// </summary>
        public DaySchedule(string configxml)
		{
			XmlDocument configXml = new XmlDocument();
			configXml.LoadXml(configxml);
			base.type = ExtractScheduleType(configXml);
			if (base.type != ScheduleType.Daily)
			{
				throw (new ApplicationException("Invalid Configuration Type"));
			}
			this.StartDate = ExtractDate(configXml, "/schedule/startdate", true);
			this.StartTime = ExtractTime(configXml, "/schedule/starttime", true);
			
			this.interval = IfExistsExtractInt(configXml, "/schedule/interval", 0);
			if (this.Interval == 0)
			{
				this.ScheduledDays = ExtractScheduleDay(configXml, "/schedule/days", true);
			}
		}
        /// <summary>
        /// Returns the next time the schedule will be triggerd
        /// </summary>
        /// <returns></returns>
		public override DateTime GetNextActivationTime()
		{
            TraceMessage("[DaySchedule]Executing GetNextActivationTime");
			if ((this.Interval == 0) && (this.ScheduledDays == ScheduleDay.None))
			{
				throw(new ApplicationException("Uninitialized daily schedule")); 
			}
			DateTime now = DateTime.Now;
			if (this.StartDate > now)
			{
				now =  new DateTime(this.StartDate.Year, this.StartDate.Month, this.StartDate.Day, 0, 0,0);
				if (this.Interval > 1)
				{
					return now;
				}
			}
			//Interval Days
			if (interval > 0)
			{
				DateTime compare =  new DateTime(now.Year, now.Month, now.Day,0, 0, 0);
				TimeSpan diff = compare.Subtract(this.StartDate);
				int daysAhead = diff.Days % interval;
				int daysToGo = 0;
				if (daysAhead == 0)
				{
					if (((StartTime.Hour == now.Hour) && (StartTime.Minute > now.Minute)) || (StartTime.Hour > now.Hour))
					{
						return new DateTime(now.Year, now.Month, now.Day, StartTime.Hour, StartTime.Minute, 0);
					}
					daysToGo = interval;
				}
				else
				{
					daysToGo = interval - daysAhead;
				}
				DateTime returnDate = new DateTime(now.Year, now.Month, now.Day , StartTime.Hour, StartTime.Minute, 0);
				return returnDate.AddDays(daysToGo);
			}
			//Day of Week
			if ((GetScheduleDayFlag(now) & this.ScheduledDays) > 0)
			{ //today could be our lucky day
				if (((StartTime.Hour == now.Hour) && (StartTime.Minute > now.Minute)) || (StartTime.Hour > now.Hour))
				{
					return new DateTime(now.Year, now.Month, now.Day, StartTime.Hour, StartTime.Minute, 0);
				}
			}
			//Find next day
			for (int i = 1; i < 8; i++)
			{
				now = now.AddDays(1);
				if ((GetScheduleDayFlag(now) & this.ScheduledDays) > 0)
					break;
			}
			return new DateTime(now.Year, now.Month, now.Day, StartTime.Hour, StartTime.Minute, 0);
		}
	}
}
