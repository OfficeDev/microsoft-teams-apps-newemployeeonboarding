// <copyright file="OnBoardingCheckListCard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Cards
{
    using System;
    using System.Collections.Generic;
    using AdaptiveCards;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;

    /// <summary>
    /// Class that helps to return on-boarding check list card as attachment.
    /// </summary>
    public static class OnBoardingCheckListCard
    {
        /// <summary>
        /// This method will construct the on-boarding check list card.
        /// </summary>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="applicationManifestId">Application manifest id.</param>
        /// <returns>On boarding check list card attachment.</returns>
        public static Attachment GetCard(
            IStringLocalizer<Strings> localizer,
            string applicationManifestId)
        {
            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = localizer.GetString("BoardingCheckListHeaderText"),
                        Wrap = true,
                    },
                },
            };

            card.Actions.Add(
                new AdaptiveOpenUrlAction
                {
                    Title = localizer.GetString("ViewCompleteLearningPlanTitle"),
                    Url = new Uri($"{DeepLinkConstants.TabBaseRedirectURL}/{applicationManifestId}/{CardConstants.OnboardingJourneyTabEntityId}"),
                });

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
        }
    }
}
