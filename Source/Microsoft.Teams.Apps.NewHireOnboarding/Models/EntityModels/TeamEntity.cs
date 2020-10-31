// <copyright file="TeamEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels
{
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Class contains team details where application is installed.
    /// </summary>
    public class TeamEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets team id where application is installed.
        /// </summary>
        public string TeamId
        {
            get
            {
                return this.PartitionKey;
            }

            set
            {
                this.PartitionKey = value;
                this.RowKey = value;
            }
        }

        /// <summary>
        /// Gets or sets service URL.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets name of team where bot installed.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory id of the user who installed the application.
        /// </summary>
        public string InstalledByAadObjectId { get; set; }

        /// <summary>
        /// Gets or sets group id of team.
        /// </summary>
        public string AadGroupId { get; set; }
    }
}
