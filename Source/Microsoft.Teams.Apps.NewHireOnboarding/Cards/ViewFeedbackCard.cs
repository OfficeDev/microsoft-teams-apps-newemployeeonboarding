// <copyright file="ViewFeedbackCard.cs" company="Microsoft">
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
    /// Class that helps to return view feedback card as attachment.
    /// </summary>
    public static class ViewFeedbackCard
    {
        /// <summary>
        /// This method will construct the feedback card to share individual feedbacks.
        /// </summary>
        /// <param name="applicationBasePath">Application base path to get the logo of the application.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <returns>Feedback card attachment.</returns>
        public static Attachment GetFeedbackCard(
            string applicationBasePath,
            IStringLocalizer<Strings> localizer)
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
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveImage
                                    {
                                        Url = new Uri($"{applicationBasePath}/Artifacts/viewSubmittedFeedback.png"),
                                        AltText = localizer.GetString("AlternativeText"),
                                    },
                                },
                            },
                            new AdaptiveColumn
                            {
                                Width = AdaptiveColumnWidth.Stretch,
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveTextBlock
                                    {
                                        Text = localizer.GetString("ViewReportCardHeaderText"),
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Size = AdaptiveTextSize.Medium,
                                        Spacing = AdaptiveSpacing.None,
                                        Wrap = true,
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Size = AdaptiveTextSize.Default,
                                        Spacing = AdaptiveSpacing.None,
                                        Text = localizer.GetString("ViewReportCardTitleText"),
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Text = localizer.GetString("ViewReportUserMessageText"),
                                        Wrap = true,
                                    },
                                },
                            },
                        },
                    },
                    new AdaptiveTextBlock
                    {
                        Text = localizer.GetString("FeedbackReportNavigationText"),
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Medium,
                        Spacing = AdaptiveSpacing.Medium,
                        Wrap = true,
                    },
                },
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
        }
    }
}
