// <copyright file="IWelcomeCardFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using Microsoft.Bot.Schema;

    /// <summary>
    /// This interface will contain the welcome card attachment methods.
    /// </summary>
    public interface IWelcomeCardFactory
    {
        /// <summary>
        /// This method will construct the new hire welcome card when bot is added in personal scope.
        /// </summary>
        /// <returns>New hire welcome card attachment.</returns>
        Attachment GetNewHireWelcomeCard();

        /// <summary>
        /// This method will construct the team welcome card when bot is added in team scope.
        /// </summary>
        /// <returns>Team welcome card attachment.</returns>
        Attachment GetTeamWelcomeCard();

        /// <summary>
        /// This method will construct the hiring manager card when bot is added in personal scope.
        /// </summary>
        /// <returns>Hiring manager welcome card attachment.</returns>
        Attachment GetHiringManagerWelcomeCard();

        /// <summary>
        /// This method will construct the HR welcome card when bot is added in team scope.
        /// </summary>
        /// <returns>Human resource welcome card attachment.</returns>
        Attachment GetHumanResourceWelcomeCard();
    }
}