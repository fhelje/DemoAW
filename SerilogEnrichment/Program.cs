using System;
using System.Collections.Generic;
using System.Threading;
using Serilog;
using Serilog.Context;
using Serilog.Formatting.Compact;
using SerilogTimings.Extensions;

namespace SerilogEnrichment
{
    public class Program
    {
        private static ILogger Log = new LoggerConfiguration()
            .MinimumLevel.Debug()

            .WriteTo.LiterateConsole()
            //.WriteTo.Console(new CompactJsonFormatter())
            .WriteTo.File("log.txt")
            // Enrichers
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
            .Enrich.WithThreadId()
            .CreateLogger();
        static void Main(string[] args)
        {
            var log = Log.ForContext<Program>();
            
            // Create logger

            var calculations = new List<int>();
            log.Information("Calculating");
            for (var i = 0; i < 10; i++)
            {
                log.Debug("Entering loop {index}", i);
                try
                {                    
                    if (i % 6 == 0)
                    {
                        throw new Exception("Wrong index");
                    }
                    calculations.Add(i*i);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Calculation failed for index {index}", i);
                }
            }

            foreach (var calculation in calculations)
            {
                using (LogContext.PushProperty("Calculation", calculation))
                {
                    log.Debug("Checking data is not bigger than 10");
                    if (calculation > 10)
                    {
                        log.Error("Value is to large!");
                    }
                }
            }
            
            using (log.TimeOperation("Performing timing.", "Overlord"))
            {
                Thread.Sleep(142);
            }
        }
    }
}