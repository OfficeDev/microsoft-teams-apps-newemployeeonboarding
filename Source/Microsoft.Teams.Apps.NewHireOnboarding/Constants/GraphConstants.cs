// <copyright file="GraphConstants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Constants
{
    /// <summary>
    /// Microsoft Graph Constants.
    /// </summary>
    public static class GraphConstants
    {
        /// <summary>
        /// Microsoft Graph Beta base url.
        /// </summary>
        public const string BetaBaseUrl = "https://graph.microsoft.com/beta";

        /// <summary>
        /// Max page size.
        /// </summary>
        public const int MaxPageSize = 999;

        /// <summary>
        /// Max retry for Graph API calls.
        /// Note: Max value allowed is 10.
        /// </summary>
        public const int MaxRetry = 2;
    }
}
