// <copyright file="BaseNewHireOnboardingController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Base controller to handle new hire onboarding API operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseNewHireOnboardingController : ControllerBase
    {
        /// <summary>
        /// Instance of application insights telemetry client.
        /// </summary>
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNewHireOnboardingController"/> class.
        /// </summary>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        public BaseNewHireOnboardingController(
            TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Gets the user Azure Active Directory id from the HttpContext.
        /// </summary>
        protected string UserAadId
        {
            get
            {
                var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
                var claim = this.User.Claims.FirstOrDefault(p => oidClaimType.Equals(p.Type, StringComparison.OrdinalIgnoreCase));
                if (claim == null)
                {
                    return null;
                }

                return claim.Value;
            }
        }

        /// <summary>
        /// Records event data to Application Insights telemetry client.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public void RecordEvent(string eventName)
        {
            this.telemetryClient.TrackEvent(eventName, new Dictionary<string, string>
            {
                { "userId", this.UserAadId },
            });
        }
    }
}