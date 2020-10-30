// <copyright file="NewHireIntroductionCard.cs" company="Microsoft">
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
    /// Class that helps to return introduction card as attachment.
    /// </summary>
    public static class NewHireIntroductionCard
    {
        /// <summary>
        /// Get new hire introduction card attachment to show on Microsoft Teams personal scope.
        /// </summary>
        /// <param name="introductionEntity">New hire introduction details.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="applicationBasePath">Application base path to get the logo of the application.</param>
        /// <param name="isAllQuestionsAnswered">False when any of the question is not answered.</param>
        /// <returns>New Hire Introduction Card attachment.</returns>
        public static Attachment GetNewHireIntroductionCardAttachment(IntroductionEntity introductionEntity, IStringLocalizer<Strings> localizer, string applicationBasePath, bool isAllQuestionsAnswered = true)
        {
            introductionEntity = introductionEntity ?? throw new ArgumentNullException(nameof(introductionEntity));
            List<IntroductionDetail> questionAnswerList = JsonConvert.DeserializeObject<List<IntroductionDetail>>(introductionEntity.NewHireQuestionnaire);

            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveImage
                    {
                        Url = new Uri($"{applicationBasePath}/Artifacts/introductionImage.png"),
                        AltText = localizer.GetString("AlternativeText"),
                    },
                    new AdaptiveTextBlock
                    {
                        Spacing = AdaptiveSpacing.Medium,
                        Text = localizer.GetString("IntroductionText"),
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Size = AdaptiveTextSize.Small,
                        Spacing = AdaptiveSpacing.Small,
                        Text = localizer.GetString("IntroductionHeaderText"),
                        Wrap = true,
                    },
                    new AdaptiveTextInput
                    {
                        Spacing = AdaptiveSpacing.Small,
                        Value = !string.IsNullOrWhiteSpace(introductionEntity.NewHireProfileNote) ? introductionEntity.NewHireProfileNote : localizer.GetString("IntroductionGreetText", introductionEntity.NewHireName),
                        Id = CardConstants.NewHireProfileNoteInputId,
                        MaxLength = 500,
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = localizer.GetString("IntroductionSubmitButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Command = BotCommandConstants.SubmitIntroductionAction,
                        },
                    },
                },
            };

            foreach (var questionAnswer in questionAnswerList)
            {
                card.Body.Add(
                new AdaptiveTextBlock
                {
                    Size = AdaptiveTextSize.Medium,
                    Text = questionAnswer.Question,
                    Wrap = true,
                    Spacing = AdaptiveSpacing.Medium,
                });

                card.Body.Add(
                new AdaptiveTextInput
                {
                    Id = $"{CardConstants.QuestionId}{questionAnswerList.IndexOf(questionAnswer)}",
                    Spacing = AdaptiveSpacing.Small,
                    Value = !string.IsNullOrWhiteSpace(questionAnswer.Answer) ? questionAnswer.Answer : string.Empty,
                    MaxLength = 500,
                    Placeholder = localizer.GetString("IntroductionInputPlaceholderText"),
                });
            }

            card.Body.Add(
                new AdaptiveTextBlock
                {
                    Text = localizer.GetString("ValidationMessageText"),
                    Spacing = AdaptiveSpacing.Medium,
                    IsVisible = !isAllQuestionsAnswered,
                    Color = AdaptiveTextColor.Attention,
                    Size = AdaptiveTextSize.Small,
                });

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
        }
    }
}
