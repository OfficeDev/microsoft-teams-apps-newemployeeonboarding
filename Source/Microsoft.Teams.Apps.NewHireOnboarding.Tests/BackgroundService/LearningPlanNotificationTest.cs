// <copyright file="LearningPlanNotificationTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.BackgroundService
{
    using Castle.Core.Configuration;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.NewHireOnboarding.BackgroundService;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to test learning plan notification background service methods.
    /// </summary>
    [TestClass]
    public class LearningPlanNotificationTest
    {
        Mock<IUserStorageProvider> userStorageProvider;
        Mock<ILearningPlanHelper> learningPlanHelper;
        LearningPlanNotification learningPlanNotification;
        LearningPlanNotification constructor_SharePointOptionsArgumentNullException;
        LearningPlanNotification constructor_BotOptionsArgumentNullException;

        [TestInitialize]
        public void LearningPlanNotificationTestSetup()
        {
            var logger = new Mock<ILogger<LearningPlanNotification>>().Object;
            var localizer = new Mock<IStringLocalizer<Strings>>().Object;
            userStorageProvider = new Mock<IUserStorageProvider>();
            learningPlanHelper = new Mock<ILearningPlanHelper>();

            learningPlanNotification = new LearningPlanNotification(
                logger,
                userStorageProvider.Object,
                learningPlanHelper.Object,
                ConfigurationData.sharePointOptions,
                null,
                localizer,
                ConfigurationData.botOptions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LearningPlanNotification_SharePointOptionsArgumentNullException()
        {
            var logger = new Mock<ILogger<LearningPlanNotification>>().Object;
            var localizer = new Mock<IStringLocalizer<Strings>>().Object;
            userStorageProvider = new Mock<IUserStorageProvider>();
            learningPlanHelper = new Mock<ILearningPlanHelper>();

            constructor_SharePointOptionsArgumentNullException = new LearningPlanNotification(
                logger,
                userStorageProvider.Object,
                learningPlanHelper.Object,
                null,
                null,
                localizer,
                ConfigurationData.botOptions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LearningPlanNotification_BotOptionsArgumentNullException()
        {
            var logger = new Mock<ILogger<LearningPlanNotification>>().Object;
            var localizer = new Mock<IStringLocalizer<Strings>>().Object;
            userStorageProvider = new Mock<IUserStorageProvider>();
            learningPlanHelper = new Mock<ILearningPlanHelper>();

            constructor_BotOptionsArgumentNullException = new LearningPlanNotification(
                logger,
                userStorageProvider.Object,
                learningPlanHelper.Object,
                ConfigurationData.sharePointOptions,
                null,
                localizer,
                null);
        }

        [TestMethod]
        public async Task NotificationSentSuccessAsync()
        {
            this.userStorageProvider
                 .Setup(x => x.GetAllUsersAsync(
                     (int)UserRole.NewHire))
                 .Returns(Task.FromResult(LearningPlanNotificationData.userEntities));

            this.learningPlanHelper
                .Setup(x => x.GetCompleteLearningPlansAsync())
                .Returns(Task.FromResult(LearningPlanNotificationData.learningPlanListDetail));

            var Result = await this.learningPlanNotification.SendWeeklyNotificationAsync();

            Assert.AreEqual(Result, true);
        }

        [TestMethod]
        public async Task LearningPlanNotExistAsync()
        {
            this.userStorageProvider
                 .Setup(x => x.GetAllUsersAsync(
                     (int)UserRole.NewHire))
                 .Returns(Task.FromResult(LearningPlanNotificationData.userEntities));

            this.learningPlanHelper
                .Setup(x => x.GetCompleteLearningPlansAsync())
                .Returns(Task.FromResult(LearningPlanNotificationData.learningPlanEmptyList));

            var Result = await this.learningPlanNotification.SendWeeklyNotificationAsync();

            Assert.AreEqual(Result, false);
        }
    }
}
