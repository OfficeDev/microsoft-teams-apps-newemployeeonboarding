// <copyright file="ActivityHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Cards;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Implements the methods that are defined in <see cref="IActivityHelper"/>.
    /// The class that represent the helper methods for activity handler.
    /// </summary>
    /// <typeparam name="T">Generic class.</typeparam>
    public class ActivityHelper<T> : IActivityHelper
        where T : Dialog
    {
        /// <summary>
        /// Instance to send logs to the Application Insights service.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Provider for fetching information about team details from storage.
        /// </summary>
        private readonly ITeamStorageProvider teamStorageProvider;

        /// <summary>
        /// State management object for maintaining user conversation state.
        /// </summary>
        private readonly BotState userState;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Helper for team operations with Microsoft Graph API.
        /// </summary>
        private readonly ITeamMembership teamOperationGraphApiHelper;

        /// <summary>
        /// Helper for user profile operations with Microsoft Graph API.
        /// </summary>
        private readonly IUserProfile userProfileGraphApiHelper;

        /// <summary>
        /// Provider for fetching information about new hire introduction details from storage.
        /// </summary>
        private readonly IIntroductionStorageProvider introductionStorageProvider;

        /// <summary>
        /// Helper for working with SharePoint API.
        /// </summary>
        private readonly ISharePointHelper sharePointHelper;

        /// <summary>
        /// Instance to work with Microsoft Graph methods.
        /// </summary>
        private readonly IGraphUtilityHelper graphUtility;

        /// <summary>
        /// Factory for working with welcome card attachments.
        /// </summary>
        private readonly IWelcomeCardFactory welcomeCardFactory;

        /// <summary>
        /// A set of key/value application configuration properties for bot settings.
        /// </summary>
        private readonly IOptions<BotOptions> botOptions;

        /// <summary>
        /// Provider for fetching information about user details from storage.
        /// </summary>
        private readonly IUserStorageProvider userStorageProvider;

        /// <summary>
        /// Helper for working with introduction cards.
        /// </summary>
        private readonly IIntroductionCardHelper introductionCardHelper;

        /// <summary>
        /// A set of key/value application configuration properties for AAD security group settings.
        /// </summary>
        private readonly IOptions<AadSecurityGroupSettings> securityGroupSettings;

        /// <summary>
        /// Base class for all bot dialogs.
        /// </summary>
        private readonly Dialog dialog;

        /// <summary>
        /// State management object for maintaining conversation state.
        /// </summary>
        private readonly BotState conversationState;

        /// <summary>
        /// Provider for fetching information about feedback from storage.
        /// </summary>
        private readonly IFeedbackProvider feedbackProvider;

        /// <summary>
        /// Provider for uploading user image to blob storage.
        /// </summary>
        private readonly IImageUploadProvider imageUploadProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityHelper{T}"/> class.
        /// </summary>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        /// <param name="userState">State management object for maintaining user conversation state.</param>
        /// <param name="teamStorageProvider">Provider for fetching information about team details from storage.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="dialog">Base class for all bot dialogs.</param>
        /// <param name="conversationState">State management object for maintaining conversation state.</param>
        /// <param name="teamMembershipHelper">Helper for team operations with Microsoft Graph API.</param>
        /// <param name="userProfileGraphApiHelper">Helper for user profile operations with Microsoft Graph API.</param>
        /// <param name="introductionStorageProvider">Provider for fetching information about new joiner introduction details from storage.</param>
        /// <param name="sharePointHelper">Helper for working with SharePoint.</param>
        /// <param name="introductionCardHelper">Helper for working with introduction cards.</param>
        /// <param name="graphUtility">Instance of Microsoft Graph utility helper.</param>
        /// <param name="welcomeCardFactory">Factory for working with welcome card attachments.</param>
        /// <param name="botOptions">A set of key/value application configuration properties for bot.</param>
        /// <param name="userStorageProvider">Provider for fetching information about user details from storage.</param>
        /// <param name="securityGroupSettings"> A set of key/value application configuration properties for AAD security group settings.</param>
        /// <param name="feedbackProvider">Provider for fetching information about new joiner feedbacks from storage.</param>
        /// <param name="imageUploadProvider">Provider for uploading user image to blob storage.</param>
        public ActivityHelper(
            ILogger<ActivityHelper<T>> logger,
            UserState userState,
            ITeamStorageProvider teamStorageProvider,
            IStringLocalizer<Strings> localizer,
            T dialog,
            ConversationState conversationState,
            ITeamMembership teamMembershipHelper,
            IUserProfile userProfileGraphApiHelper,
            IIntroductionStorageProvider introductionStorageProvider,
            ISharePointHelper sharePointHelper,
            IIntroductionCardHelper introductionCardHelper,
            IGraphUtilityHelper graphUtility,
            IWelcomeCardFactory welcomeCardFactory,
            IOptions<BotOptions> botOptions,
            IUserStorageProvider userStorageProvider,
            IOptions<AadSecurityGroupSettings> securityGroupSettings,
            IFeedbackProvider feedbackProvider,
            IImageUploadProvider imageUploadProvider)
        {
            this.logger = logger;
            this.userState = userState;
            this.teamStorageProvider = teamStorageProvider;
            this.localizer = localizer;
            this.dialog = dialog;
            this.conversationState = conversationState;
            this.teamOperationGraphApiHelper = teamMembershipHelper;
            this.userProfileGraphApiHelper = userProfileGraphApiHelper;
            this.introductionStorageProvider = introductionStorageProvider;
            this.sharePointHelper = sharePointHelper;
            this.introductionCardHelper = introductionCardHelper;
            this.graphUtility = graphUtility;
            this.welcomeCardFactory = welcomeCardFactory;
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.userStorageProvider = userStorageProvider;
            this.securityGroupSettings = securityGroupSettings ?? throw new ArgumentNullException(nameof(securityGroupSettings));
            this.feedbackProvider = feedbackProvider;
            this.imageUploadProvider = imageUploadProvider;
        }

        /// <summary>
        /// Get new hire introduction card.
        /// </summary>
        /// <param name="userGraphAccessToken">User access token.</param>
        /// <param name="turnContext">Provides context for a step in a bot dialog.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that returns introduction card attachment as task module response.</returns>
        public async Task<TaskModuleResponse> GetIntroductionAsync(
            string userGraphAccessToken,
            ITurnContext turnContext,
            CancellationToken cancellationToken)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
            var userDetails = await this.GetUserDetailAsync(turnContext, cancellationToken);

            // Get Manager details from Microsoft Graph API.
            var myManager = await this.userProfileGraphApiHelper.GetMyManagerAsync(userGraphAccessToken);

            if (myManager == null)
            {
                this.logger.LogWarning($"Error in getting manager details from Microsoft Graph API for user {turnContext.Activity.From.Id}.");
                await turnContext.SendActivityAsync(this.localizer.GetString("GenericErrorMessageText"));

                return null;
            }

            var introductionEntity = await this.introductionStorageProvider.GetIntroductionDetailAsync(userDetails.AadObjectId, myManager.Id);

            if (introductionEntity == null)
            {
                // Get Microsoft Graph token response.
                var response = await this.graphUtility.ObtainApplicationTokenAsync(
                    this.botOptions.Value.TenantId,
                    this.botOptions.Value.MicrosoftAppId,
                    this.botOptions.Value.MicrosoftAppPassword);

                // Check if Microsoft Graph token is null.
                if (response == null)
                {
                    this.logger.LogInformation("Response obtained from Microsoft Graph for access taken is null.");

                    return null;
                }

                var introductionQuestions = await this.sharePointHelper.GetIntroductionQuestionsAsync(response.AccessToken);

                if (introductionQuestions == null)
                {
                    return null;
                }

                var profileNote = await this.userProfileGraphApiHelper.GetUserProfileNoteAsync(userGraphAccessToken, userDetails.AadObjectId);
                introductionEntity = new IntroductionEntity
                {
                    NewHireAadObjectId = userDetails.AadObjectId,
                    ManagerAadObjectId = myManager.Id,
                    NewHireQuestionnaire = JsonConvert.SerializeObject(introductionQuestions),
                    ApprovalStatus = (int)IntroductionStatus.PendingForApproval,
                    Comments = null,
                    NewHireName = userDetails.Name,
                    ManagerConversationId = string.Empty,
                    NewHireUserPrincipalName = userDetails.Email,
                    NewHireConversationId = userDetails.Id,
                    ApprovedOn = null,
                    NewHireProfileNote = profileNote,
                };

                return this.introductionCardHelper.GetNewHireIntroductionCard(introductionEntity);
            }
            else if (introductionEntity != null && introductionEntity.ApprovalStatus != (int)IntroductionStatus.TellMeMore)
            {
                return this.introductionCardHelper.GetIntroductionValidationCard(introductionEntity);
            }
            else
            {
                return this.introductionCardHelper.GetNewHireIntroductionCard(introductionEntity);
            }
        }

        /// <summary>
        /// Show approve introduction card details.
        /// </summary>
        /// <param name="userGraphAccessToken">User access token.</param>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <returns>A task that returns approve introduction card attachment as task module response.</returns>
        public async Task<TaskModuleResponse> ApproveIntroductionActionAsync(
            string userGraphAccessToken,
            ITurnContext turnContext)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));

            var activity = turnContext.Activity;
            var postedValues = JsonConvert.DeserializeObject<BotCommand>(((JObject)activity.Value).GetValue("data", StringComparison.OrdinalIgnoreCase).ToString());

            var result = await this.introductionStorageProvider.GetIntroductionDetailAsync(
                                postedValues.IntroductionEntity.NewHireAadObjectId,
                                postedValues.IntroductionEntity.ManagerAadObjectId);

            if (result.ApprovalStatus == (int)IntroductionStatus.Approved)
            {
                return this.introductionCardHelper.GetValidationErrorCard(this.localizer.GetString("ManagerApprovalValidationText"));
            }

            List<Models.TeamDetail> teamChannelMapping = await this.GetTeamMappingDetailsAsync(turnContext, userGraphAccessToken);

            if (teamChannelMapping == null)
            {
                return this.introductionCardHelper.GetValidationErrorCard(this.localizer.GetString("BotNotInstalledInTeamMessageText"));
            }

            this.logger.LogInformation($"Introduction approved by manager: {postedValues.IntroductionEntity.ManagerAadObjectId}");
            return this.introductionCardHelper.GetApproveDetailCard(teamChannelMapping, postedValues.IntroductionEntity);
        }

        /// <summary>
        /// Submit introduction card action.
        /// </summary>
        /// <param name="userGraphAccessToken">User access token.</param>
        /// <param name="turnContext">Provides context for a step in a bot dialog.</param>
        /// <param name="taskModuleRequest">Task module invoke request value payload.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that returns submitted introduction card attachment as task module response.</returns>
        public async Task<TaskModuleResponse> SubmitIntroductionActionAsync(
            string userGraphAccessToken,
            ITurnContext<IInvokeActivity> turnContext,
            TaskModuleRequest taskModuleRequest,
            CancellationToken cancellationToken)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
            taskModuleRequest = taskModuleRequest ?? throw new ArgumentNullException(nameof(taskModuleRequest));

            var userDetails = await this.GetUserDetailAsync(turnContext, cancellationToken);
            IntroductionEntity introductionEntity = await this.GenerateIntroductionEntityAsync(
                turnContext,
                taskModuleRequest,
                userDetails);

            var questionnaire = JsonConvert.DeserializeObject<List<IntroductionDetail>>(introductionEntity.NewHireQuestionnaire);

            if (!questionnaire.Where(row => string.IsNullOrEmpty(row.Answer)).ToList().Any()
                && !string.IsNullOrWhiteSpace(introductionEntity.NewHireProfileNote))
            {
                // Get Manager details from Microsoft Graph API.
                var myManager = await this.userProfileGraphApiHelper.GetMyManagerAsync(userGraphAccessToken);

                if (myManager == null)
                {
                    this.logger.LogWarning($"Error in getting manager details from Microsoft Graph API for user {turnContext.Activity.From.Id}.");
                    await turnContext.SendActivityAsync(this.localizer.GetString("UserNotMappedWithManagerMessageText"));

                    return null;
                }

                // Get Manager's conversation id from storage.
                var userConversationDetails = await this.userStorageProvider.GetUserDetailAsync(myManager.Id);

                if (userConversationDetails == null)
                {
                    this.logger.LogWarning($"Error in getting user conversation details from storage for user {myManager.Id}.");
                    await turnContext.SendActivityAsync(this.localizer.GetString("ManagerUnavailableText"));

                    return null;
                }

                introductionEntity.ManagerAadObjectId = myManager.Id;
                introductionEntity.ManagerConversationId = userConversationDetails.ConversationId;

                await this.introductionStorageProvider.StoreOrUpdateIntroductionDetailAsync(introductionEntity);
                await turnContext.SendActivityAsync(this.localizer.GetString("IntroSubmittedMessage"));
                this.logger.LogInformation($"Introduction submitted by: {turnContext.Activity.From.Id}.");

                var hiringMangerNotification = MessageFactory.Attachment(HiringManagerNotificationCard.GetNewEmployeeIntroductionCard(this.botOptions.Value.AppBaseUri, this.localizer, introductionEntity));
                hiringMangerNotification.Conversation = new ConversationAccount { Id = introductionEntity.ManagerConversationId };
                await turnContext.Adapter.SendActivitiesAsync(turnContext, new Activity[] { (Activity)hiringMangerNotification }, cancellationToken);

                return null;
            }
            else
            {
                // send back introduction card with corresponding validation message.
                return this.introductionCardHelper.GetNewHireIntroductionCard(introductionEntity, isAllQuestionAnswered: false);
            }
        }

        /// <summary>
        /// Get team mapping details.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="userGraphAccessToken">User access token.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task<List<Models.TeamDetail>> GetTeamMappingDetailsAsync(
          ITurnContext turnContext,
          string userGraphAccessToken)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));

            // Get teams where application is installed.
            var allTeams = await this.teamStorageProvider.GetAllTeamDetailAsync();

            if (allTeams == null || allTeams.Count == 0)
            {
                this.logger.LogWarning("Error in getting team details from storage.");
                return null;
            }

            // Get all teams where manager is a member.
            var myJoinedTeams = await this.teamOperationGraphApiHelper.GetMyJoinedTeamsAsync(userGraphAccessToken);

            if (myJoinedTeams == null || myJoinedTeams.Count == 0)
            {
                this.logger.LogWarning("Error in getting team details from Microsoft Graph API.");
                return null;
            }

            // Filter out teams where bot is not installed.
            var joinedTeamsWhereBotInstalled = allTeams.Where(row => myJoinedTeams.Select(team => team.Id).Contains(row.AadGroupId)).ToList();

            if (joinedTeamsWhereBotInstalled == null || joinedTeamsWhereBotInstalled.Count == 0)
            {
                this.logger.LogWarning("Error in getting team details from Microsoft Graph API.");
                return null;
            }

            var teamChannelMapping = new List<Models.TeamDetail>();

            foreach (var team in joinedTeamsWhereBotInstalled)
            {
                teamChannelMapping.Add(new Models.TeamDetail() { TeamId = team.AadGroupId, TeamName = team.Name });
            }

            foreach (var team in teamChannelMapping)
            {
                // Get team and channel mapping to post introduction notification.
                var channelDetails = await this.teamOperationGraphApiHelper.GetChannelsAsync(userGraphAccessToken, team.TeamId);

                if (channelDetails != null)
                {
                    var channels = new List<ChannelDetail>();
                    foreach (var channel in channelDetails)
                    {
                        channels.Add(new ChannelDetail() { ChannelId = channel.Id, ChannelName = channel.DisplayName });
                    }

                    team.Channels = channels;
                }
            }

            return teamChannelMapping;
        }

        /// <summary>
        /// Method to send welcome card once Bot is installed in personal/team scope.
        /// </summary>
        /// <param name="membersAdded">A list of all the members added to the conversation, as described by the conversation update activity.</param>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns a task.</returns>
        public async Task SendWelcomeNotificationAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var activity = turnContext?.Activity;

            if (membersAdded.Any(member => member.Id == activity.Recipient.Id) && activity.Conversation.ConversationType == CardConstants.PersonalConversationType)
            {
                await this.SendPersonalNotificationAsync(turnContext, cancellationToken);
            }

            // Check it is not the member that is adding to the conversation.
            else if (membersAdded.Any(member => member.AadObjectId == null) && activity.Conversation.ConversationType == CardConstants.ChannelConversationType)
            {
                // Storing team information to storage
                var teamsDetails = activity.TeamsGetTeamInfo();
                TeamEntity teamEntity = new TeamEntity
                {
                    TeamId = teamsDetails.Id,
                    ServiceUrl = turnContext.Activity.ServiceUrl,
                    Name = teamsDetails.Name,
                    InstalledByAadObjectId = turnContext.Activity.From.AadObjectId,
                    AadGroupId = teamsDetails.AadGroupId,
                };

                // Check whether the team id is human resource manager team.
                var teamWelcomeCardAttachment = this.botOptions.Value.HumanResourceTeamId == teamsDetails.Id ? this.welcomeCardFactory.GetHumanResourceWelcomeCard()
                    : this.welcomeCardFactory.GetTeamWelcomeCard();
                await this.teamStorageProvider.StoreOrUpdateTeamDetailAsync(teamEntity);
                await turnContext.SendActivityAsync(MessageFactory.Attachment(teamWelcomeCardAttachment));
            }
        }

        /// <summary>
        /// Method to request more information details card from new hire.
        /// </summary>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <param name="valuesfromCard">Values from card.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Request more information notification card.</returns>
        public async Task RequestMoreInfoActionAsync(ITurnContext<IMessageActivity> turnContext, AdaptiveSubmitActionData valuesfromCard, CancellationToken cancellationToken)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
            valuesfromCard = valuesfromCard ?? throw new ArgumentNullException(nameof(valuesfromCard));

            if (string.IsNullOrEmpty(valuesfromCard.Comments))
            {
                await turnContext.SendActivityAsync(this.localizer.GetString("TellMeMoreInputValidationText"));

                return;
            }

            var introduction = await this.introductionStorageProvider.GetIntroductionDetailAsync(
                valuesfromCard.IntroductionEntity.NewHireAadObjectId,
                valuesfromCard.IntroductionEntity.ManagerAadObjectId);

            if (introduction.ApprovalStatus == (int)IntroductionStatus.Approved)
            {
                await turnContext.SendActivityAsync(this.localizer.GetString("ManagerApprovalValidationText"));
            }
            else
            {
                valuesfromCard.IntroductionEntity.Comments = valuesfromCard.Comments;
                valuesfromCard.IntroductionEntity.ApprovalStatus = (int)IntroductionStatus.TellMeMore;
                await this.introductionStorageProvider.StoreOrUpdateIntroductionDetailAsync(valuesfromCard.IntroductionEntity);
                var newHireNotification = MessageFactory.Attachment(TellMeMoreCard.GetCard(this.botOptions.Value.AppBaseUri, this.localizer, valuesfromCard.IntroductionEntity));
                newHireNotification.Conversation = new ConversationAccount { Id = valuesfromCard.IntroductionEntity.NewHireConversationId };
                await turnContext.Adapter.SendActivitiesAsync(turnContext, new Activity[] { (Activity)newHireNotification }, cancellationToken);
                await turnContext.SendActivityAsync(this.localizer.GetString("RequestMoreInfoNotificationText"));
            }
        }

        /// <summary>
        /// Method to submit new hire feedback to storage.
        /// </summary>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <returns>Notification message after successful storing of feedback.</returns>
        public async Task SubmitFeedbackAsync(ITurnContext<IMessageActivity> turnContext)
        {
            var activity = turnContext?.Activity;

            var feedbackText = JObject.Parse(activity.Value.ToString()).Properties().Where(row => row.Name == CardConstants.FeedbackTextInputId).ToList().First()?.Value?.ToString();

            if (string.IsNullOrWhiteSpace(feedbackText))
            {
                IMessageActivity updateCard = MessageFactory.Attachment(FeedbackCard.GetFeedbackCardAttachment(this.localizer, isErrorMessageVisible: true));
                updateCard.Id = activity.ReplyToId;
                await turnContext.UpdateActivityAsync(updateCard);

                return;
            }

            var currentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.UtcNow.Month);

            FeedbackEntity feedbackEntity = new FeedbackEntity
            {
                NewHireAadObjectId = turnContext.Activity.From.AadObjectId,
                Feedback = feedbackText,
                Id = Guid.NewGuid().ToString(),
                BatchId = $"{currentMonth.Substring(0, 3)}_{DateTime.UtcNow.Year}",
                NewHireName = turnContext.Activity.From.Name,
                SubmittedOn = DateTime.UtcNow,
            };

            await this.feedbackProvider.StoreOrUpdateFeedbackAsync(feedbackEntity);
            this.logger.LogInformation($"Feedback submitted by userId: {turnContext.Activity.From.AadObjectId}");
            await turnContext.SendActivityAsync(this.localizer.GetString("FeedbackSuccessMessageText"));
        }

        /// <summary>
        /// Method to update matches status to storage.
        /// </summary>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <param name="command">Command text from bot.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Notification message after successful updating of status in storage.</returns>
        public async Task GetUpdatedMatchesStatusAsync(ITurnContext<IMessageActivity> turnContext, string command, CancellationToken cancellationToken)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
            command = command ?? throw new ArgumentNullException(nameof(command));

            // Command to resume all matches.
            if (command.Equals(BotCommandConstants.ResumeAllMatches, StringComparison.InvariantCultureIgnoreCase))
            {
                var userAadId = turnContext.Activity.From.AadObjectId;
                this.logger.LogInformation($"Updating user choice for resuming matches in storage for userId: {userAadId}");
                var userEntity = await this.userStorageProvider.GetUserDetailAsync(userAadId);
                userEntity.OptedIn = true;
                var operationResult = await this.userStorageProvider.StoreOrUpdateUserDetailAsync(userEntity);

                if (!operationResult)
                {
                    await turnContext.SendActivityAsync(this.localizer.GetString("ErrorInUpdatingUserChoice"), cancellationToken: cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync(this.localizer.GetString("PausedMatchesCardContent"), cancellationToken: cancellationToken);
                }
            }

            // Command to pause all matches.
            else
            {
                var userId = turnContext.Activity.From.AadObjectId;
                this.logger.LogInformation($"Sending resume all matches card and updating user choice for pausing matches in storage for userId: {userId}");
                var opteduserDetail = await this.userStorageProvider.GetUserDetailAsync(userId);
                if (opteduserDetail != null)
                {
                    opteduserDetail.OptedIn = false;
                    var operationStatus = await this.userStorageProvider.StoreOrUpdateUserDetailAsync(opteduserDetail);
                    if (operationStatus)
                    {
                        var resumeMatchesCardAttachment = MessageFactory.Attachment(ResumeMatchesCard.GetResumeMatchesCard(this.localizer));
                        await turnContext.SendActivityAsync(resumeMatchesCardAttachment, cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(this.localizer.GetString("ErrorInUpdatingUserChoice"), cancellationToken: cancellationToken);
                    }
                }
            }
        }

        /// <summary>
        /// Method to send welcome card in personal scope once Bot is installed.
        /// </summary>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Welcome card  when bot is added first time by user.</returns>
        private async Task SendPersonalNotificationAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var activity = turnContext?.Activity;
            var userStateAccessors = this.userState.CreateProperty<UserConversationState>(nameof(UserConversationState));
            var userConversationState = await userStateAccessors.GetAsync(turnContext, () => new UserConversationState());
            if (userConversationState.IsWelcomeCardSent)
            {
                return;
            }

            // Get Microsoft Graph token response.
            var response = await this.graphUtility.ObtainApplicationTokenAsync(
                this.botOptions.Value.TenantId,
                this.botOptions.Value.MicrosoftAppId,
                this.botOptions.Value.MicrosoftAppPassword);

            // get members from security group
            var securityGroupMembers = await this.teamOperationGraphApiHelper.GetGroupMemberIdsAsync(response.AccessToken, this.securityGroupSettings.Value.Id);

            if (securityGroupMembers == null)
            {
                await turnContext.SendActivityAsync(this.localizer.GetString("ErrorMessage"));
                return;
            }

            bool isNewHireEmployee = securityGroupMembers.Contains(turnContext.Activity.From.AadObjectId);

            // Check user role from Graph API, based on role send the welcome card.
            var userWelcomeCardAttachment = isNewHireEmployee ? this.welcomeCardFactory.GetNewHireWelcomeCard()
                : this.welcomeCardFactory.GetHiringManagerWelcomeCard();

            var userDetails = await this.GetUserDetailAsync(turnContext, cancellationToken);

            // upload image to storage
            var imageStream = await this.userProfileGraphApiHelper.GetUserPhotoAsync(response.AccessToken, userDetails.AadObjectId);
            string imageUrl = string.Empty;
            if (imageStream != null && imageStream.Length > 0)
            {
                imageUrl = await this.imageUploadProvider.UploadImageAsync(imageStream, userDetails.AadObjectId);
            }

            // Update user details to storage.
            // Get user information if already exists in storage.
            UserEntity userEntity = await this.userStorageProvider.GetUserDetailAsync(userDetails.AadObjectId);
            if (userEntity == null)
            {
                userEntity = new UserEntity
                {
                    AadObjectId = userDetails.AadObjectId,
                    ConversationId = activity.Conversation.Id,
                    BotInstalledOn = DateTime.UtcNow,
                    ServiceUrl = activity.ServiceUrl,
                    UserRole = isNewHireEmployee ? (int)UserRole.NewHire : (int)UserRole.HiringManager,
                    UserPrincipalName = userDetails.UserPrincipalName,
                    Name = userDetails.Name,
                    UserProfileImageUrl = string.IsNullOrEmpty(imageUrl) ? null : imageUrl,
                    OptedIn = true,
                };
            }

            // Update existing records with additional details.
            else
            {
                userEntity.ConversationId = activity.Conversation.Id;
                userEntity.ServiceUrl = activity.ServiceUrl;
                userEntity.BotInstalledOn = DateTime.UtcNow;
                userEntity.UserRole = isNewHireEmployee ? (int)UserRole.NewHire : (int)UserRole.HiringManager;
                userEntity.UserPrincipalName = userDetails.UserPrincipalName;
                userEntity.Name = userDetails.Name;
                userEntity.UserProfileImageUrl = string.IsNullOrEmpty(imageUrl) ? null : imageUrl;
                userEntity.OptedIn = true;
            }

            await this.userStorageProvider.StoreOrUpdateUserDetailAsync(userEntity);
            await turnContext.SendActivityAsync(MessageFactory.Attachment(userWelcomeCardAttachment));
            userConversationState.IsWelcomeCardSent = true;
            await userStateAccessors.SetAsync(turnContext, userConversationState);
            await this.dialog.RunAsync(turnContext, this.conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        /// <summary>
        /// To generate the introduction entity.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="taskModuleRequest">Task module invoke request value payload.</param>
        /// <param name="userDetails">Get user details.</param>
        /// <returns>Introduction entity.</returns>
        private async Task<IntroductionEntity> GenerateIntroductionEntityAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, TeamsChannelAccount userDetails)
        {
            var objects = (JObject)taskModuleRequest.Data;
            var count = 0;
            List<IntroductionDetail> questionList;

            // Get Microsoft Graph token response.
            var response = await this.graphUtility.ObtainApplicationTokenAsync(
                this.botOptions.Value.TenantId,
                this.botOptions.Value.MicrosoftAppId,
                this.botOptions.Value.MicrosoftAppPassword);

            // Check if Microsoft Graph token is null.
            if (response == null)
            {
                this.logger.LogInformation("Response obtained from Microsoft Graph for access taken is null.");

                return null;
            }

            questionList = (List<IntroductionDetail>)await this.sharePointHelper.GetIntroductionQuestionsAsync(response.AccessToken);

            var aboutMe = objects.Properties().Where(row => row.Name.Equals(CardConstants.NewHireProfileNoteInputId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()?.Value.ToString();

            // Mapping question and answer based on question's unique id.
            foreach (var question in questionList)
            {
                question.Answer = objects.Properties().Where(row => row.Name == $"{CardConstants.QuestionId}{count}").FirstOrDefault()?.Value.ToString();
                count++;
            }

            IntroductionEntity introductionEntity = new IntroductionEntity
            {
                NewHireAadObjectId = userDetails.AadObjectId,
                ManagerAadObjectId = string.Empty,
                NewHireQuestionnaire = JsonConvert.SerializeObject(questionList),
                ApprovalStatus = (int)IntroductionStatus.PendingForApproval,
                Comments = null,
                NewHireName = userDetails.Name,
                ManagerConversationId = string.Empty,
                NewHireUserPrincipalName = userDetails.Email,
                NewHireConversationId = turnContext.Activity.Conversation.Id,
                ApprovedOn = null,
                NewHireProfileNote = aboutMe,
            };

            return introductionEntity;
        }

        /// <summary>
        /// Get Teams channel account detailing user Azure Active Directory details.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        private async Task<TeamsChannelAccount> GetUserDetailAsync(
          ITurnContext turnContext,
          CancellationToken cancellationToken)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));

            var members = await ((BotFrameworkAdapter)turnContext.Adapter).GetConversationMembersAsync(turnContext, cancellationToken);

            return JsonConvert.DeserializeObject<TeamsChannelAccount>(JsonConvert.SerializeObject(members[0]));
        }
    }
}
