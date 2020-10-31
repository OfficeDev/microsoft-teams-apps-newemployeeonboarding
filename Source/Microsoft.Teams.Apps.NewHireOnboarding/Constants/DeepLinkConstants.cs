// <copyright file="DeepLinkConstants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Constants
{
    /// <summary>
    /// A class that holds deep links that are used in multiple files.
    /// </summary>
    public static class DeepLinkConstants
    {
        /// <summary>
        /// Deep link to navigate to Channels Tab.
        /// </summary>
        public const string TabBaseRedirectURL = "https://teams.microsoft.com/l/entity";

        /// <summary>
        /// Deep link to initiate chat.
        /// </summary>
        public const string ChatInitiateURL = "https://teams.microsoft.com/l/chat/0/0";

        /// <summary>
        /// Link that redirects to tab.
        /// </summary>
        public const string FeedbackTabURL = "https://teams.microsoft.com/l/entity/{0}/Feedback?context={1}";

        /// <summary>
        /// Link to open file in teams.
        /// </summary>
        public const string OpenFileInTeamsURL = "https://teams.microsoft.com/_#/{0}/viewer/teams/{1}";

        /// <summary>
        /// Link to open meeting in teams.
        /// </summary>
        public const string MeetingLink = "https://teams.microsoft.com/l/meeting/new?subject=";
    }
}
