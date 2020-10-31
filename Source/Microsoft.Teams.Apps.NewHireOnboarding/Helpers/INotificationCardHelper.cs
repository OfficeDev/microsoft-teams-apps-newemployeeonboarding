// <copyright file="INotificationCardHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// This interface will contain notification card method.
    /// </summary>
    public interface INotificationCardHelper
    {
        /// <summary>
        /// Send the given attachment to the specified conversation id.
        /// </summary>
        /// <param name="cardToSend">The attachment card to send.</param>
        /// <param name="conversationId">Conversation id where the notification have to be sent.</param>
        /// <param name="serviceBasePath">Service URL.</param>
        /// <returns>A task that sends notification card.</returns>
        Task SendProActiveNotificationCardAsync(
             Attachment cardToSend,
             string conversationId,
             string serviceBasePath);
    }
}