// <copyright file="GraphApiHelperTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.Helpers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Class to test Graph API helper methods.
    /// </summary>
    [TestClass]
    public class GraphApiHelperTest
    {
        /// <summary>
        /// Graph API helper instance.
        /// </summary>
        GraphApiHelper graphApiHelper;

        GraphApiHelper constructor_ArgumentNullException;

        [TestInitialize]
        public void GraphApiHelperTestSetup()
        {
            var logger = new Mock<ILogger<GraphApiHelper>>().Object;
            var memoryCache = new Mock<IMemoryCache>().Object;
            graphApiHelper = new GraphApiHelper(logger, memoryCache, ConfigurationData.botOptions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GraphApiHelper_ThrowsArgumentNullException()
        {
            var logger = new Mock<ILogger<GraphApiHelper>>().Object;
            var memoryCache = new Mock<IMemoryCache>().Object;

            constructor_ArgumentNullException = new GraphApiHelper(logger, memoryCache, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetUserPhotoAsync_ThrowsUserIdArgumentNullException()
        {
            await graphApiHelper.GetUserPhotoAsync(GraphApiHelperData.AccessToken, null);
        }

        [TestMethod]
        public async Task GetUserPhotoAsync_ReturnsNullForInvalidUserId()
        {
            var Result = await graphApiHelper.GetUserPhotoAsync(GraphApiHelperData.AccessToken, GraphApiHelperData.UserId);
            Assert.IsNull(Result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetUserProfileAsync_ThrowsUserIdArgumentNullException()
        {
            await graphApiHelper.GetUserProfileAsync(GraphApiHelperData.AccessToken, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetUserProfileNoteAsync_ThrowsUserIdArgumentNullException()
        {
            await graphApiHelper.GetUserProfileNoteAsync(GraphApiHelperData.AccessToken, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetChannelsAsync_ThrowsTeamIdArgumentNullException()
        {
            await graphApiHelper.GetChannelsAsync(GraphApiHelperData.AccessToken, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetGroupMemberDetailsAsync_ThrowsUserIdArgumentNullException()
        {
            await graphApiHelper.GetGroupMemberIdsAsync(GraphApiHelperData.AccessToken, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UserManagerDetailsAsync_ThrowsUserIdArgumentNullException()
        {
            await graphApiHelper.GetMyManagerAsync(null);
        }
    }
}
