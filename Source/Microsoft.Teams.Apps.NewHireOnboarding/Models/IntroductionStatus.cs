// <copyright file="IntroductionStatus.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models
{
    /// <summary>
    /// Enum values for the introduction approval status.
    /// </summary>
    public enum IntroductionStatus
    {
        /// <summary>
        /// PendingForApproval status will be having 0 value.
        /// </summary>
        PendingForApproval = 0,

        /// <summary>
        /// Approved status will be having 1 value.
        /// </summary>
        Approved = 1,

        /// <summary>
        /// TellMeMore status will be having 2 value.
        /// </summary>
        TellMeMore = 2,
    }
}
