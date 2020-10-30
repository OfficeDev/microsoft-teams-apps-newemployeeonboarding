// <copyright file="ActivityHelperTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.Helpers
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Dialogs;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to test activity helper methods.
    /// </summary>
    [TestClass]
    public class ActivityHelperTest
    {
        Mock<IGraphUtilityHelper> graphUtility;
        Mock<ISharePointHelper> sharePointHelper;
        Mock<INotificationCardHelper> notificationCardHelper;
        Mock<IIntroductionCardHelper> introductionCardHelper;
        Mock<IWelcomeCardFactory> welcomeCardFactory;

        Mock<ITeamStorageProvider> teamStorageProvider;
        Mock<IIntroductionStorageProvider> introductionStorageProvider;
        Mock<IUserStorageProvider> userStorageProvider;
        Mock<IFeedbackProvider> feedbackProvider;
        Mock<IImageUploadProvider> imageUploadProvider;

        Mock<ITeamMembership> teamMembership;
        Mock<IUserProfile> userProfile;
        ActivityHelper<MainDialog> activityHelper;
        ActivityHelper<MainDialog> constructor_ArgumentNullException;
        ITurnContext turnContext;
        ITurnContext<IInvokeActivity> turnContextInvokeActivity;
        Mock<ITurnContext> turnContextMock;

        [TestInitialize]
        public void ActivityHelperTestSetup()
        {
            // Mock providers
            teamStorageProvider = new Mock<ITeamStorageProvider>();
            introductionStorageProvider = new Mock<IIntroductionStorageProvider>();
            userStorageProvider = new Mock<IUserStorageProvider>();
            feedbackProvider = new Mock<IFeedbackProvider>();
            imageUploadProvider = new Mock<IImageUploadProvider>();

            // Mock helpers
            sharePointHelper = new Mock<ISharePointHelper>();
            graphUtility = new Mock<IGraphUtilityHelper>();
            notificationCardHelper = new Mock<INotificationCardHelper>();
            introductionCardHelper = new Mock<IIntroductionCardHelper>(); 
            welcomeCardFactory = new Mock<IWelcomeCardFactory>();

            dynamic myexpando = new ExpandoObject();
            myexpando.Data = new ExpandoObject() as dynamic;
            myexpando.Data = new AdaptiveSubmitActionData
            {
                Msteams = new CardAction
                {
                    Type = ActionTypes.MessageBack,
                    Text = BotCommandConstants.RequestMoreInfoAction,
                },
                IntroductionEntity = ActivityHelperData.introductionEntity,
            };

            var botAdapter = new Mock<BotAdapter>();

            turnContext = new TurnContext(
                botAdapter.Object,
                new Activity
                {
                    Value = JsonConvert.SerializeObject(myexpando),
                });

            turnContextInvokeActivity = null;

            teamMembership = new Mock<ITeamMembership>();
            userProfile = new Mock<IUserProfile>();
            var loggerMainDialog = new Mock<ILogger<MainDialog>>().Object;
            var logger = new Mock<ILogger<ActivityHelper<MainDialog>>>().Object;
            var localizer = new Mock<IStringLocalizer<Strings>>().Object;
            turnContextMock = new Mock<ITurnContext>();

            IStorage storage = new AzureBlobStorage(ConfigurationData.storageOptions.Value.ConnectionString, "bot-state");
            UserState userState = new UserState(storage);
            ConversationState conversationState = new ConversationState(storage);

            MainDialog mainDialog = new MainDialog(
                ConfigurationData.tokenOptions,
                loggerMainDialog,
                localizer);

            // Class contructor.
            activityHelper = new ActivityHelper<MainDialog>(
                logger,
                userState,
                teamStorageProvider.Object,
                localizer,
                mainDialog,
                conversationState,
                teamMembership.Object,
                userProfile.Object,
                introductionStorageProvider.Object,
                sharePointHelper.Object,
                introductionCardHelper.Object,
                graphUtility.Object,
                welcomeCardFactory.Object,
                ConfigurationData.botOptions,
                userStorageProvider.Object,
                ConfigurationData.aadSecurityGroupSettings,
                feedbackProvider.Object,
                imageUploadProvider.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ActivityHelperConstructor_ThrowsArgumentNullException()
        {
            var logger = new Mock<ILogger<ActivityHelper<MainDialog>>>().Object;
            var localizer = new Mock<IStringLocalizer<Strings>>().Object;
            turnContextMock = new Mock<ITurnContext>();
            IStorage storage = new AzureBlobStorage(ConfigurationData.storageOptions.Value.ConnectionString, "bot-state");
            UserState userState = new UserState(storage);
            ConversationState conversationState = new ConversationState(storage);
            var loggerMainDialog = new Mock<ILogger<MainDialog>>().Object;
            MainDialog mainDialog = new MainDialog(
                ConfigurationData.tokenOptions,
                loggerMainDialog,
                localizer);

            constructor_ArgumentNullException = new ActivityHelper<MainDialog>(
            logger,
            userState,
            teamStorageProvider.Object,
            localizer,
            mainDialog,
            conversationState,
            teamMembership.Object,
            userProfile.Object,
            introductionStorageProvider.Object,
            sharePointHelper.Object,
            introductionCardHelper.Object,
            graphUtility.Object,
            welcomeCardFactory.Object,
            null,
            userStorageProvider.Object,
            null,
            feedbackProvider.Object,
            imageUploadProvider.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task IntroductionCardAsync_Success()
        {
            this.userProfile
                 .Setup(x => x.GetMyManagerAsync(GraphApiHelperData.AccessToken))
                 .Returns(Task.FromResult(ActivityHelperData.userProfileDetail));

            this.turnContextMock
                 .Setup(x => x.SendActivityAsync(GraphApiHelperData.AccessToken, null, "acceptingInput", new System.Threading.CancellationToken()))
                 .Returns(Task.FromResult(new ResourceResponse()));

            this.introductionStorageProvider
                 .Setup(x => x.GetIntroductionDetailAsync(ActivityHelperData.NewJoinerAadObjectId, ActivityHelperData.HiringManagerAadObjectId))
                 .Returns(Task.FromResult(ActivityHelperData.introductionEntity));

            this.graphUtility
                .Setup(x => x.ObtainApplicationTokenAsync(ConfigurationData.botOptions.Value.TenantId, ConfigurationData.botOptions.Value.MicrosoftAppId, ConfigurationData.botOptions.Value.MicrosoftAppPassword))
                .Returns(Task.FromResult(new Models.Graph.GraphTokenResponse() { AccessToken = GraphApiHelperData.AccessToken }));

            this.sharePointHelper
                 .Setup(x => x.GetIntroductionQuestionsAsync(GraphApiHelperData.AccessToken))
                 .Returns(Task.FromResult(ActivityHelperData.IntroductionQuestions));

            this.userProfile
                 .Setup(x => x.GetUserProfileNoteAsync(GraphApiHelperData.AccessToken, GraphApiHelperData.UserId))
                 .Returns(Task.FromResult("ProfileNote"));

            this.introductionCardHelper
                 .Setup(x => x.GetNewHireIntroductionCard(ActivityHelperData.introductionEntity, true))
                 .Returns(new TaskModuleResponse() { Task = new TaskModuleResponseBase() { Type = "continue" } });

            this.introductionCardHelper
                 .Setup(x => x.GetIntroductionValidationCard(ActivityHelperData.introductionEntity))
                 .Returns(new TaskModuleResponse() { Task = new TaskModuleResponseBase() { Type = "continue" } });

            this.introductionCardHelper
                 .Setup(x => x.GetNewHireIntroductionCard(ActivityHelperData.introductionEntity, true))
                 .Returns(new TaskModuleResponse() { Task = new TaskModuleResponseBase() { Type = "continue" } });

            await this.activityHelper.GetIntroductionAsync(GraphApiHelperData.AccessToken, turnContext, new System.Threading.CancellationToken());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task ApproveIntroductionCardAsync_Success()
        {
            this.introductionStorageProvider
                 .Setup(x => x.GetIntroductionDetailAsync(ActivityHelperData.NewJoinerAadObjectId, ActivityHelperData.HiringManagerAadObjectId))
                 .Returns(Task.FromResult(ActivityHelperData.introductionEntity));

            this.introductionCardHelper
                 .Setup(x => x.GetValidationErrorCard(ActivityHelperData.ValidationErrorCardText))
                 .Returns(new TaskModuleResponse());

            this.teamStorageProvider
                 .Setup(x => x.GetAllTeamDetailAsync())
                 .Returns(Task.FromResult(ActivityHelperData.teamEntities));

            this.teamMembership
                .Setup(x => x.GetMyJoinedTeamsAsync(GraphApiHelperData.AccessToken))
                .Returns(Task.FromResult(ActivityHelperData.teamCollection));

            this.teamMembership
                .Setup(x => x.GetChannelsAsync(GraphApiHelperData.AccessToken, ConfigurationData.TeamId))
                .Returns(Task.FromResult(ActivityHelperData.channelCollection));

            this.introductionCardHelper
                 .Setup(x => x.GetApproveDetailCard(ActivityHelperData.teamDetailCollection, ActivityHelperData.introductionEntity, true))
                 .Returns(new TaskModuleResponse());

            await this.activityHelper.ApproveIntroductionActionAsync(GraphApiHelperData.AccessToken, turnContext);
           
        }

        [TestMethod]
        public async Task TeamMappingDetailsAsync_Success()
        {
            this.teamStorageProvider
                 .Setup(x => x.GetAllTeamDetailAsync())
                 .Returns(Task.FromResult(ActivityHelperData.teamEntities));

            this.teamMembership
                .Setup(x => x.GetMyJoinedTeamsAsync(GraphApiHelperData.AccessToken))
                .Returns(Task.FromResult(ActivityHelperData.teamCollection));

            this.teamMembership
                .Setup(x => x.GetChannelsAsync(GraphApiHelperData.AccessToken, ConfigurationData.TeamId))
                .Returns(Task.FromResult(ActivityHelperData.channelCollection));

            var Result = await this.activityHelper.GetTeamMappingDetailsAsync(turnContext, GraphApiHelperData.AccessToken);
            Assert.AreEqual(Result.ToList().Any(), true);
        }

        [TestMethod]
        public async Task TeamMappingDetailsAsync_Failure()
        {
            this.teamStorageProvider
                 .Setup(x => x.GetAllTeamDetailAsync())
                 .Returns(Task.FromResult(ActivityHelperData.emptyTeamCollection));

            this.teamMembership
                .Setup(x => x.GetMyJoinedTeamsAsync(GraphApiHelperData.AccessToken))
                .Returns(Task.FromResult(ActivityHelperData.teamCollection));

            this.teamMembership
                .Setup(x => x.GetChannelsAsync(GraphApiHelperData.AccessToken, ConfigurationData.TeamId))
                .Returns(Task.FromResult(ActivityHelperData.channelCollection));

            var Result = await this.activityHelper.GetTeamMappingDetailsAsync(turnContext, GraphApiHelperData.AccessToken);
            Assert.AreEqual(Result, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SubmitIntroductionAsync_Success()
        {
            this.userProfile
                 .Setup(x => x.GetMyManagerAsync(GraphApiHelperData.AccessToken))
                 .Returns(Task.FromResult(ActivityHelperData.userProfileDetail));

            this.turnContextMock
                 .Setup(x => x.SendActivityAsync("UserNotMappedWithManagerMessageText", null, "acceptingInput", new System.Threading.CancellationToken()))
                 .Returns(Task.FromResult(new ResourceResponse()));

            this.userStorageProvider
                .Setup(x => x.GetUserDetailAsync(GraphApiHelperData.UserId))
                .Returns(Task.FromResult(ActivityHelperData.userEntity));

            this.turnContextMock
                 .Setup(x => x.SendActivityAsync("ManagerUnavailableText", null, "acceptingInput", new System.Threading.CancellationToken()))
                 .Returns(Task.FromResult(new ResourceResponse()));

            this.introductionStorageProvider
                 .Setup(x => x.StoreOrUpdateIntroductionDetailAsync(ActivityHelperData.introductionEntity))
                 .Returns(Task.FromResult(true));

            var Result = await this.activityHelper.SubmitIntroductionActionAsync(
            GraphApiHelperData.AccessToken,
            turnContextInvokeActivity,
            new TaskModuleRequest(),
            new System.Threading.CancellationToken());
        }
    }
}
