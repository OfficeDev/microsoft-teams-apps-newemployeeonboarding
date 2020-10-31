// <copyright file="FeedbackController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.NewHireOnboarding.Authentication;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;

    /// <summary>
    /// Controller to handle view feedback API operations.
    /// </summary>
    [Route("api/feedback")]
    [ApiController]
    [Authorize(PolicyNames.MustBeHumanResourceTeamMemberUserPolicy)]
    public class FeedbackController : BaseNewHireOnboardingController
    {
        /// <summary>
        /// Instance to send logs to the Application Insights service.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Instance of feedback provider to get the feedback from table storage.
        /// </summary>
        private readonly IFeedbackProvider feedbackProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackController"/> class.
        /// </summary>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        /// <param name="feedbackProvider">Instance of feedback provider.</param>
        public FeedbackController(
        ILogger<FeedbackController> logger,
        TelemetryClient telemetryClient,
        IFeedbackProvider feedbackProvider)
        : base(telemetryClient)
        {
            this.logger = logger;
            this.feedbackProvider = feedbackProvider;
        }

        /// <summary>
        /// Get call to retrieve list of feedback data.
        /// </summary>
        /// <param name="batchId">Unique id of batch, combination of year and month</param>
        /// <returns>A collection of feedback data.</returns>
        [HttpGet]
        public async Task<IActionResult> FeedbacksAsync(string batchId)
        {
            try
            {
                this.RecordEvent("Feedback - HTTP Get call initiated.");

                if (string.IsNullOrEmpty(batchId))
                {
                    this.logger.LogError($"Batch Id is either null or empty. User Id: {this.UserAadId}");
                    return this.BadRequest("Batch Id cannot be null or empty.");
                }

                var feedbackEntities = await this.feedbackProvider.GetFeedbackAsync(batchId);

                if (feedbackEntities == null)
                {
                    this.logger.LogInformation("Feedback data is not available.");
                    return this.Ok(new List<FeedbackEntity>());
                }

                var filteredData = feedbackEntities
                    .Select(row => new
                    {
                        SubmittedOn = string.Format(CultureInfo.InvariantCulture, row.SubmittedOn.ToString()),
                        Feedback = row.Feedback,
                        NewHireName = row.NewHireName,
                    }).OrderByDescending(feedback => feedback.SubmittedOn);

                this.RecordEvent("Feedback - HTTP Get call succeeded.");

                return this.Ok(filteredData);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetching feedback data.");
                throw;
            }
        }
    }
}