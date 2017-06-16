# Serilog Azure WebJobs/Functions TraceWriter Sink

[![NuGet Version](https://img.shields.io/nuget/v/Serilog.Sinks.AzureWebJobsTraceWriter.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.AzureWebJobsTraceWriter/)
[![AppVeyor](https://img.shields.io/appveyor/ci/ScottHolden/serilog-sinks-azurewebjobstracewriter.svg)](https://ci.appveyor.com/project/ScottHolden/serilog-sinks-azurewebjobstracewriter)
[![Coverage Status](https://coveralls.io/repos/github/StarRez/Serilog.Sinks.AzureWebJobsTraceWriter/badge.svg?branch=master)](https://coveralls.io/github/StarRez/Serilog.Sinks.AzureWebJobsTraceWriter?branch=master)

A Serilog sink that writes events to Azure WebJob Host's TraceWriter. This is the logging mechanism used by both Azure WebJob's, and Azure Functions (which is built on top of the WebJob Host).

### Getting started

Install the [Serilog.Sinks.AzureWebJobsTraceWriter](https://nuget.org/packages/Serilog.Sinks.AzureWebJobsTraceWriter/) package from NuGet.

Within your logger configuration, you can now include a TraceWriter as a sink:

```csharp
using Serilog.Sinks.AzureWebJobsTraceWriter;

ILogger log = new LoggerConfiguration()
    .WriteTo.TraceWriter(traceWriter)
    .CreateLogger();
    
log.Warning("This will be written to the TraceWriter");
```

### Azure Functions Example

You will need to include the required Nuget packages within the functions `project.json`:

```json
{
    "frameworks": {
        "net46": {
            "dependencies": {
                "Serilog": "2.4.0",
                "Serilog.Sinks.AzureWebJobsTraceWriter": "1.0.0",
                "Microsoft.Azure.WebJobs": "2.1.0"
            }
        }
    }
}
```

Then you can create a new logger within the scope of your function's static run method:

```csharp
// This is required to point to the internal version of WebJobs.Host
#r "Microsoft.Azure.WebJobs.Host"

using System.Net;
using Serilog;
using Serilog.Sinks.AzureWebJobsTraceWriter;

public static string Run(HttpRequestMessage req, TraceWriter log)
{
    ILogger logger = new LoggerConfiguration()
                        .WriteTo.TraceWriter(log)
                        .CreateLogger();

    string someData = Guid.NewGuid().ToString();

    logger.Information("This is logging test for {someData}", someData);

    return $"Done with {someData}"; 
} 
```

### Helpful Links

* [Serilog Documentation](https://github.com/serilog/serilog/wiki)
* [Azure WebJobs SDK Documentation](https://github.com/Azure/azure-webjobs-sdk/wiki)