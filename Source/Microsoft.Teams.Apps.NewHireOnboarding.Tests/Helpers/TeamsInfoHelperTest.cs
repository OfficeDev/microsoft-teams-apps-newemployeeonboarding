// <copyright file="ToKenHelperTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.Helpers
{
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to teams info helper methods.
    /// </summary>
    [TestClass]
    public class TeamsInfoHelperTest
    {
        TeamsInfoHelper teamsInfoHelper;
        TeamsInfoHelper constructor_BotAdapterArgumentNullException;
        TeamsInfoHelper constructor_providerArgumentNullException;
        Mock<ITeamStorageProvider> teamStorageProvider;
        Mock<IBotFrameworkHttpAdapter> botAdapter;
        MicrosoftAppCredentials microsoftAppCredentials;

        Mock<IAdapterIntegration> adapterIntegration;
        ConversationReference conversationReference;

        [TestInitialize]
        public void teamsInfoHelperTestSetup()
        {
            var logger = new Mock<ILogger<TeamsInfoHelper>>().Object;
            teamStorageProvider = new Mock<ITeamStorageProvider>();
            botAdapter = new Mock<IBotFrameworkHttpAdapter>();
            teamsInfoHelper = new TeamsInfoHelper(botAdapter.Object, teamStorageProvider.Object, microsoftAppCredentials, logger);

            adapterIntegration = new Mock<IAdapterIntegration>();

            var conversationReference = new ConversationReference
            {
                ChannelId = CardConstants.TeamsBotFrameworkChannelId,
                ServiceUrl = "https://test.com",
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TeamsInfoHelper_ThrowsBotAdapterArgumentNullException()
        {
           var logger = new Mock<ILogger<TeamsInfoHelper>>().Object;
           teamStorageProvider = new Mock<ITeamStorageProvider>();
           constructor_BotAdapterArgumentNullException = new TeamsInfoHelper(null, teamStorageProvider.Object, microsoftAppCredentials, logger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TeamsInfoHelper_ThrowsProviderArgumentNullException()
        {
            var logger = new Mock<ILogger<TeamsInfoHelper>>().Object;
            botAdapter = new Mock<IBotFrameworkHttpAdapter>();
            constructor_providerArgumentNullException = new TeamsInfoHelper(botAdapter.Object, null, microsoftAppCredentials, logger);
            
        }

        [TestMethod]
        public async Task GetTeamMemberAsync()
        {
            this.teamStorageProvider
                .Setup(x => x.GetTeamDetailAsync("123"))
                .Returns(Task.FromResult(NotificationHelperData.teamEntity));

            this.adapterIntegration
               .Setup(x => x.ContinueConversationAsync("12345", conversationReference, null, CancellationToken.None))
               .Returns(Task.FromResult(NotificationHelperData.teamEntity));

            var Result = await teamsInfoHelper.GetTeamMemberAsync("123", "6d230b1a-065e-4dab-9253-caa64f2d3519");
            Assert.IsNull(Result);
        }
    }
}
