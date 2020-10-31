// <copyright file="MainDialog.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Dialogs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Main Dialog to provide user signin dialog prompt.
    /// </summary>
    public class MainDialog : LogoutDialog
    {
        /// <summary>
        /// Command Text.
        /// </summary>
        private const string CommandText = "command";

        /// <summary>
        /// Instance to send logs to the Application Insights service.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainDialog"/> class.
        /// </summary>
        /// <param name="optionsAccessor">A set of key/value application configuration properties for AADv1 connection name.</param>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        /// <param name="localizer">The current cultures' string localizer.</param>
        public MainDialog(
            IOptions<TokenSettings> optionsAccessor,
            ILogger<MainDialog> logger,
            IStringLocalizer<Strings> localizer)
            : base(nameof(MainDialog), optionsAccessor?.Value.ConnectionName, localizer)
        {
            this.logger = logger;
            this.localizer = localizer;

            this.AddDialog(new OAuthPrompt(
                nameof(OAuthPrompt),
                new OAuthPromptSettings
                {
                    ConnectionName = optionsAccessor.Value.ConnectionName,
                    Text = this.localizer.GetString("SignInHeaderText"),
                    Title = this.localizer.GetString("SignInButtonText"),
                    Timeout = 300000, // User has 5 minutes to login (1000 * 60 * 5)
                }));

            this.AddDialog(new WaterfallDialog(
                nameof(WaterfallDialog),
                new WaterfallStep[] { this.OAuthPromptStepAsync }));

            // The initial child Dialog to run.
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Initiate prompt for user sign-in.
        /// </summary>
        /// <param name="stepContext">Provides context for a step in a bot dialog.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that reprents token on successfull authentication.</returns>
        private async Task<DialogTurnResult> OAuthPromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activity = stepContext.Context.Activity;
            stepContext.Values[CommandText] = activity.Text?.Trim();

            if (activity.Text == null && activity.Value != null && activity.Type == ActivityTypes.Message)
            {
                stepContext.Values[CommandText] = JToken.Parse(activity.Value.ToString()).SelectToken(CommandText).ToString();
            }

            this.logger.LogInformation($"Sign-in card is send for conversation id :  {activity.Conversation.Id}.");

            return await stepContext.BeginDialogAsync(nameof(OAuthPrompt), null, cancellationToken);
        }
    }
}
