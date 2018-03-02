using System;
using Serilog;
using Serilog.Formatting.Compact;

namespace SerilogSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            using (var log = new LoggerConfiguration()
                                    .MinimumLevel.Debug()
                                    //.WriteTo.LiterateConsole()
                                    .WriteTo.Console(new CompactJsonFormatter())
                                    .CreateLogger())
            {
                for (int i = 0; i < 10; i++)
                {
                    //log.Debug("Sample log elasped {0} in {1}", random.Next(0,100), "Main");
                    log.Debug("Sample log elasped {Elapsed} in {Method}", random.Next(0,100), "Main");
                    
                }                
            }
        }
    }
}