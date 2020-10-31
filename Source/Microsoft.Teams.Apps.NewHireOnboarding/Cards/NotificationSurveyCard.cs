// <copyright file="NotificationSurveyCard.cs" company="Microsoft">
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
    /// Class that helps to return survey notification card as attachment.
    /// </summary>
    public static class NotificationSurveyCard
    {
        /// <summary>
        /// This method will construct the survey notification card.
        /// </summary>
        /// <param name="applicationBasePath">Application base path to get the logo of the application.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="surveyNotificationSharePointPath">SharePoint path for Survey Notification.</param>
        /// <returns>Survey notification card attachment.</returns>
        public static Attachment GetSurveyNotificationCard(
            string applicationBasePath,
            IStringLocalizer<Strings> localizer,
            string surveyNotificationSharePointPath)
        {
            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>
                        {
                            new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveTextBlock
                                    {
                                        Text = localizer.GetString("CardHeaderText"),
                                        Spacing = AdaptiveSpacing.Small,
                                        Color = AdaptiveTextColor.Accent,
                                        Wrap = true,
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Size = AdaptiveTextSize.Large,
                                        Spacing = AdaptiveSpacing.Medium,
                                        Text = localizer.GetString("CardSubHeaderText"),
                                        Wrap = true,
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Spacing = AdaptiveSpacing.Medium,
                                        Text = localizer.GetString("CardContentText"),
                                        Wrap = true,
                                    },
                                },
                            },
                            new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveImage
                                    {
                                        Url = new Uri($"{applicationBasePath}/Artifacts/notificationSurvey.png"),
                                        HorizontalAlignment = AdaptiveHorizontalAlignment.Right,
                                        AltText = localizer.GetString("AlternativeText"),
                                    },
                                },
                            },
                        },
                    },
                },
            };

            card.Actions.Add(
                new AdaptiveOpenUrlAction
                {
                    Title = localizer.GetString("GetStartedButtonText"),
                    Url = new Uri(surveyNotificationSharePointPath),
                });

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
        }
    }
}
