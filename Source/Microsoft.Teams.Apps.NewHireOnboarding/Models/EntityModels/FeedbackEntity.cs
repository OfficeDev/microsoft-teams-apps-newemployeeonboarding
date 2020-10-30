// <copyright file="FeedbackEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Class contains shared Feedback details.
    /// </summary>
    public class FeedbackEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets new hire Azure Active Directory object id.
        /// </summary>
        public string NewHireAadObjectId { get; set; }

        /// <summary>
        /// Gets or sets Feedback.
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        /// Gets or sets batch id of the month.
        /// </summary>
        public string BatchId
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
        /// Gets or sets id of the feedback.
        /// </summary>
        public string Id
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
        /// Gets or sets new hire name.
        /// </summary>
        public string NewHireName { get; set; }

        /// <summary>
        /// Gets or sets survey submission date and time.
        /// </summary>
        public DateTime? SubmittedOn { get; set; }
    }
}
