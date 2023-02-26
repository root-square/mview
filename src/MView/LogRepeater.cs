using Caliburn.Micro;
using MView.ViewModels;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView
{
    public class LogRepeater : ILogEventSink
    {
        public delegate void LogEmittedEventHandler(string log);

        public event LogEmittedEventHandler? LogEmittedEvent;

        private readonly string _format = "[{0}] (Thread #{1}) {2}";

        public string LastLog = string.Empty;

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write</param>
        public void Emit(LogEvent logEvent)
        {
            string level = LevelToSeverity(logEvent);
            string threadId = logEvent.Properties["ThreadId"].ToString();
            string content = logEvent.RenderMessage();

            string log = string.Format(_format, level, threadId, content);
            LastLog = log;
            LogEmittedEvent?.Invoke(log);
        }

        static string LevelToSeverity(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Debug:
                    return "DBG";
                case LogEventLevel.Error:
                    return "ERR";
                case LogEventLevel.Fatal:
                    return "FTL";
                case LogEventLevel.Verbose:
                    return "VERBOSE";
                case LogEventLevel.Warning:
                    return "WARN";
                default:
                    return "INF";
            }
        }
    }
}
