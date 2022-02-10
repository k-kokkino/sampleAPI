using System;

using Kkokkino.SampleAPI.Web.Helpers.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;

namespace SampleAPI.Web
{
  public class Program
  {
    internal static readonly LoggingLevelSwitch LevelSwitch = new LoggingLevelSwitch();

    private static Microsoft.Extensions.Logging.ILogger logger
      = new NullLogger<Startup>();

    public static int Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .CreateStartupLogger()
        .CreateBootstrapLogger();

      using (var loggerProvider = new SerilogLoggerProvider(Log.Logger))
      {
          logger = loggerProvider.CreateLogger(nameof(Startup));
      }

      try
      {
        var host = CreateHostBuilder(args).Build();
        var configuration = host.Services.GetRequiredService<IConfiguration>();
        var environment = host.Services.GetRequiredService<IWebHostEnvironment>();
        Log.Logger = new LoggerConfiguration()
            .CreateActualLogger(configuration, environment)
            .CreateLogger();

        logger = host.Services.GetRequiredService<ILogger<Startup>>();
        host.Run();
        return 0;
      }
      catch (Exception e)
      {
        logger.LogCritical(e, LogTemplates.UnhandledException, e.Message); // exception should be passed first in order to print stacktrace
        // throw; gia na gyrisei to e xwris allagi stacktrace
        return 1;
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            })
            .UseSerilog();
  }

  public static class LogTemplates
  {
      public const string UnhandledException = "Unhandled exception: {Message}";
  }
}
