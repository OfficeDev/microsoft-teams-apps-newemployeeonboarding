// <copyright file="IntroductionDetail.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models
{
    /// <summary>
    /// A class that represents properties to be parsed from Sharepoint Question and Answer data.
    /// </summary>
    public class IntroductionDetail
    {
        /// <summary>
        /// Gets or sets question text.
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets answer text.
        /// </summary>
        public string Answer { get; set; }
    }
}
