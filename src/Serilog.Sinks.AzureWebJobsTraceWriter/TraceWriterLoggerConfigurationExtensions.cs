using System;
using Microsoft.Azure.WebJobs.Host;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.AzureWebJobsTraceWriter.Sinks;

namespace Serilog.Sinks.AzureWebJobsTraceWriter
{
    public static class TraceWriterLoggerConfigurationExtensions
	{
		private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

		public static LoggerConfiguration TraceWriter(
			this LoggerSinkConfiguration loggerConfiguration,
			TraceWriter traceWriter,
			string outputTemplate = DefaultOutputTemplate,
			IFormatProvider formatProvider = null,
			LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
		{
			if (loggerConfiguration == null)
			{
				throw new ArgumentNullException(nameof(loggerConfiguration));
			}

			if (traceWriter == null)
			{
				throw new ArgumentNullException(nameof(traceWriter));
			}

			if (outputTemplate == null)
			{
				throw new ArgumentNullException(nameof(outputTemplate));
			}

			ITextFormatter formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);

			return TraceWriter(loggerConfiguration, traceWriter, formatter, restrictedToMinimumLevel);
		}

		public static LoggerConfiguration TraceWriter(
			this LoggerSinkConfiguration loggerConfiguration,
			TraceWriter traceWriter,
			ITextFormatter formatter,
			LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
		{
			if (loggerConfiguration == null)
			{
				throw new ArgumentNullException(nameof(loggerConfiguration));
			}

			if (traceWriter == null)
			{
				throw new ArgumentNullException(nameof(traceWriter));
			}

			if (formatter == null)
			{
				throw new ArgumentNullException(nameof(formatter));
			}

			TraceWriterSink sink = new TraceWriterSink(traceWriter, formatter);

			return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
		}
	}
}