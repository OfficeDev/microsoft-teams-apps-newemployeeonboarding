// <copyright file="LearningPlanHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Cards;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;

    /// <summary>
    /// Implements the methods that are defined in <see cref="ILearningPlanHelper"/>.
    /// </summary>
    public class LearningPlanHelper : ILearningPlanHelper
    {
        /// <summary>
        /// Instance to log details in application insights.
        /// </summary>
        private readonly ILogger<LearningPlanHelper> logger;

        /// <summary>
        /// A set of key/value application configuration properties for bot settings.
        /// </summary>
        private readonly IOptions<BotOptions> botOptions;

        /// <summary>
        /// Instance to work with Microsoft Graph methods.
        /// </summary>
        private readonly IGraphUtilityHelper graphUtility;

        /// <summary>
        /// Instance to get the SharePoint utility methods.
        /// </summary>
        private readonly ISharePointHelper sharePointHelper;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// A set of key/value application configuration properties of SharePoint.
        /// </summary>
        private readonly IOptions<SharePointSettings> sharePointOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningPlanHelper"/> class.
        /// </summary>
        /// <param name="logger">Instance of ILogger</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="botOptions">A set of key/value application configuration properties.</param>
        /// <param name="sharePointOptions">A set of key/value application configuration properties for SharePoint.</param>
        /// <param name="graphUtility">Instance of Microsoft Graph utility helper.</param>
        /// <param name="sharePointHelper">Instance of SharePoint utility helper.</param>
        public LearningPlanHelper(
            ILogger<LearningPlanHelper> logger,
            IStringLocalizer<Strings> localizer,
            IOptions<BotOptions> botOptions,
            IOptions<SharePointSettings> sharePointOptions,
            IGraphUtilityHelper graphUtility,
            ISharePointHelper sharePointHelper)
        {
            this.logger = logger;
            this.localizer = localizer;
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.sharePointOptions = sharePointOptions ?? throw new ArgumentNullException(nameof(sharePointOptions));
            this.graphUtility = graphUtility;
            this.sharePointHelper = sharePointHelper;
        }

        /// <summary>
        /// Get complete learning plans details for new hire from SharePoint using Microsoft Graph.
        /// </summary>
        /// <returns>Complete learning plans details.</returns>
        public async Task<IEnumerable<LearningPlanListItemField>> GetCompleteLearningPlansAsync()
        {
            this.logger.LogInformation("Get complete learning plans initiated.");

            // Get Microsoft Graph token response.
            var response = await this.graphUtility.ObtainApplicationTokenAsync(
              this.botOptions.Value.TenantId,
              this.botOptions.Value.MicrosoftAppId,
              this.botOptions.Value.MicrosoftAppPassword);

            var result = await this.sharePointHelper.GetCompleteLearningPlanDataAsync(response.AccessToken);

            if (result == null)
            {
                this.logger.LogInformation("Get complete learning plans failed.");

                return null;
            }

            this.logger.LogInformation("Get complete learning plans succeed.");

            return result;
        }

        /// <summary>
        /// Get learning plan card for selected week and item of the list card.
        /// </summary>
        /// <param name="learningPlan">Learning plan item value.</param>
        /// <returns>Learning plan card as attachment.</returns>
        public async Task<Attachment> GetLearningPlanCardAsync(string learningPlan)
        {
            learningPlan = learningPlan ?? throw new ArgumentNullException(nameof(learningPlan));

            // Learning plan list card we are explicitly added ‘=>’ to split learning plan and learning content before sending the message back to Bot,
            // here we are checking length of the message is coming from tap event.
            if (learningPlan.Split("=>").Length != 3)
            {
                return null;
            }

            var learningWeek = learningPlan.Split("=>")[0]?.Trim();
            var plan = learningPlan.Split("=>")[1]?.Trim();
            var taskName = learningPlan.Split("=>")[2]?.Trim();

            // Get learning plan data for selected learning content.
            var learningPlans = await this.GetCompleteLearningPlansAsync();

            if (learningPlans == null)
            {
                this.logger.LogInformation("Complete learning plans not available.");

                return null;
            }

            // filtering out selected learning plan content from list card items.
            var selectedWeekLearningPlan = learningPlans.Where(learningContent => learningContent.CompleteBy.ToUpperInvariant() == learningWeek.ToUpperInvariant());

            if (selectedWeekLearningPlan == null)
            {
                this.logger.LogInformation("Selected week learning plan is not available.");

                return null;
            }

            var weeklyLearningPlan = selectedWeekLearningPlan.Where(
                listItem => listItem.Topic.ToUpperInvariant() == plan.ToUpperInvariant()
                && listItem.TaskName.Contains(
                    taskName, StringComparison.InvariantCultureIgnoreCase))?.FirstOrDefault();

            if (weeklyLearningPlan == null)
            {
                this.logger.LogInformation("Learning plan content data not available.");

                return null;
            }

            // Create learning plan data card.
            var learningCard = LearningPlanCard.GetNewHireLearningCard(
                this.localizer,
                this.botOptions.Value.AppBaseUri,
                weeklyLearningPlan);

            return learningCard;
        }

        /// <summary>
        /// Send learning plan list card for selected week.
        /// </summary>
        /// <param name="turnContext">Complete learning plan data.</param>
        /// <param name="userBotInstalledDate">User bot installed date.</param>
        /// <returns>Learning plan list card as attachment.</returns>
        public async Task GetWeeklyLearningPlanCardAsync(
            ITurnContext<IMessageActivity> turnContext,
            DateTime? userBotInstalledDate)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
            DateTime botInstalledDate = userBotInstalledDate ?? throw new ArgumentNullException(nameof(userBotInstalledDate));

            // Complete learning plan for weeks.
            var completeLearningPlan = await this.GetCompleteLearningPlansAsync();

            if (completeLearningPlan == null || !completeLearningPlan.Any())
            {
                await turnContext.SendActivityAsync(this.localizer.GetString("CompleteLearningPlanNotExistText"));
            }
            else
            {
                int currentLearningWeek = ((DateTime.UtcNow - botInstalledDate).Days / 7) + 1;

                if (currentLearningWeek > this.sharePointOptions.Value.NewHireLearningPlansInWeeks)
                {
                    await turnContext.SendActivityAsync(this.localizer.GetString("NoCurrentWeekPlanText"));
                    return;
                }

                // Send current week learning list card.
                var learningWeek = $"{BotCommandConstants.LearningPlanWeek} {currentLearningWeek}";
                var listCardAttachment = LearningPlanListCard.GetLearningPlanListCard(
                completeLearningPlan.Where(learningPlan => learningPlan.CompleteBy.ToUpperInvariant() == learningWeek.ToUpperInvariant()),
                this.localizer,
                this.localizer.GetString("LearningPlanWeekListCardTitleText", learningWeek),
                this.botOptions.Value.ManifestId,
                this.botOptions.Value.AppBaseUri);

                await turnContext.SendActivityAsync(MessageFactory.Attachment(listCardAttachment));
            }

            return;
        }
    }
}