// <copyright file="IntroductionDetailCard.cs" company="Microsoft">
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
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Newtonsoft.Json;

    /// <summary>
    /// Class that helps to return introduction detail card as attachment.
    /// </summary>
    public static class IntroductionDetailCard
    {
        /// <summary>
        /// This method will construct the introduction detail card for hiring manager's team.
        /// </summary>
        /// <param name="applicationBasePath">Application base path to get the logo of the application.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="introductionEntity">Introduction entity.</param>
        /// <returns>Introduction detail card attachment.</returns>
        public static Attachment GetCard(
            string applicationBasePath,
            IStringLocalizer<Strings> localizer,
            IntroductionEntity introductionEntity)
        {
            introductionEntity = introductionEntity ?? throw new ArgumentNullException(nameof(introductionEntity));

            var questionAnswerList = JsonConvert.DeserializeObject<List<IntroductionDetail>>(introductionEntity.NewHireQuestionnaire);

            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveImage
                    {
                        Url = new Uri($"{applicationBasePath}/Artifacts/hiringManagerNotification.png"),
                        AltText = localizer.GetString("AlternativeText"),
                    },
                },
            };

            foreach (var questionAnswer in questionAnswerList)
            {
                card.Body.Add(
                    new AdaptiveTextBlock
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                        Spacing = AdaptiveSpacing.Medium,
                        Text = questionAnswer.Question,
                        Wrap = true,
                    });

                card.Body.Add(
                    new AdaptiveTextBlock
                    {
                        Spacing = AdaptiveSpacing.Small,
                        Text = questionAnswer.Answer,
                        Wrap = true,
                    });
            }

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
        }
    }
}
