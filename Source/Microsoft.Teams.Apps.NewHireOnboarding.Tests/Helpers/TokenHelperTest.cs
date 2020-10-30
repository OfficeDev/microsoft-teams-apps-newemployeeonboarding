// <copyright file="ToKenHelperTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.Helpers
{
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to test token helper methods.
    /// </summary>
    [TestClass]
    public class ToKenHelperTest
    {
        OAuthClient oAuthClient = new OAuthClient(new MicrosoftAppCredentials(ConfigurationData.botOptions.Value.MicrosoftAppId, ConfigurationData.botOptions.Value.MicrosoftAppPassword));
        TokenHelper tokenHelper;
        TokenHelper constructor_OAuthClientArgumentNullException;
        TokenHelper constructor_TokenOptionsArgumentNullException;
        TokenHelper constructor_LoggerArgumentNullException;

        public static readonly IOptions<TokenSettings> options = Options.Create(new TokenSettings()
        {
            ConnectionName = "test"
        });

        [TestInitialize]
        public void ToKenHelperTestSetup()
        {
            var logger = new Mock<ILogger<TokenHelper>>().Object;
            tokenHelper = new TokenHelper(oAuthClient, options, logger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToKenHelper_ThrowsOAuthClientArgumentNullException()
        {
            var logger = new Mock<ILogger<TokenHelper>>().Object;

            constructor_OAuthClientArgumentNullException = new TokenHelper(null, options, logger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToKenHelper_ThrowsTokenOptionsArgumentNullException()
        {
            var logger = new Mock<ILogger<TokenHelper>>().Object;

            constructor_TokenOptionsArgumentNullException = new TokenHelper(oAuthClient, null, logger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToKenHelper_ThrowsLoggerArgumentNullException()
        {
            constructor_LoggerArgumentNullException = new TokenHelper(oAuthClient, options, null);
        }

        [TestMethod]
        public async Task GetUserTokenAsync_ReturnsNull()
        {
            var userObjectId = "User Azure Active Directory object id";
            var Result = await tokenHelper.GetUserTokenAsync(userObjectId);
            Assert.IsNull(Result);
        }
    }
}
