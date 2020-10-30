// <copyright file="PairUpNotificationAdaptiveCard.cs" company="Microsoft">
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

    /// <summary>
    /// Class for the pair-up notification card.
    /// </summary>
    public static class PairUpNotificationAdaptiveCard
    {
        /// <summary>
        /// Creates the pair-up notification card.
        /// </summary>
        /// <param name="sender">The user who will be sending this card.</param>
        /// <param name="recipient">The user who will be receiving this card.</param>
        /// <param name="localizer">The current cultures' string localizer.</param>
        /// <returns>Pair-up notification card</returns>
        public static Attachment GetPairUpNotificationCard(UserEntity sender, UserEntity recipient, IStringLocalizer<Strings> localizer)
        {
            sender = sender ?? throw new ArgumentNullException(nameof(sender));
            recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));

            var meetingTitle = localizer.GetString("MeetupTitle", sender.Name, recipient.Name);
            var meetingContent = localizer.GetString("MeetupContent", localizer.GetString("AppTitle"));
            var meetingLink = $"{DeepLinkConstants.MeetingLink}{Uri.EscapeDataString(meetingTitle)}&attendees={recipient.UserPrincipalName}&content={Uri.EscapeDataString(meetingContent)}";
            var encodedMessage = Uri.EscapeDataString(localizer.GetString("InitiateChatText"));

            AdaptiveCard pairUpNotificationCard = new AdaptiveCard(CardConstants.AdaptiveCardVersion)
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Size = AdaptiveTextSize.Medium,
                        Weight = AdaptiveTextWeight.Bolder,
                        Text = localizer.GetString("MatchUpCardTitleContent"),
                        Wrap = true,
                        MaxLines = 2,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = localizer.GetString("MatchUpCardMatchedText", recipient.Name),
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = localizer.GetString("MatchUpCardContentPart1", localizer.GetString("AppTitle"), recipient.Name),
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = localizer.GetString("MatchUpCardContentPart2"),
                        Wrap = true,
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveOpenUrlAction
                    {
                        Title = localizer.GetString("ChatWithMatchButtonText", recipient.Name),
                        Url = new Uri($"{DeepLinkConstants.ChatInitiateURL}?users={Uri.EscapeDataString(recipient.UserPrincipalName)}&message={encodedMessage}"),
                    },
                    new AdaptiveOpenUrlAction
                    {
                        Title = localizer.GetString("ProposeMeetupButtonText"),
                        Url = new Uri(meetingLink),
                    },
                    new AdaptiveSubmitAction
                    {
                        Title = localizer.GetString("PauseMatchesButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                DisplayText = localizer.GetString("PauseMatchesButtonText"),
                                Text = BotCommandConstants.PauseAllMatches,
                            },
                        },
                    },
                },
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = pairUpNotificationCard,
            };
        }
    }
}