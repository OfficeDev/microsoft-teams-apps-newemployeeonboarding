// <copyright file="WelcomeCardFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System;
    using System.Collections.Generic;
    using AdaptiveCards;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;

    /// <summary>
    /// Class that helps to send welcome card attachment methods.
    /// </summary>
    public class WelcomeCardFactory : IWelcomeCardFactory
    {
        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// A set of key/value application configuration properties for bot settings.
        /// </summary>
        private readonly IOptions<BotOptions> botOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeCardFactory"/> class.
        /// </summary>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="botOptions">A set of key/value application configuration properties.</param>
        public WelcomeCardFactory(
            IStringLocalizer<Strings> localizer,
            IOptions<BotOptions> botOptions)
        {
            this.localizer = localizer;
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
        }

        /// <summary>
        /// This method will construct the new hire welcome card when bot is added in personal scope.
        /// </summary>
        /// <returns>New hire welcome card attachment.</returns>
        public Attachment GetNewHireWelcomeCard()
        {
            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("WelcomeHeaderText"),
                        Spacing = AdaptiveSpacing.Medium,
                        Color = AdaptiveTextColor.Accent,
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Medium,
                    },
                    new AdaptiveTextBlock
                    {
                        Spacing = AdaptiveSpacing.None,
                        Size = AdaptiveTextSize.ExtraLarge,
                        Text = $"**{this.localizer.GetString("WelcomeSubHeaderText")}**",
                    },
                    new AdaptiveImage
                    {
                        Url = new Uri($"{this.botOptions.Value.AppBaseUri}/Artifacts/welcomeImage.png"),
                        AltText = this.localizer.GetString("AlternativeText"),
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("WelcomeContentText"),
                        Spacing = AdaptiveSpacing.Small,
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("AccessOnDemandBulletPoint"),
                        Spacing = AdaptiveSpacing.Small,
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("HelpBulletPoint"),
                        Spacing = AdaptiveSpacing.Small,
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("MoreInfoBulletPoint"),
                        Spacing = AdaptiveSpacing.Small,
                        Wrap = true,
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = this.localizer.GetString("IntroduceButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = CardConstants.FetchActionType,
                                Text = BotCommandConstants.IntroductionAction,
                            },
                            Command = BotCommandConstants.IntroductionAction,
                        },
                    },
                    new AdaptiveSubmitAction
                    {
                        Title = this.localizer.GetString("TakeaTourButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Text = BotCommandConstants.HelpAction,
                            },
                            Command = BotCommandConstants.HelpAction,
                        },
                    },
                    new AdaptiveSubmitAction
                    {
                        Title = this.localizer.GetString("ViewLearningButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Text = BotCommandConstants.ViewLearningAction,
                            },
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

        /// <summary>
        /// This method will construct the team welcome card when bot is added in team scope.
        /// </summary>
        /// <returns>Team welcome card attachment.</returns>
        public Attachment GetTeamWelcomeCard()
        {
            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large,
                        Spacing = AdaptiveSpacing.None,
                        Text = this.localizer.GetString("WelcomeSubHeaderText"),
                    },
                    new AdaptiveImage
                    {
                        Url = new Uri($"{this.botOptions.Value.AppBaseUri}/Artifacts/welcomeImage.png"),
                        AltText = this.localizer.GetString("AlternativeText"),
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("TeamWelcomeContentText"),
                        Spacing = AdaptiveSpacing.Small,
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("TeamWelcomeCardBulletPointText"),
                        Spacing = AdaptiveSpacing.Small,
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

        /// <summary>
        /// This method will construct the hiring manager card when bot is added in personal scope.
        /// </summary>
        /// <returns>Hiring manager welcome card attachment.</returns>
        public Attachment GetHiringManagerWelcomeCard()
        {
            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large,
                        Spacing = AdaptiveSpacing.None,
                        Text = this.localizer.GetString("WelcomeSubHeaderText"),
                    },
                    new AdaptiveImage
                    {
                        Url = new Uri($"{this.botOptions.Value.AppBaseUri}/Artifacts/welcomeImage.png"),
                        AltText = this.localizer.GetString("AlternativeText"),
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("HiringManagerWelcomeContentText"),
                        Spacing = AdaptiveSpacing.Small,
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("HiringManagerBulletPoint1Text"),
                        Spacing = AdaptiveSpacing.Small,
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("HiringManagerBulletPoint2Text"),
                        Spacing = AdaptiveSpacing.Small,
                        Wrap = true,
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = this.localizer.GetString("ReviewIntroductionsText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Text = BotCommandConstants.ReviewIntroductionAction,
                            },
                            Command = BotCommandConstants.ReviewIntroductionAction,
                        },
                    },
                    new AdaptiveSubmitAction
                    {
                        Title = this.localizer.GetString("TakeaTourButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Text = BotCommandConstants.HelpAction,
                            },
                            Command = BotCommandConstants.HelpAction,
                        },
                    },
                    new AdaptiveOpenUrlAction
                    {
                        Title = this.localizer.GetString("ViewCompleteLearningPlanTitle"),
                        Url = new Uri($"{DeepLinkConstants.TabBaseRedirectURL}/{this.botOptions.Value.ManifestId}/{CardConstants.OnboardingJourneyTabEntityId}"),
                    },
                },
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
        }

        /// <summary>
        /// This method will construct the HR welcome card when bot is added in team scope.
        /// </summary>
        /// <returns>Human resource welcome card attachment.</returns>
        public Attachment GetHumanResourceWelcomeCard()
        {
            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large,
                        Spacing = AdaptiveSpacing.None,
                        Text = this.localizer.GetString("WelcomeSubHeaderText"),
                    },
                    new AdaptiveImage
                    {
                        Url = new Uri($"{this.botOptions.Value.AppBaseUri}/Artifacts/welcomeImage.png"),
                        AltText = this.localizer.GetString("AlternativeText"),
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("HumanResourceWelcomeContentText"),
                        Spacing = AdaptiveSpacing.Small,
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = this.localizer.GetString("HumanResourceBulletPointText"),
                        Spacing = AdaptiveSpacing.Small,
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