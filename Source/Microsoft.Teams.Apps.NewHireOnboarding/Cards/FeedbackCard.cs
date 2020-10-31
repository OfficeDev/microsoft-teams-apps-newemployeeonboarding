// <copyright file="FeedbackCard.cs" company="Microsoft">
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
    /// Class that helps to return feedback card as attachment.
    /// </summary>
    public static class FeedbackCard
    {
        /// <summary>
        /// This method will construct the feedback card to share individual feedbacks.
        /// </summary>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="isErrorMessageVisible">flag if feedback is empty.</param>
        /// <returns>Feedback card.</returns>
        public static Attachment GetFeedbackCardAttachment(IStringLocalizer<Strings> localizer, bool isErrorMessageVisible = false)
        {
            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(CardConstants.AdaptiveCardVersion))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = localizer.GetString("FeedbackHeaderText"),
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large,
                        Wrap = true,
                    },
                    new AdaptiveTextBlock
                    {
                        Text = localizer.GetString("FeedbackTitleText"),
                        Size = AdaptiveTextSize.Small,
                    },
                    new AdaptiveTextInput
                    {
                        Id = CardConstants.FeedbackTextInputId,
                        Spacing = AdaptiveSpacing.Small,
                        MaxLength = 200,
                        IsMultiline = true,
                        Placeholder = localizer.GetString("FeedbackPlaceHolderText"),
                    },
                    new AdaptiveTextBlock
                    {
                        Size = AdaptiveTextSize.Small,
                        Text = localizer.GetString("FeedbackRequiredFieldText"),
                        IsVisible = isErrorMessageVisible,
                        Color = AdaptiveTextColor.Attention,
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = localizer.GetString("SubmitButtonText"),
                        Data = new AdaptiveSubmitActionData
                        {
                            Msteams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Text = BotCommandConstants.SubmitFeedback,
                            },
                            Command = BotCommandConstants.SubmitFeedback,
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
