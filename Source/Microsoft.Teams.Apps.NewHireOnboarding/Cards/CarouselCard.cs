// <copyright file="CarouselCard.cs" company="Microsoft">
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

    /// <summary>
    /// Class that helps to return Carousel card for help command.
    /// </summary>
    public static class CarouselCard
    {
        /// <summary>
        ///  Create the set of cards that comprise the user help carousel.
        /// </summary>
        /// <param name="applicationBasePath">Application base URL.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="applicationManifestId">Application manifest id.</param>
        /// <param name="isManager">True when request comes from manager.</param>
        /// <returns>The cards that comprise the user tour.</returns>
        public static IEnumerable<Attachment> GetUserHelpCards(
            string applicationBasePath,
            IStringLocalizer<Strings> localizer,
            string applicationManifestId = null,
            bool isManager = false)
        {
            var attachments = new List<Attachment>();

            if (isManager)
            {
                attachments.Add(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = GetTakeATourCard(
                        localizer.GetString("ReviewIntroductionsText"),
                        localizer.GetString("ReviewIntroductionsBriefText"),
                        $"{applicationBasePath}/Artifacts/reviewintrosCarouselImage.png",
                        localizer,
                        isManager: true),
                });
            }
            else
            {
                attachments.Add(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = GetTakeATourCard(
                        localizer.GetString("IntroductionTitle"),
                        localizer.GetString("IntroductionBriefText"),
                        $"{applicationBasePath}/Artifacts/reviewintrosCarouselImage.png",
                        localizer),
                });
            }

            attachments.Add(new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = GetTakeATourCard(
                    localizer.GetString("OnBoardingCheckListTitle"),
                    localizer.GetString("LearningPlanBriefText"),
                    $"{applicationBasePath}/Artifacts/learningPlanImage.png",
                    localizer,
                    applicationManifestId),
            });

            attachments.Add(new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = GetTakeATourCard(
                    localizer.GetString("ShareFeedbackTitle"),
                    localizer.GetString("ShareFeedbackBriefText"),
                    $"{applicationBasePath}/Artifacts/feedbackCarouselImage.png",
                    localizer,
                    applicationManifestId),
            });

            return attachments;
        }

        /// <summary>
        /// Create carousel card for user tour.
        /// </summary>
        /// <param name="title">Title of the card.</param>
        /// <param name="briefText">Brief information about the actions.</param>
        /// <param name="imageUri">Image url.</param>
        /// <param name="localizer">The current culture string localizer.</param>
        /// <param name="applicationManifestId">Application manifest id.</param>
        /// <param name="isManager">True when request comes from manager.</param>
        /// <returns>Carousel card.</returns>
        private static AdaptiveCard GetTakeATourCard(
            string title,
            string briefText,
            string imageUri,
            IStringLocalizer<Strings> localizer,
            string applicationManifestId = null,
            bool isManager = false)
        {
            AdaptiveCard carouselCard = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = title,
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large,
                    },
                    new AdaptiveImage
                    {
                         Url = new Uri(imageUri),
                         HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                         AltText = localizer.GetString("AlternativeText"),
                    },
                    new AdaptiveTextBlock
                    {
                        Text = briefText,
                        Wrap = true,
                    },
                },
            };

            if (title == localizer.GetString("OnBoardingCheckListTitle"))
            {
                carouselCard.Actions.Add(
                    new AdaptiveOpenUrlAction
                    {
                        Title = localizer.GetString("ViewCompleteLearningPlanTitle"),
                        Url = new Uri($"{DeepLinkConstants.TabBaseRedirectURL}/{applicationManifestId}/{CardConstants.OnboardingJourneyTabEntityId}"),
                    });
            }
            else if (title == localizer.GetString("ShareFeedbackTitle"))
            {
                carouselCard.Actions.Add(
                    new AdaptiveSubmitAction
                    {
                        Title = localizer.GetString("ShareFeedbackButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Text = BotCommandConstants.ShareFeedback,
                            },
                            Command = BotCommandConstants.ShareFeedback,
                        },
                    });
            }
            else if (isManager)
            {
                carouselCard.Actions.Add(
                    new AdaptiveSubmitAction
                    {
                        Title = localizer.GetString("ReviewIntroductionsText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Text = BotCommandConstants.ReviewIntroductionAction,
                            },
                            Command = BotCommandConstants.ReviewIntroductionAction,
                        },
                    });
            }
            else
            {
                carouselCard.Actions.Add(
                    new AdaptiveSubmitAction
                    {
                        Title = localizer.GetString("IntroduceButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = CardConstants.FetchActionType,
                                Text = BotCommandConstants.IntroductionAction,
                            },
                            Command = BotCommandConstants.IntroductionAction,
                        },
                    });
            }

            return carouselCard;
        }
    }
}
