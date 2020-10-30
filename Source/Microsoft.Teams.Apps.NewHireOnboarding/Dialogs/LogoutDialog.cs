// <copyright file="LogoutDialog.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Dialogs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;

    /// <summary>
    /// Dialog for handling interruption.
    /// </summary>
    public class LogoutDialog : ComponentDialog
    {
        /// <summary>
        /// AADv1 bot connection name.
        /// </summary>
        private readonly string connectionName;

        /// <summary>
        /// The current culture string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutDialog"/> class.
        /// </summary>
        /// <param name="id">Dialog Id.</param>
        /// <param name="connectionName">AAD v2 connection name.</param>
        /// <param name="localizer">The current cultures string localizer.</param>
        public LogoutDialog(string id, string connectionName, IStringLocalizer<Strings> localizer)
            : base(id)
        {
            this.connectionName = connectionName;
            this.localizer = localizer;
        }

        /// <summary>
        /// Called when the dialog is started and pushed onto the parent's dialog stack.
        /// </summary>
        /// <param name="innerDc">The inner Microsoft.Bot.Builder.Dialogs.DialogContext for the current turn of conversation.</param>
        /// <param name="options">Optional, initial information to pass to the dialog.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            innerDc = innerDc ?? throw new ArgumentNullException(nameof(innerDc));

            var result = await this.InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        /// <summary>
        /// Called when the dialog is _continued_, where it is the active dialog and the user replies with a new activity.
        /// </summary>
        /// <param name="innerDc">The inner Microsoft.Bot.Builder.Dialogs.DialogContext for the current turn of conversation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            innerDc = innerDc ?? throw new ArgumentNullException(nameof(innerDc));

            var result = await this.InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        /// <summary>
        /// Handling interruption.
        /// </summary>
        /// <param name="innerDc">The inner Microsoft.Bot.Builder.Dialogs.DialogContext for the current turn of conversation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text.Trim().ToUpperInvariant();

                // Allow logout anywhere in the command
                if (text == this.localizer.GetString("LogoutText").ToString().ToUpperInvariant())
                {
                    // The bot adapter encapsulates the authentication processes.
                    var botAdapter = (BotFrameworkAdapter)innerDc.Context.Adapter;
                    await botAdapter.SignOutUserAsync(innerDc.Context, this.connectionName, null, cancellationToken);
                    await innerDc.Context.SendActivityAsync(MessageFactory.Text(this.localizer.GetString("SigoutText")), cancellationToken);
                    return await innerDc.CancelAllDialogsAsync(cancellationToken);
                }
            }

            return null;
        }
    }
}
