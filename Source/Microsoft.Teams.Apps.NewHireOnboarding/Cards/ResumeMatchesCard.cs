// <copyright file="ResumeMatchesCard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Cards
{
    using System.Collections.Generic;
    using AdaptiveCards;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;

    /// <summary>
    /// Class for resume all matches card.
    /// </summary>
    public static class ResumeMatchesCard
    {
        /// <summary>
        /// Creates the notification card to resume all pair up meetings.
        /// </summary>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <returns>Pair-up notification card</returns>
        public static Attachment GetResumeMatchesCard(IStringLocalizer<Strings> localizer)
        {
            AdaptiveCard resumeMatchesCard = new AdaptiveCard(CardConstants.AdaptiveCardVersion)
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = localizer.GetString("ResumeMatchesCardContent"),
                        Wrap = true,
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = localizer.GetString("ResumeMatchesButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                DisplayText = localizer.GetString("ResumeMatchesButtonText"),
                                Text = BotCommandConstants.ResumeAllMatches,
                            },
                        },
                    },
                },
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = resumeMatchesCard,
            };
        }
    }
}