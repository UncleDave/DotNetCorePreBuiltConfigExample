using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace PreBuiltConfigExample.Logging.Silent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new HostBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json");
                    config.AddCommandLine(args);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddSerilog(
                        new LoggerConfiguration()
                            .WriteTo.Console()
                            .WriteTo.File(context.Configuration["Logging:FilePath"])
                            .CreateLogger()
                    );
                })
                .ConfigureServices(_ =>
                {
                    // Throw an example exception
                    // In reality this could be any exception from anywhere within Host.Build()
                    // Exception will not be logged with Serilog, only to the Console
                    throw new ApplicationException("Oh no, it broke!");
                })
                .Build()
                .Run();
        }
    }
}