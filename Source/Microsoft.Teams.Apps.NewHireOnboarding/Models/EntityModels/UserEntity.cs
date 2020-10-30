// <copyright file="UserEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Class contains user details.
    /// </summary>
    public class UserEntity : TableEntity
    {
        /// <summary>
        /// Constant partition key value.
        /// </summary>
        public const string UsersPartitionKey = "Users";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserEntity"/> class.
        /// </summary>
        public UserEntity()
        {
            this.PartitionKey = UsersPartitionKey;
        }

        /// <summary>
        /// Gets or sets Azure Active Directory id of the user who installed the application.
        /// </summary>
        public string AadObjectId
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
        /// Gets or sets service URL.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets conversation id for 1:1 chat with user.
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// Gets or sets user role.
        /// </summary>
        public int UserRole { get; set; }

        /// <summary>
        /// Gets or sets date time when user install the application.
        /// </summary>
        public DateTime? BotInstalledOn { get; set; }

        /// <summary>
        /// Gets or sets user profile image url.
        /// </summary>
        public string UserProfileImageUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is opted in to pair-ups.
        /// </summary>
        public bool OptedIn { get; set; }

        /// <summary>
        ///  Gets or sets name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets unique user principal name.
        /// </summary>
        public string UserPrincipalName { get; set; }
    }
}
