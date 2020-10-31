// <copyright file="GraphApiHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    extern alias BetaLib;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;
#pragma warning disable SA1135 // Application requires both Graph v1.0 and beta SDKs which needs to add extern reference. More details can be found here : https://github.com/microsoftgraph/msgraph-beta-sdk-dotnet
    using Beta = BetaLib.Microsoft.Graph;
#pragma warning restore SA1135 // Application requires both Graph v1.0 and beta SDKs which needs to add extern reference. More details can be found here : https://github.com/microsoftgraph/msgraph-beta-sdk-dotnet

    /// <summary>
    /// Implements the methods that are defined in <see cref="ITeamMembership"/>.
    /// Implements the methods that are defined in <see cref="IUserProfile"/>.
    /// Implements the methods that are defined in <see cref="IAppManagerService"/>.
    /// The class that represent the helper methods to access Microsoft Graph API.
    /// </summary>
    public class GraphApiHelper : ITeamMembership, IUserProfile, IAppManagerService
    {
        /// <summary>
        /// Instance to send logs to the Application Insights service.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Cache for storing Microsoft Graph result.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// A set of key/value configuration of bot settings.
        /// </summary>
        private readonly IOptions<BotOptions> botSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphApiHelper"/> class.
        /// </summary>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        /// <param name="memoryCache">MemoryCache instance for caching Microsoft Graph result.</param>
        /// <param name="botSettings">Represents a set of key/value bot settings.</param>
        public GraphApiHelper(ILogger<GraphApiHelper> logger, IMemoryCache memoryCache, IOptions<BotOptions> botSettings)
        {
            this.logger = logger;
            this.memoryCache = memoryCache;
            this.botSettings = botSettings ?? throw new ArgumentNullException(nameof(botSettings));
        }

        /// <summary>
        /// Get user profile details from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph user access token.</param>
        /// <param name="userIds">Aad object id of users.</param>
        /// <returns>List of user profile details.</returns>
        public async Task<List<User>> GetUserProfileAsync(string token, List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            string query = string.Empty;
            foreach (var id in userIds)
            {
                query += $"id eq '{id}' or ";
            }

            query = query.TrimEnd().Remove(query.Length - 3);

            var graphClient = this.GetGraphServiceClient(token);
            var users = await graphClient.Users
                .Request()
                .Filter(query)
                .Select("displayName, id, jobTitle")
                .WithMaxRetry(GraphConstants.MaxRetry)
                .GetAsync();

            if (users == null)
            {
                return null;
            }

            var userProfiles = users.ToList().Select(row => new User()
            {
                Id = row.Id,
                DisplayName = row.DisplayName,
                AboutMe = row.AboutMe,
                JobTitle = row.JobTitle,
            }).ToList();

            return userProfiles;
        }

        /// <summary>
        /// Get user photo from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph user access token.</param>
        /// <param name="userId">Aad object id of user.</param>
        /// <returns>User photo details.</returns>
        public async Task<Stream> GetUserPhotoAsync(string token, string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentNullException(nameof(userId));
                }

                this.memoryCache.TryGetValue(this.GetUserProfilePictureCacheKey(userId), out Stream cacheImage);
                if (cacheImage != null)
                {
                    return cacheImage;
                }

                var graphClient = this.GetGraphServiceClient(token);
                var stream = await graphClient
                    .Users[userId]
                    .Photo
                    .Content
                    .Request()
                    .WithMaxRetry(GraphConstants.MaxRetry)
                    .GetAsync();

                if (stream == null)
                {
                    return null;
                }

                this.memoryCache.Set(this.GetUserProfilePictureCacheKey(userId), stream, TimeSpan.FromMinutes(this.botSettings.Value.CacheDurationInMinutes));

                return stream;
            }
            catch (ServiceException ex)
            {
                this.logger.LogWarning($"Graph API getting user photo error- {ex.Message}");
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound && ex.RawResponseBody.Contains("The photo wasn't found.", StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }

            return null;
        }

        /// <summary>
        /// Get user profile notes from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph user access token.</param>
        /// <param name="userId">Aad object id of user.</param>
        /// <returns>User profile note.</returns>
        public async Task<string> GetUserProfileNoteAsync(string token, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var graphClient = this.GetGraphServiceClientBeta(token);
            var notes = await graphClient
                .Users[userId]
                .Profile
                .Notes
                .Request()
                .WithMaxRetry(GraphConstants.MaxRetry)
                .GetAsync();

            if (notes == null)
            {
                return null;
            }

            return notes.First().Detail?.Content;
        }

        /// <summary>
        /// Get user manager details from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph user access token.</param>
        /// <returns>User manager details.</returns>
        public async Task<UserProfileDetail> GetMyManagerAsync(string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var graphClient = this.GetGraphServiceClient(token);
            var manager = await graphClient.Me.Manager
                .Request()
                .WithMaxRetry(GraphConstants.MaxRetry)
                .GetAsync();

            if (manager == null)
            {
                return null;
            }

            return new UserProfileDetail()
            {
                Id = manager.Id,
            };
        }

        /// <summary>
        /// Get manager ids for a given list of user Ids from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph application access token.</param>
        /// <param name="userIds">List of user Ids.</param>
        /// <returns>Returns list of manager Ids.</returns>
        public async Task<IEnumerable<string>> GetUserManagerIdsAsync(string token, IEnumerable<string> userIds)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var graphClient = this.GetGraphServiceClient(token);
            HashSet<string> hiringManagerIds = new HashSet<string>();
            foreach (var userId in userIds.Distinct())
            {
                try
                {
                    var manager = await graphClient
                        .Users[userId]
                        .Manager
                        .Request()
                        .WithMaxRetry(GraphConstants.MaxRetry)
                        .GetAsync();
                    if (manager == null)
                    {
                        this.logger.LogInformation($"Unable to find manager for user: {userId}.");
                        continue;
                    }
                    else
                    {
                        hiringManagerIds.Add(manager.Id);
                    }
                }
                catch (ServiceException ex)
                {
                    this.logger.LogError(ex, $"Failed to get manager for user: {userId}. Status Code: {ex.StatusCode} Exception: {ex.Message}");

                    continue;
                }
            }

            return hiringManagerIds;
        }

        /// <summary>
        /// Get joined teams from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph application access token.</param>
        /// <returns>Joined teams details.</returns>
        public async Task<List<Team>> GetMyJoinedTeamsAsync(string token)
        {
            var graphClient = this.GetGraphServiceClient(token);
            var joinedTeams = await graphClient.Me.JoinedTeams
                .Request()
                .WithMaxRetry(GraphConstants.MaxRetry)
                .GetAsync();

            if (joinedTeams == null)
            {
                return null;
            }

            return joinedTeams.Select(row => row).ToList();
        }

        /// <summary>
        /// GET all channels of a team from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph application access token.</param>
        /// <param name="teamId">Unique id of Teams.</param>
        /// <returns>Channels details.</returns>
        public async Task<List<Channel>> GetChannelsAsync(string token, string teamId)
        {
            if (string.IsNullOrWhiteSpace(teamId))
            {
                throw new ArgumentNullException(nameof(teamId));
            }

            var graphClient = this.GetGraphServiceClient(token);
            var channels = await graphClient
                .Teams[teamId]
                .Channels
                .Request()
                .WithMaxRetry(GraphConstants.MaxRetry)
                .GetAsync();

            return channels.Select(row => row).ToList();
        }

        /// <summary>
        /// Get group member details from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph application access token.</param>
        /// <param name="groupId">Unique id of Azure Active Directory security group.</param>
        /// <returns>Group members details.</returns>
        public async Task<List<string>> GetGroupMemberIdsAsync(string token, string groupId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            var graphClient = this.GetGraphServiceClient(token);
            var response = await graphClient
                                    .Groups[groupId]
                                    .TransitiveMembers
                                    .Request()
                                    .Top(GraphConstants.MaxPageSize)
                                    .WithMaxRetry(GraphConstants.MaxRetry)
                                    .Select("id")
                                    .GetAsync();

            var users = response.OfType<User>().ToList();
            while (response.NextPageRequest != null)
            {
                response = await response.NextPageRequest.GetAsync();
                users?.AddRange(response.OfType<User>() ?? new List<User>());
            }

            if (users != null)
            {
                var groupMemberIds = users.ToList().Select(row => row.Id).ToList();

                return groupMemberIds;
            }

            return null;
        }

        /// <summary>
        /// Installs App for a user.
        /// </summary>
        /// <param name="token">Microsoft Graph application access token.</param>
        /// <param name="appId">Teams App Id.</param>
        /// <param name="userIds">List of user's AAD Id.</param>
        /// <returns>A <see cref="Task"/>Representing the asynchronous operation.</returns>
        public async Task InstallAppForUserAsync(string token, string appId, List<string> userIds)
        {
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentNullException(nameof(appId));
            }

            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }

            var graphClient = this.GetGraphServiceClientBeta(token);
            var userScopeTeamsAppInstallation = new Beta.UserScopeTeamsAppInstallation
            {
                AdditionalData = new Dictionary<string, object>()
                {
                    { "teamsApp@odata.bind", $"{GraphConstants.BetaBaseUrl}/appCatalogs/teamsApps/{appId}" },
                },
            };

            foreach (var userId in userIds.Distinct())
            {
                try
                {
                    await graphClient.Users[userId]
                        .Teamwork
                        .InstalledApps
                        .Request()
                        .WithMaxRetry(GraphConstants.MaxRetry)
                        .AddAsync(userScopeTeamsAppInstallation);

                    this.logger.LogInformation($"Application is installed for the user {userId}.");
                }
                catch (ServiceException exception)
                {
                    switch (exception.StatusCode)
                    {
                        case HttpStatusCode.Conflict:
                            // Note: application is already installed.
                            this.logger.LogWarning($"Application is already installed for the user {userId}.");

                            continue;

                        default:
                            this.logger.LogError(exception, $"Failed to install application for user: {userId}. Status Code: {exception.StatusCode} Exception: {exception.Message}");

                            continue;
                    }
                }
#pragma warning disable CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                {
                    this.logger.LogError(ex, $"Failed to install application for user: {userId}. Exception: {ex.Message}");

                    continue;
                }
            }
        }

        /// <summary>
        /// Get Microsoft Graph service client.
        /// </summary>
        /// <param name="accessToken">Token to access MS graph.</param>
        /// <returns>Returns a graph service client object.</returns>
        private GraphServiceClient GetGraphServiceClient(string accessToken)
        {
            return new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        await Task.Run(() =>
                        {
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                                "Bearer",
                                accessToken);
                        });
                    }));
        }

        /// <summary>
        /// Get Microsoft Graph service client beta.
        /// </summary>
        /// <param name="accessToken">Token to access MS graph.</param>
        /// <returns>Returns a graph service client object.</returns>
        private Beta.GraphServiceClient GetGraphServiceClientBeta(string accessToken)
        {
            return new Beta.GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        await Task.Run(() =>
                        {
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                                "Bearer",
                                accessToken);
                        });
                    }));
        }

        /// <summary>
        /// Get user profile picture cache key value.
        /// </summary>
        /// <param name="userId">Azure Active Directory id of user.</param>
        /// <returns>Returns cache key value for user profile picture.</returns>
        private string GetUserProfilePictureCacheKey(string userId)
        {
            return $"{userId}{CacheKeysConstants.Image}";
        }
    }
}
