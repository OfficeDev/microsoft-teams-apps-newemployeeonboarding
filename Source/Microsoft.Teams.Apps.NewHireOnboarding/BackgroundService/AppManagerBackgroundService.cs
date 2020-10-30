// <copyright file="AppManagerBackgroundService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.BackgroundService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;

    /// <summary>
    /// This class inherits IHostedService and implements the methods related to background tasks for installing the app for a user.
    /// </summary>
    public class AppManagerBackgroundService : BackgroundService
    {
        /// <summary>
        /// Instance to log details in application insights.
        /// </summary>
        private readonly ILogger<AppManagerBackgroundService> logger;

        /// <summary>
        /// Provider for fetching information about user details from storage.
        /// </summary>
        private readonly IUserStorageProvider userStorageProvider;

        /// <summary>
        /// A set of key/value application configuration properties for bot settings.
        /// </summary>
        private readonly IOptions<BotOptions> botOptions;

        /// <summary>
        /// Helper for working with Microsoft Graph API.
        /// </summary>
        private readonly IAppManagerService appManagerServiceHelper;

        /// <summary>
        /// Instance to work with Microsoft Graph methods.
        /// </summary>
        private readonly IGraphUtilityHelper graphTokenUtility;

        /// <summary>
        /// Helper for team operations with Microsoft Graph API.
        /// </summary>
        private readonly ITeamMembership membersService;

        /// <summary>
        /// Helper for user profile operations with Microsoft Graph API.
        /// </summary>
        private readonly IUserProfile userProfileOperations;

        /// <summary>
        /// A set of key/value application configuration properties for AAD security group settings.
        /// </summary>
        private readonly IOptions<AadSecurityGroupSettings> securityGroupSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppManagerBackgroundService"/> class.
        /// </summary>
        /// <param name="graphTokenUtility">Instance of Microsoft Graph utility helper.</param>
        /// <param name="appManagerServiceHelper">Helper for working with Microsoft Graph API.</param>
        /// <param name="logger">Instance of ILogger</param>
        /// <param name="userStorageProvider">Provider for fetching information about user details from storage.</param>
        /// <param name="teamMembershipHelper">Helper for team operations with Microsoft Graph API.</param>
        /// <param name="botOptions">A set of key/value application configuration properties.</param>
        /// <param name="securityGroupSettings"> A set of key/value application configuration properties for AAD security group settings.</param>
        /// <param name="userProfileOperations">Helper for working with Microsoft Graph API for user operation.</param>
        public AppManagerBackgroundService(
            IGraphUtilityHelper graphTokenUtility,
            IAppManagerService appManagerServiceHelper,
            ILogger<AppManagerBackgroundService> logger,
            IUserStorageProvider userStorageProvider,
            ITeamMembership teamMembershipHelper,
            IOptions<BotOptions> botOptions,
            IOptions<AadSecurityGroupSettings> securityGroupSettings,
            IUserProfile userProfileOperations)
        {
            this.appManagerServiceHelper = appManagerServiceHelper ?? throw new ArgumentNullException(nameof(appManagerServiceHelper));
            this.graphTokenUtility = graphTokenUtility ?? throw new ArgumentNullException(nameof(graphTokenUtility));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userStorageProvider = userStorageProvider ?? throw new ArgumentNullException(nameof(userStorageProvider));
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.securityGroupSettings = securityGroupSettings ?? throw new ArgumentNullException(nameof(securityGroupSettings));
            this.membersService = teamMembershipHelper ?? throw new ArgumentNullException(nameof(teamMembershipHelper));
            this.userProfileOperations = userProfileOperations ?? throw new ArgumentNullException(nameof(userProfileOperations));
        }

        /// <summary>
        ///  This method is called when the Microsoft.Extensions.Hosting.IHostedService starts.
        ///  The implementation should return a task that represents the lifetime of the long
        ///  running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken) is called.</param>
        /// <returns>A System.Threading.Tasks.Task that represents the long running operations.</returns>
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var currentDateTime = DateTime.UtcNow;
                    this.logger.LogInformation($"App pre-installer for a user Hosted Service is running at: {currentDateTime}.");

                    var response = await this.graphTokenUtility.ObtainApplicationTokenAsync(this.botOptions.Value.TenantId, this.botOptions.Value.MicrosoftAppId, this.botOptions.Value.MicrosoftAppPassword);
                    if (response == null)
                    {
                        this.logger.LogInformation($"App pre-installer: Failed to acquire application token for application Id: {this.botOptions.Value.MicrosoftAppId}.");
                        return;
                    }

                    // Sync new employees on boarded from security group.
                    await this.SyncUsersFromSecurityGroupAsync(response.AccessToken, this.securityGroupSettings.Value.Id);

                    // Pre-install app for new users.
                    await this.InstallAppForUsersAsync(response.AccessToken);
                }
