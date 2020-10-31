// <copyright file="SurveyNotificationBackgroundService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.BackgroundService
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;

    /// <summary>
    /// This class inherits IHostedService and implements the methods related to background tasks for sending survey notifications.
    /// </summary>
    public class SurveyNotificationBackgroundService : BackgroundService
    {
        /// <summary>
        /// Instance to send logs to the Application Insights service.
        /// </summary>
        private readonly ILogger<SurveyNotificationBackgroundService> logger;

        /// <summary>
        /// Instance of survey notification helper which helps in sending survey notifications.
        /// </summary>
        private readonly INotificationHelper surveyNotificationHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyNotificationBackgroundService"/> class.
        /// BackgroundService class that inherits IHostedService and implements the methods related to sending survey notification tasks.
        /// </summary>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        /// <param name="notificationHelper">Helper to send survey notification in channels.</param>
        public SurveyNotificationBackgroundService(
        ILogger<SurveyNotificationBackgroundService> logger,
        INotificationHelper notificationHelper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.surveyNotificationHelper = notificationHelper;
        }

        /// <summary>
        /// This method is called when the Microsoft.Extensions.Hosting.IHostedService starts.
        /// The implementation should return a task that represents the lifetime of the long
        /// running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken) is called.</param>
        /// <returns>A System.Threading.Tasks.Task that represents the long running operations.</returns>
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var currentDateTime = DateTime.UtcNow;
                    this.logger.LogInformation($"Survey notification Hosted Service is running at: {currentDateTime}.");

                    // To send survey notification on every Monday to new hire.
                    if (currentDateTime.DayOfWeek == DayOfWeek.Monday)
                    {
                        this.logger.LogInformation($"Monday of the week: {currentDateTime} and sending the survey notification to new hire.");
                        await this.surveyNotificationHelper.SendSurveyNotificationToNewHireAsync();
                    }

                    // To send feedback notification to hiring manager on every first day of month.
                    if (currentDateTime.Day == 1)
                    {
                        this.logger.LogInformation($"First day of the month: {currentDateTime} and sending the feedback notification to hiring manager.");
                        await this.surveyNotificationHelper.SendFeedbackNotificationInChannelAsync();
                    }
                }
#pragma warning disable CA1031 // Catching general exception for any errors occurred during background service execution.
                catch (Exception ex)
#pragma warning restore CA1031 // Catching general exception for any errors occurred during background service execution.
                {
                    this.logger.LogError(ex, $"Error while running the background service to send notification): {ex.Message} at: {DateTime.UtcNow}", SeverityLevel.Error);
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                    this.logger.LogInformation($"Survey notification service execution completed and will resume after {TimeSpan.FromDays(1)} delay.");
                }
            }
        }
    }
}
