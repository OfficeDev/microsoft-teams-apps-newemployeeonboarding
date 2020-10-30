// <copyright file="HelpCard.cs" company="Microsoft">
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
    /// Class that helps to return help card for un-supported bot commands as attachment.
    /// </summary>
    public static class HelpCard
    {
        /// <summary>
        /// This method will construct the help card.
        /// </summary>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <returns>Help card attachment.</returns>
        public static Attachment GetCard(IStringLocalizer<Strings> localizer)
        {
            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = localizer.GetString("HelpCardHeaderText"),
                        Wrap = true,
                    },
                },
            };

            card.Actions.Add(
                new AdaptiveSubmitAction
                {
                    Title = localizer.GetString("TakeaTourButtonText"),
                    Data = new AdaptiveSubmitActionData
                    {
                        Msteams = new CardAction
                        {
                            Type = ActionTypes.MessageBack,
                            Text = BotCommandConstants.HelpAction,
                        },
                        Command = BotCommandConstants.HelpAction,
                    },
                });

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
        }
    }
}
