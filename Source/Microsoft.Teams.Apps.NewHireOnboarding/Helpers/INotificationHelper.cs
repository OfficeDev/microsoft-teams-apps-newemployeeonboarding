// <copyright file="INotificationHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for notification helper, which helps in sending list card notification on Monthly/Weekly basis as per the configured preference to different users.
    /// </summary>
    public interface INotificationHelper
    {
        /// <summary>
        /// Send survey notification to new hire on Weekly basis in a batch.
        /// </summary>
        /// <returns>A task that represents that sends notification to new hire sends successfully.</returns>
        Task<bool> SendSurveyNotificationToNewHireAsync();

        /// <summary>
        /// Send feedback notification to hiring manager in team on Monthly basis as per the configuration.
        /// </summary>
        /// <returns>A task that represents whether feedback notification to hiring manager sends successfully.</returns>
        Task<bool> SendFeedbackNotificationInChannelAsync();
    }
}
