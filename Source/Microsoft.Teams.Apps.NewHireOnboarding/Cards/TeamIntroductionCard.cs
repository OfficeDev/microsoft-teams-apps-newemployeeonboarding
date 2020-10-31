// <copyright file="TeamIntroductionCard.cs" company="Microsoft">
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
    /// Class that helps to team introduction card as attachment.
    /// </summary>
    public static class TeamIntroductionCard
    {
        /// <summary>
        /// Represent image width in pixel.
        /// </summary>
        private const uint IntroductionImageWidthInPixel = 200;

        /// <summary>
        /// Represent image width in pixel.
        /// </summary>
        private const uint IntroductionImageHeightInPixel = 200;

        /// <summary>
        /// Get notification card after approved introduction from hiring manager.
        /// </summary>
        /// <param name="applicationBasePath">Application base path to get the logo of the application.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="introductionEntity">New hire introduction details.</param>
        /// <returns>Team introduction card attachment.</returns>
        public static Attachment GetCard(
            string applicationBasePath,
            IStringLocalizer<Strings> localizer,
            IntroductionEntity introductionEntity)
        {
            introductionEntity = introductionEntity ?? throw new ArgumentNullException(nameof(introductionEntity));

            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = localizer.GetString("TeamNotificationHeaderText", introductionEntity.NewHireName),
                        Spacing = AdaptiveSpacing.Small,
                        Color = AdaptiveTextColor.Accent,
                    },
                    new AdaptiveTextBlock
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large,
                        Spacing = AdaptiveSpacing.None,
                        Text = introductionEntity.NewHireName,
                    },
                    new AdaptiveImage
                    {
                        Url = new Uri(!string.IsNullOrEmpty(introductionEntity.UserProfileImageUrl) ? introductionEntity.UserProfileImageUrl : $"{applicationBasePath}/Artifacts/peopleAvatar.png"),
                        Spacing = AdaptiveSpacing.ExtraLarge,
                        PixelHeight = IntroductionImageHeightInPixel,
                        PixelWidth = IntroductionImageWidthInPixel,
                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                        Style = AdaptiveImageStyle.Person,
                        AltText = localizer.GetString("AlternativeText"),
                    },
                    new AdaptiveTextBlock
                    {
                        Text = !string.IsNullOrEmpty(introductionEntity.NewHireProfileNote) ? introductionEntity.NewHireProfileNote : localizer.GetString("IntroductionGreetText", introductionEntity.NewHireName),
                        Spacing = AdaptiveSpacing.Small,
                        Wrap = true,
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveOpenUrlAction
                    {
                        Title = localizer.GetString("ChatButtonText", introductionEntity.NewHireName),
                        Url = new Uri($"{DeepLinkConstants.ChatInitiateURL}?users={Uri.EscapeDataString(introductionEntity.NewHireUserPrincipalName)}"),
                    },
                    new AdaptiveSubmitAction
                    {
                        Title = localizer.GetString("SeeMoreDetailsButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = CardConstants.FetchActionType,
                                Text = BotCommandConstants.SeeIntroductionDetailAction,
                            },
                            IntroductionEntity = introductionEntity,
                            Command = BotCommandConstants.SeeIntroductionDetailAction,
                        },
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
