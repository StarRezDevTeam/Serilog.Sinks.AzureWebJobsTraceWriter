using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Azure.WebJobs.Host;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Display;
using Serilog.Formatting.Raw;
using Xunit;

namespace Serilog.Sinks.AzureWebJobsTraceWriter.UnitTests
{
	[ExcludeFromCodeCoverage]
	public class TraceWriterLoggerConfigurationExtensionsTests
	{
		[Fact]
		public void TraceWriter_ITextFormatter_NullLoggerConfiguration()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				InMemoryTraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);

				ITextFormatter formatter = new CompactJsonFormatter();

				TraceWriterLoggerConfigurationExtensions.TraceWriter(null, traceWriter, formatter);
			});
		}

		[Fact]
		public void TraceWriter_ITextFormatter_NullTraceWriter()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				LoggerConfiguration loggerConfiguration = new LoggerConfiguration();

				ITextFormatter formatter = new CompactJsonFormatter();

				TraceWriterLoggerConfigurationExtensions.TraceWriter(loggerConfiguration.WriteTo, null, formatter);
			});
		}

		[Fact]
		public void TraceWriter_ITextFormatter_NullFormatter()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				LoggerConfiguration loggerConfiguration = new LoggerConfiguration();

				InMemoryTraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);

				ITextFormatter formatter = null;

				TraceWriterLoggerConfigurationExtensions.TraceWriter(loggerConfiguration.WriteTo, traceWriter, formatter);
			});
		}

		[Fact]
		public void TraceWriter_IFormatProvider_NullLoggerConfiguration()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				InMemoryTraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);

				TraceWriterLoggerConfigurationExtensions.TraceWriter(null, traceWriter);
			});
		}

		[Fact]
		public void TraceWriter_IFormatProvider_NullTraceWriter()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				LoggerConfiguration loggerConfiguration = new LoggerConfiguration();

				TraceWriterLoggerConfigurationExtensions.TraceWriter(loggerConfiguration.WriteTo, null);
			});
		}

		[Fact]
		public void TraceWriter_IFormatProvider_NullOutputTemplate()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				LoggerConfiguration loggerConfiguration = new LoggerConfiguration();

				InMemoryTraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);

				string outputTemplate = null;

				TraceWriterLoggerConfigurationExtensions.TraceWriter(loggerConfiguration.WriteTo, traceWriter, outputTemplate);
			});
		}

		[Theory, MemberData(nameof(TraceWriter_TestData))]
		public void TraceWriter_ITextFormatter_NonExtensionCallPattern(Func<ILogger, string, TraceLevel> loggerAction)
		{
			string message = "Hello, World!";

			// Need to explicity define lowest level.
			LoggerConfiguration loggerConfiguration = new LoggerConfiguration().MinimumLevel.Verbose();

			InMemoryTraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);

			ITextFormatter formatter = new MessageTemplateTextFormatter("{Message}", null);

			loggerConfiguration = TraceWriterLoggerConfigurationExtensions.TraceWriter(loggerConfiguration.WriteTo, traceWriter, formatter, LogEventLevel.Verbose);

			ILogger logger = loggerConfiguration.CreateLogger();

			Assert.Equal(0, traceWriter.Events.Count);

			TraceLevel expectedTraceLevel = loggerAction(logger, message);

			Assert.Equal(1, traceWriter.Events.Count);

			TraceEvent traceEvent = traceWriter.Events.First();

			Assert.Equal(message, traceEvent.Message);
			Assert.Equal(expectedTraceLevel, traceEvent.Level);
		}

		[Theory, MemberData(nameof(TraceWriter_TestData))]
		public void TraceWriter_ITextFormatter(Func<ILogger, string, TraceLevel> loggerAction)
		{
			string message = "Hello, World!";

			// Need to explicity define lowest level.
			LoggerConfiguration loggerConfiguration = new LoggerConfiguration().MinimumLevel.Verbose();

			InMemoryTraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);

			ITextFormatter formatter = new MessageTemplateTextFormatter("{Message}", null);

			loggerConfiguration = loggerConfiguration.WriteTo.TraceWriter(traceWriter, formatter);

			ILogger logger = loggerConfiguration.CreateLogger();

			Assert.Equal(0, traceWriter.Events.Count);

			TraceLevel expectedTraceLevel = loggerAction(logger, message);

			Assert.Equal(1, traceWriter.Events.Count);

			TraceEvent traceEvent = traceWriter.Events.First();

			Assert.Equal(message, traceEvent.Message);
			Assert.Equal(expectedTraceLevel, traceEvent.Level);
		}

		[Theory, MemberData(nameof(TraceWriter_TestData))]
		public void TraceWriter_IFormatProvider(Func<ILogger, string, TraceLevel> loggerAction)
		{
			string message = "Hello, World!";

			// Need to explicity define lowest level.
			LoggerConfiguration loggerConfiguration = new LoggerConfiguration().MinimumLevel.Verbose();

			InMemoryTraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);

			loggerConfiguration = loggerConfiguration.WriteTo.TraceWriter(traceWriter, "{Message}");

			ILogger logger = loggerConfiguration.CreateLogger();

			Assert.Equal(0, traceWriter.Events.Count);

			TraceLevel expectedTraceLevel = loggerAction(logger, message);

			Assert.Equal(1, traceWriter.Events.Count);

			TraceEvent traceEvent = traceWriter.Events.First();

			Assert.Equal(message, traceEvent.Message);
			Assert.Equal(expectedTraceLevel, traceEvent.Level);
		}

		public static IEnumerable<object[]> TraceWriter_TestData
		{
			get
			{
				return new Func<ILogger, string, TraceLevel>[]
				{
					(logger, message) => { logger.Fatal(message); return TraceLevel.Error; },
					(logger, message) => { logger.Error(message); return TraceLevel.Error; },
					(logger, message) => { logger.Warning(message); return TraceLevel.Warning; },
					(logger, message) => { logger.Information(message); return TraceLevel.Info; },
					(logger, message) => { logger.Verbose(message); return TraceLevel.Verbose; },
					(logger, message) => { logger.Debug(message); return TraceLevel.Verbose; }
				}.Select(x => new object[] { x });
			}
		}
	}
}