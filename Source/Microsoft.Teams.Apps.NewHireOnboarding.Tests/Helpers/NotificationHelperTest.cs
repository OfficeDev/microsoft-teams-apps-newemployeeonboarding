// <copyright file="NotificationHelperTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.Helpers
{
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to test notification helper methods.
    /// </summary>
    [TestClass]
    public class NotificationHelperTest
    {
        Mock<IIntroductionStorageProvider> introductionStorageProvider;
        Mock<IUserStorageProvider> userStorageProvider;
        Mock<ITeamStorageProvider> teamStorageProvider;
        Mock<INotificationCardHelper> cardHelper;
        NotificationHelper notificationHelper;
        NotificationHelper constructor_ArgumentNullException;

        [TestInitialize]
        public void NotificationHelperTestSetup()
        {
            introductionStorageProvider = new Mock<IIntroductionStorageProvider>();
            userStorageProvider = new Mock<IUserStorageProvider>();
            teamStorageProvider = new Mock<ITeamStorageProvider>();
            cardHelper = new Mock<INotificationCardHelper>();
            var logger = new Mock<ILogger<NotificationHelper>>().Object;
            var localizer = new Mock<IStringLocalizer<Strings>>().Object;

            notificationHelper = new NotificationHelper(
                introductionStorageProvider.Object,
                userStorageProvider.Object,
                logger,
                localizer,
                ConfigurationData.botOptions,
                ConfigurationData.sharePointOptions,
               teamStorageProvider.Object,
               cardHelper.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotificationHelper_ThrowsArgumentNullException()
        {
            var logger = new Mock<ILogger<NotificationHelper>>().Object;
            var localizer = new Mock<IStringLocalizer<Strings>>().Object;

            constructor_ArgumentNullException = new NotificationHelper(
            introductionStorageProvider.Object,
            userStorageProvider.Object,
            logger,
            localizer,
            null,
            null,
            teamStorageProvider.Object,
            cardHelper.Object);
        }

        [TestMethod]
        public async Task SurveyNotificationSentSuccessAsync()
        {
            this.introductionStorageProvider
                 .Setup(x => x.GetAllPendingSurveyIntroductionAsync())
                 .Returns(Task.FromResult(NotificationHelperData.introductionEntities));

            this.introductionStorageProvider
                .Setup(x => x.StoreOrUpdateIntroductionDetailAsync(new IntroductionEntity()))
                .Returns(Task.FromResult(true));

            this.cardHelper
                .Setup(x => x.SendProActiveNotificationCardAsync(new Attachment(),"12345","https://www.test.com"))
                .Returns(Task.FromResult(true));

            var Result = await this.notificationHelper.SendSurveyNotificationToNewHireAsync();
            Assert.AreEqual(Result, true);
        }

        [TestMethod]
        public async Task SurveyNotificationSentAsync_Failure()
        {
            this.introductionStorageProvider
                 .Setup(x => x.GetAllPendingSurveyIntroductionAsync())
                 .Returns(Task.FromResult(NotificationHelperData.emptyIntroductionList));

            this.introductionStorageProvider
                .Setup(x => x.StoreOrUpdateIntroductionDetailAsync(new IntroductionEntity()))
                .Returns(Task.FromResult(true));

            this.cardHelper
                .Setup(x => x.SendProActiveNotificationCardAsync(new Attachment(),"12345","https://www.test.com"))
                .Returns(Task.FromResult(true));

            var Result = await this.notificationHelper.SendSurveyNotificationToNewHireAsync();
            Assert.AreEqual(Result, false);
        }

        [TestMethod]
        public async Task FeedbackNotificationSentAsync_Success()
        {
            this.teamStorageProvider
                  .Setup(x => x.GetTeamDetailAsync("12345"))
                  .Returns(Task.FromResult(NotificationHelperData.teamEntity));

            this.cardHelper
                .Setup(x => x.SendProActiveNotificationCardAsync(new Attachment(), "12345", "https://www.test.com"))
                .Returns(Task.FromResult(true));

            var Result = await this.notificationHelper.SendFeedbackNotificationInChannelAsync();
            Assert.AreEqual(Result, true);
        }

        [TestMethod]
        public async Task FeedbackNotificationSentAsync_Failure()
        {
            this.teamStorageProvider
                  .Setup(x => x.GetTeamDetailAsync("123456"))
                  .Returns(Task.FromResult(NotificationHelperData.teamEntity));

            this.cardHelper
                .Setup(x => x.SendProActiveNotificationCardAsync(new Attachment(), "12345", "https://www.test.com"))
                .Returns(Task.FromResult(false));

            var Result = await this.notificationHelper.SendFeedbackNotificationInChannelAsync();
            Assert.AreEqual(Result, false);
        }
    }
}
