using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.AzureWebJobsTraceWriter.UnitTests
{
	[ExcludeFromCodeCoverage]
	internal class LogEventHelper
	{
		public static LogEvent GetLogEvent()
		{
			return GetLogEvent(nameof(GetLogEvent));
		}

		public static LogEvent GetLogEvent(string message)
		{
			return GetLogEvent(message, LogEventLevel.Information);
		}

		public static LogEvent GetLogEvent(string message, LogEventLevel level)
		{
			MessageTemplate template = new MessageTemplateParser().Parse(message);

			return new LogEvent(DateTimeOffset.Now, level, null, template, Enumerable.Empty<LogEventProperty>());
		}
	}
}