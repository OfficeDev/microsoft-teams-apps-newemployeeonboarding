// <copyright file="PolicyNames.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
namespace Microsoft.Teams.Apps.NewHireOnboarding.Authentication
{
    /// <summary>
    /// This class lists the names of the custom authorization policies in the project.
    /// </summary>
    public static class PolicyNames
    {
        /// <summary>
        /// The name of the authorization policy, MustBeHumanResourceTeamMemberUserPolicy.
        /// Indicates that user is a part of human resource team and has permission to view and download feedback reports.
        /// </summary>
        public const string MustBeHumanResourceTeamMemberUserPolicy = "MustBeHumanResourceTeamMemberUserPolicy";
    }
}
