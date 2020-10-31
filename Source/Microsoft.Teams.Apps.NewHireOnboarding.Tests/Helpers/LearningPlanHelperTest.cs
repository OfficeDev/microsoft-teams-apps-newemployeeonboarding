// <copyright file="LearningPlanHelperTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.Helpers
{
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;
    using Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to test learning plan helper methods.
    /// </summary>
    [TestClass]
    public class LearningPlanHelperTest
    {
        Mock<IGraphUtilityHelper> graphUtility;
        Mock<ISharePointHelper> sharePointHelper;
        LearningPlanHelper learningPlanHelper;
        LearningPlanHelper constructor_ArgumentNullException;

        private readonly List<LearningPlanListItemField> learningPlanEmptyList = new List<LearningPlanListItemField>();

        [TestInitialize]
        public void LearningPlanHelperTestSetup()
        {
            var logger = new Mock<ILogger<LearningPlanHelper>>().Object;
            var localizer = new Mock<IStringLocalizer<Strings>>().Object;
            graphUtility = new Mock<IGraphUtilityHelper>();
            sharePointHelper = new Mock<ISharePointHelper>();

            learningPlanHelper = new LearningPlanHelper(
                logger,
                localizer,
                ConfigurationData.botOptions,
                ConfigurationData.sharePointOptions,
                graphUtility.Object,
                sharePointHelper.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LearningPlanHelper_ThrowsArgumentNullException()
        {
            var logger = new Mock<ILogger<LearningPlanHelper>>().Object;
            var localizer = new Mock<IStringLocalizer<Strings>>().Object;

            constructor_ArgumentNullException = new LearningPlanHelper(
            logger,
            localizer,
            null,
            null,
            graphUtility.Object,
            sharePointHelper.Object);
        }

        [TestMethod]
        public async Task LearningPlansExistAsync()
        {
            this.graphUtility
                 .Setup(x => x.ObtainApplicationTokenAsync(
                     LearningPlanHelperData.TenantId,
                     LearningPlanHelperData.ClientId,
                     LearningPlanHelperData.ClientSecret))
                 .Returns(Task.FromResult(LearningPlanHelperData.graphTokenResponse));

            this.sharePointHelper
                .Setup(x => x.GetCompleteLearningPlanDataAsync(LearningPlanHelperData.graphTokenResponse.AccessToken))
                .Returns(Task.FromResult(LearningPlanHelperData.learningPlanListDetail));

            var Result = await this.learningPlanHelper.GetCompleteLearningPlansAsync();

            Assert.AreEqual(Result.ToList().Any(), true);
        }

        [TestMethod]
        public async Task LearningPlansNotExistAsync()
        {
            this.graphUtility
                 .Setup(x => x.ObtainApplicationTokenAsync(
                     LearningPlanHelperData.TenantId,
                     LearningPlanHelperData.ClientId,
                     LearningPlanHelperData.ClientSecret))
                 .Returns(Task.FromResult(LearningPlanHelperData.graphTokenResponse));

            this.sharePointHelper
                .Setup(x => x.GetCompleteLearningPlanDataAsync(LearningPlanHelperData.graphTokenResponse.AccessToken))
                .Returns(Task.FromResult(learningPlanEmptyList));

            var Result = await this.learningPlanHelper.GetCompleteLearningPlansAsync();

            Assert.AreEqual(Result.ToList().Any(), false);
        }

        [TestMethod]
        public void LearningPlanAttachmentCardExist()
        {
            this.graphUtility
                 .Setup(x => x.ObtainApplicationTokenAsync(
                     LearningPlanHelperData.TenantId,
                     LearningPlanHelperData.ClientId,
                     LearningPlanHelperData.ClientSecret))
                 .Returns(Task.FromResult(LearningPlanHelperData.graphTokenResponse));

            this.sharePointHelper
                .Setup(x => x.GetCompleteLearningPlanDataAsync(LearningPlanHelperData.graphTokenResponse.AccessToken))
                .Returns(Task.FromResult(LearningPlanHelperData.learningPlanListDetail));

            var Result = this.learningPlanHelper.GetLearningPlanCardAsync("Week 1 => Technology => ReactJS").Result;

            Assert.AreEqual(Result.ContentType, "application/vnd.microsoft.card.adaptive");
        }
    }
}
