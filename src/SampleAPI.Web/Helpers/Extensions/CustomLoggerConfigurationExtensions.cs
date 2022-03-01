using System;
using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using SampleAPI.Web;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Kkokkino.SampleAPI.Web.Helpers.Extensions
{
  public static class CustomLoggerConfigurationExtensions
  {
    public static LoggerConfiguration CreateStartupLogger(this LoggerConfiguration log)
      => log
        .MinimumLevel.Verbose()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .MinimumLevel.Override("System", LogEventLevel.Error)
        .Enrich.FromLogContext()
        .WriteTo.Debug()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code, standardErrorFromLevel: LogEventLevel.Verbose);

    public static LoggerConfiguration CreateActualLogger(this LoggerConfiguration log, IConfiguration configuration,
      IWebHostEnvironment environment)
      => log
        .CreateStartupLogger()
        .Enrich.WithProperty("Application", environment.ApplicationName)
        .Enrich.WithProperty("Environment", environment.EnvironmentName)
        .WriteTo.Logger(log => log
          .Filter.ByExcluding(
            "SourceContext='Microsoft.Hosting.Lifetime' or SourceContext='Microsoft.EntityFrameworkCore.Database.Command' or SourceContext='Serilog.AspNetCore.RequestLoggingMiddleware' or SourceContext='IdentityServer4.Hosting.IdentityServerMiddleware'")
          .MinimumLevel.ControlledBy(Program.LevelSwitch)
          .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "Logs", $"{environment.ApplicationName}-.log"),
            fileSizeLimitBytes: 31_457_280, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10,
            shared: true, flushToDiskInterval: TimeSpan.FromSeconds(5)));
    //.WriteTo.Seq("https://logs.kritikos.io", apiKey: configuration["Seq:ApiKey"], controlLevelSwitch: Program.LevelSwitch));
  }
}
