// <copyright file="AuthenticationControllerTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Teams.Apps.NewHireOnboarding.Controllers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Controller to handle authentication API operations.
    /// </summary>
    [TestClass]
    public class AuthenticationControllerTest
    {
        AuthenticationController controller;
        AuthenticationController controller_ArgumentNullException;

        [TestInitialize]
        public void AuthenticationControllerTestSetup()
        {
            controller = new AuthenticationController(
                ConfigurationData.botOptions);

            var httpContext = MakeFakeContext();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = httpContext;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AuthenticationController_ThrowsArgumentNullException()
        {
            controller_ArgumentNullException = new AuthenticationController(null);
        }

        [TestMethod]
        public void ConsentUrl_NotNull_Success()
        {
            var okResult = this.controller.GetConsentUrl("test", "test");

            Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public void ConsentUrl_ReturnswithAssignedparameters()
        {
            var okResult = this.controller.GetConsentUrl("test", "test");

            var loginHint = okResult.Split("&").Last();
            var windowLocationOriginDomain = okResult.Split("=")[1].Contains("test");

            Assert.IsTrue(windowLocationOriginDomain);
            Assert.AreEqual(loginHint.Split("=")[1], "test");
        }

        /// <summary>
        /// Make fake HTTP context for unit testing.
        /// </summary>
        /// <returns></returns>
        private static HttpContext MakeFakeContext()
        {
            var userAadObjectId =  "<<AAD object id>>";
            var context = new Mock<HttpContext>();
            var request = new Mock<HttpContext>();
            var response = new Mock<HttpContext>();
            var user = new Mock<ClaimsPrincipal>();
            var identity = new Mock<IIdentity>();
            var claim = new Claim[]
            {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", userAadObjectId),
            };

            context.Setup(ctx => ctx.User).Returns(user.Object);
            user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            user.Setup(ctx => ctx.Claims).Returns(claim);
            identity.Setup(id => id.IsAuthenticated).Returns(true);
            identity.Setup(id => id.Name).Returns("test");

            return context.Object;
        }
    }
}
