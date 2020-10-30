// <copyright file="HiringManagerNotificationCard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Cards
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using AdaptiveCards;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Newtonsoft.Json;

    /// <summary>
    /// Class that helps to return new employee introduction card for hiring manager.
    /// </summary>
    public static class HiringManagerNotificationCard
    {
        /// <summary>
        /// Represents the container minimum height in pixel.
        /// </summary>
        private const int ContainerPixelHeight = 270;

        /// <summary>
        /// Represents the comments input id.
        /// </summary>
        private const string CommentsInputId = "Comments";

        /// <summary>
        /// Represents the team input id.
        /// </summary>
        private const string TeamInputId = "teamId";

        /// <summary>
        /// Get new employee introduction card attachment for hiring manager to show on Microsoft Teams personal scope.
        /// </summary>
        /// <param name="applicationBasePath">Application base path to get the logo of the application.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="introductionEntity">Introduction entity.</param>
        /// <returns>New employee introduction card attachment.</returns>
        public static Attachment GetNewEmployeeIntroductionCard(
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
                    new AdaptiveTextBlock
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large,
                        Spacing = AdaptiveSpacing.Medium,
                        Text = introductionEntity.NewHireName,
                    },
                    new AdaptiveTextBlock
                    {
                        Spacing = AdaptiveSpacing.Small,
                        Text = !string.IsNullOrWhiteSpace(introductionEntity.NewHireProfileNote) ? introductionEntity.NewHireProfileNote : localizer.GetString("IntroductionGreetText", introductionEntity.NewHireName),
                        Wrap = true,
                    },
                },
                Actions = new List<AdaptiveAction>(),
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

            card.Actions.Add(
                new AdaptiveSubmitAction
                {
                    Title = localizer.GetString("ApproveButtonText"),
                    Data = new AdaptiveSubmitActionData
                    {
                        Msteams = new CardAction
                        {
                            Type = CardConstants.FetchActionType,
                            Text = BotCommandConstants.ApproveIntroductionAction,
                        },
                        Command = BotCommandConstants.ApproveIntroductionAction,
                        IntroductionEntity = introductionEntity,
                    },
                });

            card.Actions.Add(
                new AdaptiveShowCardAction()
                {
                    Title = localizer.GetString("TellMeMoreButtonText"),
                    Card = GetMoreInfoCard(localizer, introductionEntity),
                });

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
        }

        /// <summary>
        /// Get confirmation card attachment to post a card to particular team.
        /// </summary>
        /// <param name="teamChannelMapping">Teams/Channel mappings.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="introductionEntity">New hire introduction details.</param>
        /// <param name="isTeamSelected">True if team selected.</param>
        /// <returns>Introduction confirmation card attachment.</returns>
        public static Attachment GetTeamConfirmationCard(List<Models.TeamDetail> teamChannelMapping, IStringLocalizer<Strings> localizer, IntroductionEntity introductionEntity, bool isTeamSelected = true)
        {
            introductionEntity = introductionEntity ?? throw new ArgumentNullException(nameof(introductionEntity));
            teamChannelMapping = teamChannelMapping ?? throw new ArgumentNullException(nameof(teamChannelMapping));

            var teamsChoices = new List<AdaptiveChoice>();
            foreach (var team in teamChannelMapping)
            {
                if (team.Channels != null)
                {
                    foreach (var channel in team.Channels)
                    {
                        teamsChoices.Add(new AdaptiveChoice()
                        {
                            Title = HttpUtility.HtmlEncode($"{team.TeamName} | {channel.ChannelName}"),

                            // binding team id and Channel id by ';' (semicolon)
                            Value = $"{team.TeamId};{channel.ChannelId}",
                        });
                    }
                }
            }

            AdaptiveCard card = new AdaptiveCard(CardConstants.AdaptiveCardVersion)
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveContainer()
                    {
                        PixelMinHeight = ContainerPixelHeight,
                        Items = new List<AdaptiveElement>()
                        {
                            new AdaptiveTextBlock
                            {
                                Weight = AdaptiveTextWeight.Bolder,
                                Spacing = AdaptiveSpacing.Medium,
                                Text = localizer.GetString("TeamConfirmHeaderText"),
                                Wrap = true,
                            },
                            new AdaptiveChoiceSetInput
                            {
                                Spacing = AdaptiveSpacing.Small,
                                Id = TeamInputId,
                                Choices = teamsChoices,
                            },
                            new AdaptiveTextBlock
                            {
                                Size = AdaptiveTextSize.Small,
                                Text = localizer.GetString("RequiredFieldText"),
                                IsVisible = !isTeamSelected,
                                HorizontalAlignment = AdaptiveHorizontalAlignment.Right,
                                Color = AdaptiveTextColor.Attention,
                            },
                        },
                    },
                },

                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = localizer.GetString("PostInTeamButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Command = BotCommandConstants.PostTeamNotificationAction,
                            IntroductionEntity = introductionEntity,
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
        /// Get more information card attachment to get more information about new hire employee.
        /// </summary>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="introductionEntity">Introduction entity</param>
        /// <returns>Tell me more information card attachment.</returns>
        public static AdaptiveCard GetMoreInfoCard(IStringLocalizer<Strings> localizer, IntroductionEntity introductionEntity)
        {
            introductionEntity = introductionEntity ?? throw new ArgumentNullException(nameof(introductionEntity));

            AdaptiveCard card = new AdaptiveCard(CardConstants.AdaptiveCardVersion)
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                        Spacing = AdaptiveSpacing.Small,
                        Text = localizer.GetString("MoreInfoCardSubHeaderText", introductionEntity.NewHireName),
                        Wrap = true,
                    },
                    new AdaptiveTextInput
                    {
                        Id = CommentsInputId,
                        Placeholder = localizer.GetString("CommentsPlaceHolderText"),
                        IsMultiline = true,
                        Height = AdaptiveHeight.Stretch,
                        MaxLength = 200,
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = localizer.GetString("SendButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Text = BotCommandConstants.RequestMoreInfoAction,
                            },
                            IntroductionEntity = introductionEntity,
                        },
                    },
                },
            };

            return card;
        }

        /// <summary>
        /// Construct the card to show validation message on task module.
        /// </summary>
        /// <param name="message">Message to show in card as validation.</param>
        /// <returns>Validation message card attachment.</returns>
        public static Attachment GetValidationMessageCard(string message)
        {
            AdaptiveCard validationCard = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = message,
                        Wrap = true,
                    },
                },
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = validationCard,
            };
        }
    }
}
