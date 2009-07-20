using System;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Blogical.Shared.Adapters.Common.Schedules
{
    /// <summary>
    /// Daily Schedule class supporting  Microsoft.Biztalk.Scheduler.ISchedule interface.
    /// Allows scheduling by interval (e.g. every 3 days)  or by  weekday (e.g. on Mondays and Fridays)
    /// </summary>
    [Serializable()]
    public class TimeSchedule : Schedule
    {
        //Fields
		private int _interval = 0;					//polling interval
        private object _scheduleTime = 0;			//hours, minutes, seconds
		
        // Properties
        /// <summary>
        /// The number of units between polling request
        /// </summary>
        public int Interval 
		{
			get
			{
                return this._interval;
			}
			set
			{
                if (value != Interlocked.Exchange(ref this._interval, value))
				{
					this.FireChangedEvent();
				}
			}
		}
        /// <summary>
        /// Time Unit definition
        /// </summary>
        public ScheduleTimeType ScheduleTime 
		{
			get
			{
                return (ScheduleTimeType)this._scheduleTime;
			}
			set
			{
                if (value != (ScheduleTimeType)Interlocked.Exchange(ref this._scheduleTime, value))
				{
					this.FireChangedEvent();
				}
			}
		}
        long totalNumdebrOfSeconds
        {
            get 
            {
                switch (this.ScheduleTime)
                { 
                    case ScheduleTimeType.Hours:
                        return this._interval * 3600;
                    case ScheduleTimeType.Minutes:
                        return this._interval*60;
                    default:
                        return this._interval;
                }
            }
        }

		//Methods
        /// <summary>
        /// Constructor
        /// </summary>
		public TimeSchedule()
		{
		}
        /// <summary>
        /// Constructor
        /// </summary>
        public TimeSchedule(string configxml)
		{
			XmlDocument configXml = new XmlDocument();
			configXml.LoadXml(configxml);
			base.type = ExtractScheduleType(configXml);

			if (base.type != ScheduleType.Timely)
			{
				throw (new ApplicationException("Invalid Configuration Type"));
			}
			this.StartDate = ExtractDate(configXml, "/schedule/startdate", true);
			this.StartTime = ExtractTime(configXml, "/schedule/starttime", true);
			
			this._interval = IfExistsExtractInt(configXml, "/schedule/interval", 0);
            this.ScheduleTime = ExtractScheduleTimeType(configXml, "/schedule/timeintervalltype", true);

            //if (this.Interval == 0)
            //{
            //    this.ScheduleTime = ExtractScheduleTimeType(configXml, "/schedule/timeintervalltype", true);
            //}
		}
        /// <summary>
        /// Returns the next time the schedule will be triggerd
        /// </summary>
        /// <returns></returns>
		public override DateTime GetNextActivationTime()
		{
            TraceMessage("[TimeSchedule]Executing GetNextActivationTime");
            if (this.Interval == 0)
			{
				throw(new ApplicationException("Uninitialized timely schedule")); 
			}

            return DateTime.Now.AddSeconds(this.totalNumdebrOfSeconds);

		}
    }
}
