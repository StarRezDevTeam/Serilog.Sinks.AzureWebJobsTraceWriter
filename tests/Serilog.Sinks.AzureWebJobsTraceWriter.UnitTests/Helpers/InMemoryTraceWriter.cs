using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.WebJobs.Host;

namespace Serilog.Sinks.AzureWebJobsTraceWriter.UnitTests
{
	[ExcludeFromCodeCoverage]
	internal class InMemoryTraceWriter : TraceWriter
	{
		private readonly List<TraceEvent> m_events;

		public IReadOnlyCollection<TraceEvent> Events
		{
			get
			{
				return m_events.AsReadOnly();
			}
		}

		public InMemoryTraceWriter(TraceLevel level) : base(level)
		{
			m_events = new List<TraceEvent>();
		}

		public override void Trace(TraceEvent traceEvent)
		{
			m_events.Add(traceEvent);
		}
	}
}