// <copyright file="NotificationHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Cards;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;

    /// <summary>
    /// Implements the methods that are defined in <see cref="INotificationHelper"/>.
    /// A class that contains helper methods for sending survey notifications.
    /// </summary>
    public class NotificationHelper : INotificationHelper
    {
        /// <summary>
        /// Sets the batch size of different new hire users.
        /// </summary>
        private const int SendSurveyNotificationBatchLimit = 5;

        /// <summary>
        /// Provider for fetching information about new hire introduction details from storage.
        /// </summary>
        private readonly IIntroductionStorageProvider introductionStorageProvider;

        /// <summary>
        /// Provider for fetching information about user details from storage.
        /// </summary>
        private readonly IUserStorageProvider userStorageProvider;

        /// <summary>
        /// A set of key/value application configuration properties for bot settings.
        /// </summary>
        private readonly IOptions<BotOptions> botOptions;

        /// <summary>
        /// Instance to send logs to the logger service.
        /// </summary>
        private readonly ILogger<NotificationHelper> logger;

        /// <summary>
        /// The current cultures' string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// A set of key/value application configuration properties of SharePoint.
        /// </summary>
        private readonly IOptions<SharePointSettings> sharePointOptions;

        /// <summary>
        /// Provider for fetching information about team details from storage.
        /// </summary>
        private readonly ITeamStorageProvider teamStorageProvider;

        /// <summary>
        /// Helper for working with bot notification card.
        /// </summary>
        private readonly INotificationCardHelper notificationCardHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHelper"/> class.
        /// </summary>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        /// <param name="localizer">The current cultures' string localizer.</param>
        /// <param name="botOptions">A set of key/value application configuration properties for bot.</param>
        /// <param name="storageProvider">Storage provider for Introduction storage.</param>
        /// <param name="userStorageProvider">User Storage provider for Introduction storage.</param>
        /// <param name="sharePointOptions">A set of key/value pair configuration properties for SharePoint.</param>
        /// <param name="teamStorageProvider">Provider for fetching information about team details from storage.</param>
        /// <param name="notificationCardHelper">Helper for working with bot notification card.</param>
        public NotificationHelper(
            IIntroductionStorageProvider storageProvider,
            IUserStorageProvider userStorageProvider,
            ILogger<NotificationHelper> logger,
            IStringLocalizer<Strings> localizer,
            IOptions<BotOptions> botOptions,
            IOptions<SharePointSettings> sharePointOptions,
            ITeamStorageProvider teamStorageProvider,
            INotificationCardHelper notificationCardHelper)
        {
            this.introductionStorageProvider = storageProvider;
            this.userStorageProvider = userStorageProvider;
            this.logger = logger;
            this.localizer = localizer;
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.sharePointOptions = sharePointOptions ?? throw new ArgumentNullException(nameof(sharePointOptions));
            this.teamStorageProvider = teamStorageProvider;
            this.notificationCardHelper = notificationCardHelper;
        }

        /// <summary>
        /// Send survey notification to new hire on Weekly basis in a batch.
        /// </summary>
        /// <returns>A task that represents that sends notification to new hire sends successfully.</returns>
        public async Task<bool> SendSurveyNotificationToNewHireAsync()
        {
            this.logger.LogInformation($"Send notification Timer trigger function executed at: {DateTime.UtcNow}");
            var introductionEntities = await this.introductionStorageProvider.GetAllPendingSurveyIntroductionAsync();

            if (introductionEntities == null || !introductionEntities.Any())
            {
                this.logger.LogWarning("No introduction is found to send survey notification.");

                return false;
            }

            var notificationCard = NotificationSurveyCard.GetSurveyNotificationCard(
                               this.botOptions.Value.AppBaseUri,
                               this.localizer,
                               this.sharePointOptions.Value.ShareFeedbackFormUrl);

            var batchCount = (int)Math.Ceiling((double)introductionEntities.Count() / SendSurveyNotificationBatchLimit);
            for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
            {
                var introductionEntitiesBatch = introductionEntities
                    .Skip(batchIndex * SendSurveyNotificationBatchLimit)
                    .Take(SendSurveyNotificationBatchLimit);

                foreach (var introductionEntity in introductionEntitiesBatch)
                {
                    try
                    {
                        var userConversationDetails = await this.userStorageProvider.GetUserDetailAsync(introductionEntity.NewHireAadObjectId);
                        await this.notificationCardHelper.SendProActiveNotificationCardAsync(notificationCard, userConversationDetails.ConversationId, userConversationDetails.ServiceUrl);
                        introductionEntity.SurveyNotificationSentStatus = (int)SurveyNotificationStatus.Sent;
                        introductionEntity.SurveyNotificationSentOn = DateTime.UtcNow;
                        await this.introductionStorageProvider.StoreOrUpdateIntroductionDetailAsync(introductionEntity);
                    }
#pragma warning disable CA1031 // Catching general exception for any errors occurred during send survey notification card to user.
                    catch (Exception ex)
#pragma warning disable CA1031 // Catching general exception for any errors occurred during send survey notification card to user.
                    {
                        this.logger.LogError(ex, $"Error while performing retry logic to send survey notification to user: {introductionEntity.NewHireAadObjectId}.");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Send feedback notification to hiring manager in team on Monthly basis as per the configuration.
        /// </summary>
        /// <returns>A task that represents whether feedback notification to hiring manager sends successfully.</returns>
        public async Task<bool> SendFeedbackNotificationInChannelAsync()
        {
            this.logger.LogInformation($"Send notification Timer trigger function executed at: {DateTime.UtcNow}");
            string teamsChannelId = this.botOptions.Value.HumanResourceTeamId;
            var teamEntity = await this.teamStorageProvider.GetTeamDetailAsync(teamsChannelId);

            if (teamEntity == null)
            {
                this.logger.LogWarning("No team is found to send feedback notification");

                return false;
            }
            else
            {
                var notificationCard = ViewFeedbackCard.GetFeedbackCard(this.botOptions.Value.AppBaseUri, this.localizer);

                try
                {
                    await this.notificationCardHelper.SendProActiveNotificationCardAsync(notificationCard, teamEntity.TeamId, teamEntity.ServiceUrl);

                    return true;
                }
#pragma warning disable CA1031 // Catching general exception for any errors occurred during send feedback notification card to user.
                catch (Exception ex)
#pragma warning disable CA1031 // Catching general exception for any errors occurred during send feedback notification card to user.
                {
                    this.logger.LogError(ex, $"Error while performing retry logic to send feedback notification to team: {teamEntity.TeamId}.");

                    return false;
                }
            }
        }
    }
}
