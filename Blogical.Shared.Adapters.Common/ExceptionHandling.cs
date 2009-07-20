using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.Message.Interop;
using System.Diagnostics;
using System.Security.Permissions;

namespace Blogical.Shared.Adapters.Common
{
    /// <summary>
    /// Contains functions to handle exceptions.
    /// </summary>
    public static class ExceptionHandling
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodId">String that identifies the method.</param>
        /// <param name="ex">The exception that occured.</param>
        /// <returns></returns>
        public static Exception HandleComponentException(string methodId, Exception ex)
        {
            CreateEventLogMessage(ex, methodId, null);
            return ex;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="eventID"></param>
        /// <param name="methodId"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception HandleComponentException(int eventID, string methodId, Exception ex)
        {
            CreateEventLogMessage(ex, methodId, null,eventID);
            return ex;
        }

        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodBase">Received by new StackFrame().GetMethod();
        /// <code>Sample: throw Blogical.Shared.Adapters.Common.ExceptionHandling.HandleComponentException(System.Reflection.MethodBase.GetCurrentMethod(), ex);</code>
        /// </param>
        /// <param name="ex"></param>
        /// <returns>The exception that occured.</returns>
        public static Exception HandleComponentException(System.Reflection.MethodBase methodBase, Exception ex)
        {
            string s = methodBase.DeclaringType.FullName + "." + methodBase.Name;
            return HandleComponentException(methodBase.DeclaringType.FullName + "." + methodBase.Name, ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="methodBase"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception HandleComponentException(int eventID, System.Reflection.MethodBase methodBase, Exception ex)
        {
            string s = methodBase.DeclaringType.FullName + "." + methodBase.Name;
            return HandleComponentException(eventID, methodBase.DeclaringType.FullName + "." + methodBase.Name, ex);
        }

        public static void CreateEventLogMessage(Exception ex, string methodId, string messageName)
        {
            CreateEventLogMessage(createLogMessage(ex, methodId, messageName), 0, 0, EventLogEntryType.Error);
        }

        public static void CreateEventLogMessage(Exception ex, string methodId, string messageName, int eventID)
        {
            CreateEventLogMessage(createLogMessage(ex, methodId, messageName), eventID, 0, EventLogEntryType.Error);
        }

        public static void CreateEventLogMessage(Exception ex, string methodId, string messageName, int eventID, EventLogEntryType entryType)
        {
            CreateEventLogMessage(createLogMessage(ex, methodId, messageName), eventID, 0, entryType);
        }

        [EventLogPermission(SecurityAction.Demand, PermissionAccess=EventLogPermissionAccess.Write)]
        public static void CreateEventLogMessage(string message,
            int eventID, short category, EventLogEntryType entryType)
        {
            EventLog eventLog = new EventLog();
            eventLog.Source = EventLogSources.SFTPAdapter;
            eventLog.WriteEntry(message, entryType, eventID, category);
        }

        /// <summary>
        /// Creates a message to write to event-log.
        /// </summary>
        /// <param name="ex">The Exception to log</param>
        /// <param name="methodId">Name of the method that handled this exception.</param>
        /// <param name="messageName">The name of the message that caused the exception. (null if not available)</param>
        /// <returns>A string to write to the event-log</returns>
        private static string createLogMessage(Exception ex, string methodId, string messageName) {
            
            StringBuilder message = new StringBuilder();
            // Add info about log-entry
            message.AppendFormat("Method: {0}\r\n", methodId);
            if (ex != null)
                message.AppendFormat("Error: {0}\r\n", ex.Message);
            if (messageName != null)
                message.AppendFormat("Message name: {0}\r\n", messageName);
            // Add info about exception
            message.Append("\r\n------------------------------\r\nInformation:\r\n");
            message.Append(exceptionMessage(ex));
            // Add info about inner exceptions
            Exception e = ex;
            while (e.InnerException != null)
            {
                message.Append("------------------------------\r\n");
                if (e.InnerException.GetType() == typeof(System.Data.SqlClient.SqlException))
                    message.Append(exceptionMessage((System.Data.SqlClient.SqlException)e.InnerException));
                else
                    message.Append(exceptionMessage(e.InnerException));
                message.Append("\r\n");
                e = e.InnerException;
            }
            // Return message
            return message.ToString();
        }

        /// <summary>
        /// Creates a message describing an Exception.
        /// </summary>
        /// <param name="ex">The exception to describe</param>
        /// <returns>A message describing an Exception.</returns>
        private static string exceptionMessage(Exception ex)
        {
            // Add exception info
            StringBuilder message = new StringBuilder();
            message.AppendFormat("Type: {0}\r\nTarget: {1}\r\nMessage: {2}\r\nStacktrace:\r\n{3}\r\n\r\n",
                                    ex.GetType().FullName,
                                    ex.TargetSite,
                                    ex.Message,
                                    ex.StackTrace);

            if (ex.Data.Count > 0)
            {
                // Add data info
                message.Append("Data:\r\n");
                foreach (System.Collections.DictionaryEntry item in ex.Data)
                {
                    message.AppendFormat("{0}:{1}\r\n", item.Key, item.Value);
                }
            }
            // Return message
            return message.ToString();
        }

    }
}
