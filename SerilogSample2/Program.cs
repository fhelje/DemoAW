using System;
using Serilog;
using Serilog.Formatting.Compact;

namespace SerilogSample2
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
                for (var i = 0; i < 10; i++)
                {
                    var user = new {Name = $"Name {i}", Id = i};
                    log.Debug("User {user} was used", user);
                    log.Debug("User {@user} was used", user);
                    
                }                
            }
        }
    }
}