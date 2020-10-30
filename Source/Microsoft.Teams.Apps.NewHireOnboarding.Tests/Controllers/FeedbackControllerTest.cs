// <copyright file="FeedbackControllerTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>


namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.NewHireOnboarding.Controllers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Controller to handle personal goal API operations.
    /// </summary>
    [TestClass]
    public class FeedbackControllerTest
    {
        FeedbackController controller;
        Mock<IFeedbackProvider> feedbackProvider;
        TelemetryClient telemetryClient;

        private readonly IEnumerable<FeedbackEntity> feedbackEntities = new List<FeedbackEntity>()
        {
            new FeedbackEntity()
            {
                BatchId = "Sep_2020",
                NewHireAadObjectId = "12345",
                SubmittedOn = DateTime.UtcNow,
                Feedback = "Test",
                NewHireName = "Abc"
            },
            new FeedbackEntity()
            {
                BatchId = "Sep_2020",
                NewHireAadObjectId = "45678",
                SubmittedOn = DateTime.UtcNow,
                Feedback = "Test2",
                NewHireName = "Xyz"
            }
        };

        [TestInitialize]
        public void FeedbackControllerTestSetup()
        {
            var logger = new Mock<ILogger<FeedbackController>>().Object;
            feedbackProvider = new Mock<IFeedbackProvider>();
            telemetryClient = new TelemetryClient();

            controller = new FeedbackController(
                logger,
                telemetryClient,
                feedbackProvider.Object);

            var httpContext = MakeFakeContext();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = httpContext;
        }

        [TestMethod]
        public async Task GetFeedbacksAsync_ReturnsOkResult()
        {
            this.feedbackProvider
               .Setup(x => x.GetFeedbackAsync("Sep_2020"))
               .Returns(Task.FromResult(feedbackEntities));

            var currentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.UtcNow.Month);
            var batchId = $"{currentMonth.Substring(0, 3)}_{DateTime.UtcNow.Year}";

            var okResult = (ObjectResult)await this.controller.FeedbacksAsync(batchId);
            Assert.AreEqual(okResult.StatusCode, StatusCodes.Status200OK);
            Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task GetFeedbacksAsync_ReturnsBadRequest()
        {
            this.feedbackProvider
               .Setup(x => x.GetFeedbackAsync("Sep_2020"))
               .Returns(Task.FromResult(feedbackEntities));

            var okResult = (ObjectResult)await this.controller.FeedbacksAsync(string.Empty);
            Assert.AreEqual(okResult.StatusCode, StatusCodes.Status400BadRequest);
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
