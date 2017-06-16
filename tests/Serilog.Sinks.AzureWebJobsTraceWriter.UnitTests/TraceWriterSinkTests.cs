using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Azure.WebJobs.Host;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Formatting.Raw;
using Xunit;

namespace Serilog.Sinks.AzureWebJobsTraceWriter.UnitTests
{
	[ExcludeFromCodeCoverage]
	public class TraceWriterSinkTests
	{
		[Fact]
		public void Constructor_NullTraceWriter()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				ITextFormatter formatter = new RawFormatter();

				TraceWriterSink t = new TraceWriterSink(null, formatter);
			});
		}

		[Fact]
		public void Constructor_NullFormatter()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				TraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);

				TraceWriterSink t = new TraceWriterSink(traceWriter, null);
			});
		}

		[Fact]
		public void Constructor()
		{
			TraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);
			ITextFormatter formatter = new RawFormatter();

			TraceWriterSink t = new TraceWriterSink(traceWriter, formatter);
		}

		[Theory]
		[InlineData(LogEventLevel.Fatal, TraceLevel.Error)]
		[InlineData(LogEventLevel.Error, TraceLevel.Error)]
		[InlineData(LogEventLevel.Warning, TraceLevel.Warning)]
		[InlineData(LogEventLevel.Information, TraceLevel.Info)]
		[InlineData(LogEventLevel.Verbose, TraceLevel.Verbose)]
		[InlineData(LogEventLevel.Debug, TraceLevel.Verbose)]
		public void GetLogEventTraceLevel(LogEventLevel logEventLevel, TraceLevel expectedTraceLevel)
		{
			TraceLevel traceLevel = TraceWriterSink.GetLogEventTraceLevel(logEventLevel);

			Assert.Equal(expectedTraceLevel, traceLevel);
		}

		[Fact]
		public void FormatLogEventMessage()
		{
			string message = "PlainMessageTemplate";

			LogEvent logEvent = LogEventHelper.GetLogEvent(message);

			ITextFormatter formatter = new MessageTemplateTextFormatter("{Message}", null);

			string formattedMessage = TraceWriterSink.FormatLogEventMessage(logEvent, formatter);

			Assert.Equal(message, formattedMessage);
		}

		[Fact]
		public void FormatLogEventMessage_NullLogEvent()
		{
			ITextFormatter formatter = new RawFormatter();

			string formattedMessage = TraceWriterSink.FormatLogEventMessage(null, formatter);

			Assert.Null(formattedMessage);
		}

		[Fact]
		public void FormatLogEventMessage_NullFormatter()
		{
			LogEvent logEvent = LogEventHelper.GetLogEvent();

			string formattedMessage = TraceWriterSink.FormatLogEventMessage(logEvent, null);

			Assert.Null(formattedMessage);
		}

		[Fact]
		public void GetLogEventSourceProperty()
		{
			string expectedSource = "SomeSource";

			Dictionary<string, LogEventPropertyValue> propertyValues = new Dictionary<string, LogEventPropertyValue>
			{
				{ Constants.SourceContextPropertyName, new ScalarValue(expectedSource) }
			};

			string source = TraceWriterSink.GetLogEventSourceProperty(propertyValues);

			Assert.Equal(expectedSource, source);
		}

		[Fact]
		public void GetLogEventSourceProperty_NotInPropertyValues()
		{
			Dictionary<string, LogEventPropertyValue> propertyValues = new Dictionary<string, LogEventPropertyValue>();

			string source = TraceWriterSink.GetLogEventSourceProperty(propertyValues);

			Assert.Null(source);
		}

		[Fact]
		public void GetLogEventSourceProperty_NotAScalarValue()
		{
			Dictionary<string, LogEventPropertyValue> propertyValues = new Dictionary<string, LogEventPropertyValue>
			{
				{ Constants.SourceContextPropertyName, null }
			};

			string source = TraceWriterSink.GetLogEventSourceProperty(propertyValues);

			Assert.Null(source);
		}

		[Fact]
		public void GetLogEventSourceProperty_NullScalarValue()
		{
			Dictionary<string, LogEventPropertyValue> propertyValues = new Dictionary<string, LogEventPropertyValue>
			{
				{ Constants.SourceContextPropertyName, new ScalarValue(null) }
			};

			string source = TraceWriterSink.GetLogEventSourceProperty(propertyValues);

			Assert.Null(source);
		}

		[Fact]
		public void GetLogEventSourceProperty_NullLogEventProperties()
		{
			string source = TraceWriterSink.GetLogEventSourceProperty(null);

			Assert.Null(source);
		}

		[Fact]
		public void BuildTraceEvent_NullLogEvent()
		{
			ITextFormatter formatter = new RawFormatter();

			TraceEvent traceEvent = TraceWriterSink.BuildTraceEvent(null, formatter);

			Assert.Null(traceEvent);
		}

		[Fact]
		public void BuildTraceEvent_NullFormatter()
		{
			LogEvent logEvent = LogEventHelper.GetLogEvent();

			TraceEvent traceEvent = TraceWriterSink.BuildTraceEvent(logEvent, null);

			Assert.Null(traceEvent);
		}

		[Fact]
		public void BuildTraceEvent()
		{
			string message = "PlainMessageTemplate";

			LogEventLevel logEventLevel = LogEventLevel.Warning;
			TraceLevel expectedTraceLevel = TraceLevel.Warning;

			LogEvent logEvent = LogEventHelper.GetLogEvent(message, logEventLevel);

			ITextFormatter formatter = new MessageTemplateTextFormatter("{Message}", null);

			TraceEvent traceEvent = TraceWriterSink.BuildTraceEvent(logEvent, formatter);

			Assert.Equal(message, traceEvent.Message);
			Assert.Equal(expectedTraceLevel, traceEvent.Level);
		}

		[Fact]
		public void Emit_NullLogEvent()
		{
			InMemoryTraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);

			ITextFormatter formatter = new RawFormatter();

			TraceWriterSink sink = new TraceWriterSink(traceWriter, formatter);

			Assert.Equal(0, traceWriter.Events.Count);

			sink.Emit(null);

			Assert.Equal(0, traceWriter.Events.Count);
		}

		[Fact]
		public void Emit()
		{
			string message = "PlainMessageTemplate";

			LogEventLevel logEventLevel = LogEventLevel.Warning;
			TraceLevel expectedTraceLevel = TraceLevel.Warning;

			LogEvent logEvent = LogEventHelper.GetLogEvent(message, logEventLevel);

			ITextFormatter formatter = new MessageTemplateTextFormatter("{Message}", null);

			InMemoryTraceWriter traceWriter = new InMemoryTraceWriter(TraceLevel.Verbose);

			TraceWriterSink sink = new TraceWriterSink(traceWriter, formatter);

			Assert.Equal(0, traceWriter.Events.Count);

			sink.Emit(logEvent);

			Assert.Equal(1, traceWriter.Events.Count);

			TraceEvent traceEvent = traceWriter.Events.First();

			Assert.Equal(message, traceEvent.Message);
			Assert.Equal(expectedTraceLevel, traceEvent.Level);
		}
	}
}