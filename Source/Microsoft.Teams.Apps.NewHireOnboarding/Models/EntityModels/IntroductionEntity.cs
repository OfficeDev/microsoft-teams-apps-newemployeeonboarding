// <copyright file="IntroductionEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Class contains New hire details where application is installed.
    /// </summary>
    public class IntroductionEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets New hire Azure Active Directory Object id.
        /// </summary>
        public string NewHireAadObjectId
        {
            get
            {
                return this.RowKey;
            }

            set
            {
                this.RowKey = value;
            }
        }

        /// <summary>
        /// Gets or sets manager Azure Active Directory Object id from Microsoft Graph API.
        /// </summary>
        public string ManagerAadObjectId
        {
            get
            {
                return this.PartitionKey;
            }

            set
            {
                this.PartitionKey = value;
            }
        }

        /// <summary>
        /// Gets or sets new hire question answer values.
        /// </summary>
        public string NewHireQuestionnaire { get; set; }

        /// <summary>
        /// Gets or sets approval status using enum values.
        /// </summary>
        public int ApprovalStatus { get; set; }

        /// <summary>
        /// Gets or sets comments on introduction from hiring manager.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets new hire name.
        /// </summary>
        public string NewHireName { get; set; }

        /// <summary>
        /// Gets or sets new hire profile note.
        /// </summary>
        public string NewHireProfileNote { get; set; }

        /// <summary>
        /// Gets or sets new hire conversationId.
        /// </summary>
        public string NewHireConversationId { get; set; }

        /// <summary>
        /// Gets or sets manager conversationId.
        /// </summary>
        public string ManagerConversationId { get; set; }

        /// <summary>
        /// Gets or sets new hire user principal name.
        /// </summary>
        public string NewHireUserPrincipalName { get; set; }

        /// <summary>
        /// Gets or sets the date time when the introduction approved.
        /// </summary>
        public DateTime? ApprovedOn { get; set; }

        /// <summary>
        /// Gets or sets survey notification send status.
        /// </summary>
        public int SurveyNotificationSentStatus { get; set; }

        /// <summary>
        /// Gets or sets survey notification send date and time.
        /// </summary>
        public DateTime? SurveyNotificationSentOn { get; set; }

        /// <summary>
        /// Gets or sets user profile image url.
        /// </summary>
        public string UserProfileImageUrl { get; set; }
    }
}
