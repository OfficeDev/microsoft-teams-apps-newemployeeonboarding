// <copyright file="LearningPlanListCard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Cards
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Card;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;

    /// <summary>
    /// Class that helps to create learning plan list card.
    /// </summary>
    public static class LearningPlanListCard
    {
        /// <summary>
        /// Get list card for complete learning plan.
        /// </summary>
        /// <param name="learningPlans">Learning plans list object.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="cardTitle">Learning plan list card title.</param>
        /// <param name="applicationManifestId">Application manifest id.</param>
        /// <param name="applicationBasePath">Application base path.</param>
        /// <returns>An attachment card for learning plan.</returns>
        public static Attachment GetLearningPlanListCard(
            IEnumerable<LearningPlanListItemField> learningPlans,
            IStringLocalizer<Strings> localizer,
            string cardTitle,
            string applicationManifestId,
            string applicationBasePath)
        {
            learningPlans = learningPlans ?? throw new ArgumentNullException(nameof(learningPlans));

            ListCard card = new ListCard
            {
                Title = cardTitle,
                Items = new List<ListCardItem>(),
                Buttons = new List<ListCardButton>(),
            };

            if (!learningPlans.Any())
            {
                card.Items.Add(new ListCardItem
                {
                    Type = "resultItem",
                    Id = Guid.NewGuid().ToString(),
                    Title = localizer.GetString("CurrentWeekLearningPlanNotExistText"),
                });
            }

            foreach (var learningPlan in learningPlans)
            {
                card.Items.Add(new ListCardItem
                {
                    Type = "resultItem",
                    Id = Guid.NewGuid().ToString(),
                    Title = learningPlan.Topic,
                    Subtitle = learningPlan.TaskName,
                    Icon = !string.IsNullOrEmpty(learningPlan?.TaskImage?.Url) ? learningPlan.TaskImage.Url : $"{applicationBasePath}/Artifacts/listCardDefaultImage.png",
                    Tap = new ListCardItemEvent
                    {
                        Type = CardConstants.MessageBack,
                        Value = $"{learningPlan.CompleteBy} => {learningPlan.Topic} => {learningPlan.TaskName}",
                    },
                });
            }

            var viewCompletePlanActionButton = new ListCardButton()
            {
                Title = localizer.GetString("ViewCompleteLearningPlanButtonText"),
                Type = CardConstants.OpenUrlType,
                Value = $"{DeepLinkConstants.TabBaseRedirectURL}/{applicationManifestId}/{CardConstants.OnboardingJourneyTabEntityId}",
            };

            card.Buttons.Add(viewCompletePlanActionButton);

            var shareFeedbackActionButton = new ListCardButton()
            {
                Title = localizer.GetString("ShareFeedbackButtonText"),
                Type = CardConstants.MessageBack,
                Value = BotCommandConstants.ShareFeedback,
            };

            card.Buttons.Add(shareFeedbackActionButton);

            return new Attachment
            {
                ContentType = CardConstants.ListCardContentType,
                Content = card,
            };
        }
    }
}
