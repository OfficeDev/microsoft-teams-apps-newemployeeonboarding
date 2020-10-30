// <copyright file="IIntroductionCardHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;

    /// <summary>
    /// This interface will contain the introduction card helper methods.
    /// </summary>
    public interface IIntroductionCardHelper
    {
        /// <summary>
        /// Get introduction adaptive card.
        /// </summary>
        /// <param name="introductionEntity">New hire introduction details.</param>
        /// <param name="isAllQuestionAnswered">false if any of the question is not answered.</param>
        /// <returns>Envelope for Task Module Response.</returns>
        TaskModuleResponse GetNewHireIntroductionCard(IntroductionEntity introductionEntity, bool isAllQuestionAnswered = true);

        /// <summary>
        /// Get introduction detail adaptive card for hiring manager's team.
        /// </summary>
        /// <param name="introductionEntity">New hire introduction details.</param>
        /// <returns>Envelope for Task Module Response.</returns>
        TaskModuleResponse GetIntroductionDetailCardForTeam(IntroductionEntity introductionEntity);

        /// <summary>
        /// Gets validation message adaptive card to show in task module.
        /// </summary>
        /// <param name="introductionEntity">New hire introduction details.</param>
        /// <returns>Envelope for Task Module Response.</returns>
        TaskModuleResponse GetIntroductionValidationCard(IntroductionEntity introductionEntity);

        /// <summary>
        /// Get team confirmation adaptive card.
        /// </summary>
        /// <param name="teamChannelMapping">Teams/Channel mappings.</param>
        /// <param name="introductionEntity">New hire introduction details.</param>
        /// <param name="isTeamSelected">false if not team has selected.</param>
        /// <returns>Envelope for Task Module Response.</returns>
        TaskModuleResponse GetApproveDetailCard(List<Models.TeamDetail> teamChannelMapping, IntroductionEntity introductionEntity, bool isTeamSelected = true);

        /// <summary>
        /// Gets validation message details card.
        /// </summary>
        /// <param name="message">Message to show in card as validation.</param>
        /// <returns>Envelope for Task Module Response.</returns>
        TaskModuleResponse GetValidationErrorCard(string message);

        /// <summary>
        /// Get list card for pending review introductions.
        /// </summary>
        /// <param name="introductionEntities">List of introduction entities.</param>
        /// <param name="userGraphAccessToken">User access token.</param>
        /// <returns>Review introduction list card attachment.</returns>
        Task<Attachment> GetReviewIntroductionListCardAsync(
            IEnumerable<IntroductionEntity> introductionEntities,
            string userGraphAccessToken);
    }
}