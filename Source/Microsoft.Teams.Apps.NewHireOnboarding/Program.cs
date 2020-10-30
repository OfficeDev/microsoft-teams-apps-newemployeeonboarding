// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding
{
    using System;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Program class is responsible for holding the entrypoint of the program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entrypoint for the program.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Build the webhost for servicing HTTP requests.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns> The WebHostBuilder configured from the arguments with the composition root defined in <see cref="Startup" />.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddEnvironmentVariables();

                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    // Using dotnet secrets to store the settings during development
                    // https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.0&tabs=windows
                    config.AddUserSecrets<Startup>();
                }
            })
            .UseStartup<Startup>()
            .ConfigureLogging((hostingContext, logging) =>
            {
                // hostingContext.HostingEnvironment can be used to determine environments as well.
                var appInsightKey = hostingContext.Configuration["ApplicationInsights:InstrumentationKey"];
                logging.AddApplicationInsights(appInsightKey);

                // This will capture Info level traces and above.
                if (!Enum.TryParse(hostingContext.Configuration["ApplicationInsights:LogLevel:Default"], out LogLevel logLevel))
                {
                    logLevel = LogLevel.Information;
                }

                logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>(string.Empty, logLevel);
            });
    }
}
