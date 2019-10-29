using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace PreBuiltConfigExample.Logging.Loud
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builtConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(args)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(builtConfig["Logging:FilePath"])
                .CreateLogger();

            try
            {
                new HostBuilder()
                    .ConfigureAppConfiguration(config =>
                    {
                        config.AddConfiguration(builtConfig);
                    })
                    .ConfigureLogging(logging =>
                    {
                        logging.AddSerilog();
                    })
                    .ConfigureServices(_ =>
                    {
                        // Throw an example exception
                        // In reality this could be any exception from anywhere within Host.Build()
                        // Exception will be logged with Serilog
                        throw new ApplicationException("Oh no, it broke!");
                    })
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host blew up!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}