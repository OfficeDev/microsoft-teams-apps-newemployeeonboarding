// <copyright file="AdapterWithErrorHandler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Bot
{
    using System;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A class that implements error handler.
    /// </summary>
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdapterWithErrorHandler"/> class.
        /// </summary>
        /// <param name="credentialProvider">Credential provider for bot.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        /// <param name="channelProvider">Framework channel service.</param>
        /// <param name="activityMiddleware">Represents middleware that can operate on incoming activities.</param>
        /// <param name="localizer">The current cultures' string localizer.</param>
        /// <param name="conversationState">A state management object for conversation state.</param>
        public AdapterWithErrorHandler(
            ICredentialProvider credentialProvider,
            ILogger<BotFrameworkHttpAdapter> logger,
            IChannelProvider channelProvider,
            ActivityMiddleware activityMiddleware,
            IStringLocalizer<Strings> localizer,
            ConversationState conversationState = null)
            : base(credentialProvider, channelProvider: channelProvider, logger: logger)
        {
            activityMiddleware = activityMiddleware ?? throw new ArgumentNullException(nameof(activityMiddleware));

            // Add activity middleware to the adapter's middleware pipeline
            this.Use(activityMiddleware);

            this.OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync(localizer.GetString("ErrorMessage"));

                if (conversationState != null)
                {
                    try
                    {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
                        await conversationState.DeleteAsync(turnContext);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Exception caught on attempting to delete conversation state : {ex.Message}");
                        throw;
                    }
                }
            };
        }
    }
}
