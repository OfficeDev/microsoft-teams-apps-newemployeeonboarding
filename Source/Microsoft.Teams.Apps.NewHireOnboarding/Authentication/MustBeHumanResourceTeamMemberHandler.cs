// <copyright file="MustBeHumanResourceTeamMemberHandler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Authentication
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;

    /// <summary>
    /// The class that represent the helper methods for activity handler.
    /// </summary>
    public class MustBeHumanResourceTeamMemberHandler : AuthorizationHandler<MustBeHumanResourceTeamMemberRequirement>
    {
        /// <summary>
        /// A set of key/value configuration of bot settings.
        /// </summary>
        private readonly IOptions<BotOptions> botSettings;

        /// <summary>
        /// Provider to fetch team details from bot adapter.
        /// </summary>
        private readonly ITeamsInfoHelper teamsInfoHelper;

        /// <summary>
        /// Cache for storing authorization result.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Instance to send logs to the logger service.
        /// </summary>
        private readonly ILogger<MustBeHumanResourceTeamMemberHandler> logger;

        /// <summary>
        /// Human resource Team Id.
        /// </summary>
        private readonly string humanResourceTeamId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MustBeHumanResourceTeamMemberHandler"/> class.
        /// </summary>
        /// <param name="botSettings">Represents a set of key/value bot settings.</param>
        /// <param name="teamsInfoHelper">Provider to fetch team details from bot adapter.</param>
        /// <param name="memoryCache">MemoryCache instance for caching authorization result.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        public MustBeHumanResourceTeamMemberHandler(IOptions<BotOptions> botSettings, ITeamsInfoHelper teamsInfoHelper, IMemoryCache memoryCache, ILogger<MustBeHumanResourceTeamMemberHandler> logger)
        {
            botSettings = botSettings ?? throw new ArgumentNullException(nameof(botSettings));
            teamsInfoHelper = teamsInfoHelper ?? throw new ArgumentNullException(nameof(teamsInfoHelper));
            logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.botSettings = botSettings;
            this.botSettings.Value.CacheDurationInMinutes = this.botSettings.Value.CacheDurationInMinutes > 0
                ? this.botSettings.Value.CacheDurationInMinutes : 60;

            this.teamsInfoHelper = teamsInfoHelper;
            this.memoryCache = memoryCache;
            this.logger = logger;
            this.humanResourceTeamId = this.botSettings.Value.HumanResourceTeamId;
        }

        /// <summary>
        /// This method handles the authorization requirement.
        /// </summary>
        /// <param name="context">AuthorizationHandlerContext instance.</param>
        /// <param name="requirement">IAuthorizationRequirement instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        protected async override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MustBeHumanResourceTeamMemberRequirement requirement)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

            var oidClaim = context.User.Claims.FirstOrDefault(p => oidClaimType.Equals(p.Type, StringComparison.OrdinalIgnoreCase));

            if (await this.IsHRTeamMemberAsync(this.humanResourceTeamId, oidClaim?.Value))
            {
                context.Succeed(requirement);
            }
        }

        /// <summary>
        /// Check if a user is a member of a human resource team.
        /// </summary>
        /// <param name="teamId">The team id of that the uses to check if the user is a member of human resource. </param>
        /// <param name="userAadObjectId">The user's Azure Active Directory object id.</param>
        /// <returns>The flag indicates that the user is a part of certain team or not.</returns>
        private async Task<bool> IsHRTeamMemberAsync(string teamId, string userAadObjectId)
        {
            try
            {
                bool isCacheEntryExists = this.memoryCache.TryGetValue(this.GetCacheKey(userAadObjectId), out bool isUserValidMember);
                if (!isCacheEntryExists)
                {
                    var teamMember = await this.teamsInfoHelper.GetTeamMemberAsync(teamId, userAadObjectId);
                    isUserValidMember = teamMember != null;

                    this.memoryCache.Set(this.GetCacheKey(userAadObjectId), isUserValidMember, TimeSpan.FromMinutes(this.botSettings.Value.CacheDurationInMinutes));
                }

                return isUserValidMember;
            }
#pragma warning disable CA1031 // Catching general exceptions to log exception details in telemetry client.
            catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions to log exception details in telemetry client.
            {
                this.logger.LogError(ex, $"Error occurred while fetching team member for team: {teamId} - user object id: {userAadObjectId} ");

                // Return false if the member is not found in team id or either of the information is incorrect.
                // Caller should handle false value to throw unauthorized if required.
                return false;
            }
        }

        /// <summary>
        /// Get human resource team cache key value.
        /// </summary>
        /// <param name="userAadObjectId">Unique id of Azure Active Directory of user.</param>
        /// <returns>Returns a human resource team cache key value.</returns>
        private string GetCacheKey(string userAadObjectId)
        {
            return $"{this.humanResourceTeamId}{userAadObjectId}{CacheKeysConstants.HumanResourceCacheKey}";
        }
    }
}
