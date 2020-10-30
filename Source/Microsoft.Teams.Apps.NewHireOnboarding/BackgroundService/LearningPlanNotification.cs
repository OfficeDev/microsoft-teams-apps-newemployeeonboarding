// <copyright file="LearningPlanNotification.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.BackgroundService
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Rest.Azure;
    using Microsoft.Teams.Apps.NewHireOnboarding.Cards;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;

    /// <summary>
    /// This class inherits IHostedService and implements the methods related to background tasks for sending learning plan notifications.
    /// </summary>
    public class LearningPlanNotification : BackgroundService
    {
        /// <summary>
        /// Default learning plan in weeks.
        /// </summary>
        private readonly int defaultLearningPlanInWeek;

        /// <summary>
        /// Instance to send logs to the Application Insights service.
        /// </summary>
        private readonly ILogger<LearningPlanNotification> logger;

        /// <summary>
        /// Provider for fetching information about user details from storage.
        /// </summary>
        private readonly IUserStorageProvider userStorageProvider;

        /// <summary>
        /// Instance of learning helper to get learning plan methods.
        /// </summary>
        private readonly ILearningPlanHelper learningPlanHelper;

        /// <summary>
        /// A set of key/value application configuration properties for SharePoint.
        /// </summary>
        private readonly IOptions<SharePointSettings> sharePointOptions;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// A set of key/value application configuration properties for bot settings.
        /// </summary>
        private readonly IOptions<BotOptions> botOptions;

        /// <summary>
        /// Helper for working with bot notification card.
        /// </summary>
        private readonly INotificationCardHelper notificationCardHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningPlanNotification"/> class.
        /// BackgroundService class that inherits IHostedService and implements the methods related to sending notification tasks.
        /// </summary>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        /// <param name="userStorageProvider">Provider for fetching information about user details from storage.</param>
        /// <param name="learningPlanHelper">Instance of learning plan helper.</param>
        /// <param name="sharePointOptions">A set of key/value application configuration properties for SharePoint.</param>
        /// <param name="notificationCardHelper">Helper for working with bot notification card.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="botOptions">A set of key/value application configuration properties.</param>
        public LearningPlanNotification(
            ILogger<LearningPlanNotification> logger,
            IUserStorageProvider userStorageProvider,
            ILearningPlanHelper learningPlanHelper,
            IOptions<SharePointSettings> sharePointOptions,
            INotificationCardHelper notificationCardHelper,
            IStringLocalizer<Strings> localizer,
            IOptions<BotOptions> botOptions)
        {
            this.logger = logger;
            this.sharePointOptions = sharePointOptions ?? throw new ArgumentNullException(nameof(sharePointOptions));
            this.userStorageProvider = userStorageProvider;
            this.learningPlanHelper = learningPlanHelper;
            this.sharePointOptions = sharePointOptions;
            this.notificationCardHelper = notificationCardHelper;
            this.defaultLearningPlanInWeek = sharePointOptions.Value.NewHireLearningPlansInWeeks > 0 ? sharePointOptions.Value.NewHireLearningPlansInWeeks : 4;
            this.localizer = localizer;
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
        }

        /// <summary>
        /// Send learning plan notification card to new hire on weekly basis.
        /// </summary>
        /// <returns>A task that represents whether weekly notification sent successfully or not.</returns>
        public async Task<bool> SendWeeklyNotificationAsync()
        {
            try
            {
                var completeLearningPlan = await this.learningPlanHelper.GetCompleteLearningPlansAsync();

                if (completeLearningPlan == null || !completeLearningPlan.Any())
                {
                    this.logger.LogError("Learning plan not available.");

                    return false;
                }

                var newHires = await this.userStorageProvider.GetAllUsersAsync((int)UserRole.NewHire);

                if (newHires == null)
                {
                    this.logger.LogError("New hires not available.");

                    return false;
                }

                var batchStartDate = DateTime.UtcNow;
                var learningPlansInDays = 0;

                for (int i = 1; i <= this.defaultLearningPlanInWeek; i++)
                {
                    try
                    {
                        // To calculate weekly users list to send learning plan notification.
                        var users = newHires.Where(user => (batchStartDate - user.BotInstalledOn)?.Days > learningPlansInDays && (batchStartDate - user.BotInstalledOn)?.Days <= learningPlansInDays + 7).ToList();

                        var learningWeek = $"{BotCommandConstants.LearningPlanWeek} {i}";
                        var listCardAttachment = LearningPlanListCard.GetLearningPlanListCard(
                        completeLearningPlan.Where(learningPlan => learningPlan.CompleteBy.ToUpperInvariant() == learningWeek.ToUpperInvariant()),
                        this.localizer,
                        this.localizer.GetString("LearningPlanWeekListCardTitleText", learningWeek),
                        this.botOptions.Value.ManifestId,
                        this.botOptions.Value.AppBaseUri);

                        if (!users.Any() || listCardAttachment == null)
                        {
                            this.logger.LogWarning($"Learning plan notification card is not available for week {i}.");
                            continue;
                        }

                        // Send weekly learning plan notification to new hire employees.
                        foreach (var userDetail in users)
                        {
                            try
                            {
                                await this.notificationCardHelper.SendProActiveNotificationCardAsync(listCardAttachment, userDetail.ConversationId, userDetail.ServiceUrl);
                            }
#pragma warning disable CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                            catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                            {
                                this.logger.LogError(ex, $"Error occurred while sending learning plan for user {userDetail.AadObjectId}: {ex.Message} at: {DateTime.UtcNow}");
                            }
                        }
                    }
#pragma warning disable CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                    catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                    {
                        this.logger.LogError(ex, $"Error occurred while sending learning plan for week {i}: {ex.Message} at: {DateTime.UtcNow}");
                    }
                    finally
                    {
                        learningPlansInDays += 7;
                        batchStartDate.AddDays(7);
                    }
                }

                return true;
            }
            catch (CloudException ex)
            {
                this.logger.LogError(ex, $"Error occurred while accessing user details from storage: {ex.Message} at: {DateTime.UtcNow}");

                return false;
            }
        }

        /// <summary>
        ///  This method is called when the Microsoft.Extensions.Hosting.IHostedService starts.
        ///  The implementation should return a task that represents the lifetime of the long
        ///  running operation(s) being performed.
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
                    this.logger.LogInformation($"Learning plan notification Hosted Service is running at: {currentDateTime}.");

                    if (currentDateTime.DayOfWeek == DayOfWeek.Monday)
                    {
                        await this.SendWeeklyNotificationAsync();
                        this.logger.LogInformation($"Monday of the week: {currentDateTime} and learning plan notification sent.");
                    }
                }
#pragma warning disable CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                {
                    this.logger.LogError(ex, "Error occurred while running learning plan notification service.");
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                    this.logger.LogInformation($"Learning plan notification service execution completed and will resume after {TimeSpan.FromDays(1)} delay.");
                }
            }
        }
    }
}
