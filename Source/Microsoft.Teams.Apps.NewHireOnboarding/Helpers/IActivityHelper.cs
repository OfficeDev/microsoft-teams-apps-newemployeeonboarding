// <copyright file="IActivityHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;

    /// <summary>
    /// This interface will contain the activity handler helper methods for bot activity.
    /// </summary>
    public interface IActivityHelper
    {
        /// <summary>
        /// Get introduction card for new hire.
        /// </summary>
        /// <param name="userGraphAccessToken">User access token.</param>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that returns introduction card attachment as task module response.</returns>
        Task<TaskModuleResponse> GetIntroductionAsync(
            string userGraphAccessToken,
            ITurnContext turnContext,
            CancellationToken cancellationToken);

        /// <summary>
        /// Show approve introduction card details.
        /// </summary>
        /// <param name="userGraphAccessToken">User access token.</param>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <returns>A task that returns approve introduction card attachment as task module response.</returns>
        Task<TaskModuleResponse> ApproveIntroductionActionAsync(
            string userGraphAccessToken,
            ITurnContext turnContext);

        /// <summary>
        /// Submit introduction card actions.
        /// </summary>
        /// <param name="userGraphAccessToken">User access token.</param>
        /// <param name="turnContext">Provides context for a step in a bot dialog.</param>
        /// <param name="taskModuleRequest">Task module invoke request value payload.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that returns submitted introduction card attachment as task module response.</returns>
        Task<TaskModuleResponse> SubmitIntroductionActionAsync(
            string userGraphAccessToken,
            ITurnContext<IInvokeActivity> turnContext,
            TaskModuleRequest taskModuleRequest,
            CancellationToken cancellationToken);

        /// <summary>
        /// Method to send welcome card once Bot is installed in personal/team.
        /// </summary>
        /// <param name="membersAdded">A list of all the members added to the conversation, as described by the conversation update activity.</param>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Welcome card  when bot is added first time by user.</returns>
        Task SendWelcomeNotificationAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken);

        /// <summary>
        /// Get the account details of the user in a 1:1 chat with the bot.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="userGraphAccessToken">User access token.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        Task<List<Models.TeamDetail>> GetTeamMappingDetailsAsync(
           ITurnContext turnContext,
           string userGraphAccessToken);

        /// <summary>
        /// Method to submit new hire feedback to storage.
        /// </summary>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <returns>Notification message after successful storing of feedback.</returns>
        Task SubmitFeedbackAsync(ITurnContext<IMessageActivity> turnContext);

        /// <summary>
        /// Method to update matches status to storage.
        /// </summary>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <param name="command">Command text from bot.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Notification message after successful updating of status in storage.</returns>
        Task GetUpdatedMatchesStatusAsync(ITurnContext<IMessageActivity> turnContext, string command, CancellationToken cancellationToken);

        /// <summary>
        /// Method to request more information details card from new hire.
        /// </summary>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <param name="valuesfromCard">Values from card.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Request more information notification card.</returns>
        Task RequestMoreInfoActionAsync(ITurnContext<IMessageActivity> turnContext, AdaptiveSubmitActionData valuesfromCard, CancellationToken cancellationToken);
    }
}
