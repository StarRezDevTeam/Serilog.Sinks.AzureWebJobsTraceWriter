using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Azure.WebJobs.Host;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.AzureWebJobsTraceWriter
{
	public class TraceWriterSink : ILogEventSink
	{
		private readonly TraceWriter m_traceWriter;
		private readonly ITextFormatter m_formatter;

		/// <summary>
		/// Builds a sink that can link to an Azure WebJob TraceWriter
		/// </summary>
		/// <param name="traceWriter">The trace writer to log to</param>
		/// <param name="formatter">The formatter to use on emit</param>
		public TraceWriterSink(TraceWriter traceWriter, ITextFormatter formatter)
		{
			if (traceWriter == null)
			{
				throw new ArgumentNullException(nameof(traceWriter));
			}

			if (formatter == null)
			{
				throw new ArgumentNullException(nameof(formatter));
			}

			m_traceWriter = traceWriter;
			m_formatter = formatter;
		}

		/// <summary>
		/// Emits an event to the underlying tracewriter
		/// </summary>
		/// <param name="logEvent">The log event to emit</param>
		public void Emit(LogEvent logEvent)
		{
			if (logEvent == null)
			{
				return;
			}

			TraceEvent traceEvent = BuildTraceEvent(logEvent, m_formatter);

			m_traceWriter.Trace(traceEvent);
		}

		internal static TraceEvent BuildTraceEvent(LogEvent logEvent, ITextFormatter formatter)
		{
			if (logEvent == null || formatter == null)
			{
				return null;
			}

			string message = FormatLogEventMessage(logEvent, formatter);

			TraceLevel traceLevel = GetLogEventTraceLevel(logEvent.Level);

			string source = GetLogEventSourceProperty(logEvent.Properties);

			return new TraceEvent(traceLevel, message, source, logEvent.Exception);
		}

		internal static string GetLogEventSourceProperty(IReadOnlyDictionary<string, LogEventPropertyValue> logEventProperties)
		{
			if (logEventProperties == null ||
				!logEventProperties.ContainsKey(Constants.SourceContextPropertyName))
			{
				return null;
			}

			ScalarValue sourceValue = logEventProperties[Constants.SourceContextPropertyName] as ScalarValue;

			return sourceValue?.Value?.ToString();
		}

		internal static TraceLevel GetLogEventTraceLevel(LogEventLevel logEventLevel)
		{
			if (logEventLevel == LogEventLevel.Fatal || logEventLevel == LogEventLevel.Error)
			{
				return TraceLevel.Error;
			}

			if (logEventLevel == LogEventLevel.Warning)
			{
				return TraceLevel.Warning;
			}

			if (logEventLevel == LogEventLevel.Information)
			{
				return TraceLevel.Info;
			}

			return TraceLevel.Verbose;
		}

		internal static string FormatLogEventMessage(LogEvent logEvent, ITextFormatter formatter)
		{
			if (logEvent == null || formatter == null)
			{
				return null;
			}

			using (StringWriter render = new StringWriter())
			{
				formatter.Format(logEvent, render);

				return render.ToString();
			}
		}
	}
}