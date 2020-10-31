// <copyright file="PairUpBackgroundServiceSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration
{
    /// <summary>
    /// This class used to set value which are used by pair up meeting background service.
    /// </summary>
    public class PairUpBackgroundServiceSettings
    {
        /// <summary>
        /// Gets or sets delay duration to send next pair-up notification.
        /// </summary>
        public int DelayInPairUpNotificationInDays { get; set; }

        /// <summary>
        /// Gets or sets retention period of new hire employee.
        /// </summary>
        public int NewHireRetentionPeriodInDays { get; set; }
    }
}