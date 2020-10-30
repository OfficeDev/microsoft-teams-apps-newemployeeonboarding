// <copyright file="TeamDetail.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A class that represents team mapping.
    /// </summary>
    public class TeamDetail
    {
        /// <summary>
        /// Gets or sets team id.
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// Gets or sets team name.
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// Gets or sets list of channels.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        public List<ChannelDetail> Channels { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
    }
}