#pragma warning disable CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                {
                    this.logger.LogError(ex, "Error occurred while running App pre-installer service.");
                }
                finally
                {
                    this.logger.LogInformation($"App pre-installer service execution completed and will resume after {TimeSpan.FromDays(1)} delay.");
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
            }
        }

        /// <summary>
        /// Syncing all the members in a Security Group.
        /// </summary>
        /// <param name="accessToken">Graph application permission.</param>
        /// <param name="groupId">Unique id of security group.</param>
        /// <returns>Returns list of user Aad id.</returns>
        private async Task SyncUsersFromSecurityGroupAsync(string accessToken, string groupId)
        {
            try
            {
                List<string> newUsers = new List<string>();

                // Fetch all new hire from security group.
                var groupMemberIds = await this.membersService.GetGroupMemberIdsAsync(accessToken, groupId);
                if (groupMemberIds == null || !groupMemberIds.Any())
                {
                    this.logger.LogWarning($"New hire security group is empty: {groupId}.");
                    return;
                }

                // Get new hires who already installed the app.
                var existingUsers = await this.userStorageProvider.GetPreInstalledAppUsersAsync(UserRole.NewHire);
                if (existingUsers == null || !existingUsers.Any())
                {
                    // First run experience: Fetch hiring manage for all new hires.
                    var hiringManagerIds = await this.userProfileOperations.GetUserManagerIdsAsync(accessToken, groupMemberIds);
                    groupMemberIds.AddRange(hiringManagerIds);
                    newUsers = groupMemberIds;
                }
                else
                {
                    // Get the delta between new hires and users who already installed the app.
                    newUsers = groupMemberIds.Except(existingUsers.Select(row => row.AadObjectId)).ToList();

                    // Get hiring manages for each new hire from graph.
                    var hiringManagers = await this.userProfileOperations.GetUserManagerIdsAsync(accessToken, newUsers);

                    // Get hiring managers who already installed the app.
                    var existingManagers = await this.userStorageProvider.GetPreInstalledAppUsersAsync(UserRole.HiringManager);
                    if (existingManagers == null || !existingManagers.Any())
                    {
                        newUsers.AddRange(hiringManagers.ToList());
                    }

                    var newHiringManagers = hiringManagers.Except(existingManagers.Select(row => row.AadObjectId));
                    newUsers.AddRange(newHiringManagers.ToList());
                }

                try
                {
                    // Update user information to storage.
                    await this.userStorageProvider.BatchInsertOrMergeAsync(newUsers.Distinct().Select(row => new UserEntity() { AadObjectId = row, ConversationId = string.Empty }));
                }
#pragma warning disable CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                {
                    this.logger.LogError($"Failed to store user details to storage Error: {ex.Message}");
                }
            }
#pragma warning disable CA1031 // Catching general exceptions that might arise during graph call to avoid blocking next run.
            catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions that might arise during graph call to avoid blocking next run.
            {
                this.logger.LogError($"Failed to sync new hire details from security group: Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Pre install app for a user.
        /// </summary>
        /// <param name="accessToken">Graph application token.</param>
        /// <returns>Returns true if installation succeed.</returns>
        private async Task<bool> InstallAppForUsersAsync(string accessToken)
        {
            // Fetch user's Teams app id from settings.
            var teamsAppId = this.botOptions.Value.TeamsAppId;
            if (string.IsNullOrEmpty(teamsAppId))
            {
                this.logger.LogInformation("Failed to install app. Teams App Id not configured");
                return false;
            }

            List<UserEntity> newUsers = await this.userStorageProvider.GetAllUsersWhereBotIsNotInstalledAsync();

            if (newUsers == null || !newUsers.Any())
            {
                this.logger.LogInformation("App pre-installer: User not available for app installation.");
                return false;
            }

            await this.appManagerServiceHelper.InstallAppForUserAsync(accessToken, teamsAppId, newUsers.Select(row => row.AadObjectId).ToList());

            return true;
        }
    }
}
