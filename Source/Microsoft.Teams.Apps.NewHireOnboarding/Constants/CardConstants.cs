// <copyright file="CardConstants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Constants
{
    /// <summary>
    /// A class that holds card constants that are used in multiple files.
    /// </summary>
    public static class CardConstants
    {
        /// <summary>
        /// Describes adaptive card version to be used. Version can be upgraded or changed using this value.
        /// </summary>
        public const string AdaptiveCardVersion = "1.2";

        /// <summary>
        /// default value for channel activity to send notifications.
        /// </summary>
        public const string TeamsBotFrameworkChannelId = "msteams";

        /// <summary>
        /// Represents the new hire profile note input id.
        /// </summary>
        public const string NewHireProfileNoteInputId = "NewHireProfileNoteTextInput";

        /// <summary>
        /// Represents the conversation type as personal.
        /// </summary>
        public const string PersonalConversationType = "personal";

        /// <summary>
        /// Represents the conversation type as channel.
        /// </summary>
        public const string ChannelConversationType = "channel";

        /// <summary>
        /// List card content type.
        /// </summary>
        public const string ListCardContentType = "application/vnd.microsoft.teams.card.list";

        /// <summary>
        /// Task fetch action Type.
        /// </summary>
        public const string FetchActionType = "task/fetch";

        /// <summary>
        /// submit action Type.
        /// </summary>
        public const string SubmitActionType = "task/submit";

        /// <summary>
        /// Open URL Type.
        /// </summary>
        public const string OpenUrlType = "openUrl";

        /// <summary>
        /// Message back type.
        /// </summary>
        public const string MessageBack = "imback";

        /// <summary>
        /// Question unique id.
        /// </summary>
        public const string QuestionId = "QuestionId_";

        /// <summary>
        /// Feedback text input id.
        /// </summary>
        public const string FeedbackTextInputId = "FeedbackTextInput";

        /// <summary>
        /// Entity id of static onboarding journey tab.
        /// </summary>
        public const string OnboardingJourneyTabEntityId = "Journey";
    }
}
