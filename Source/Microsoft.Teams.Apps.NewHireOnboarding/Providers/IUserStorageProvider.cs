// <copyright file="IUserStorageProvider.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Providers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;

    /// <summary>
    /// Interface for user storage provider.
    /// </summary>
    public interface IUserStorageProvider
    {
        /// <summary>
        /// Store or update user details in the storage.
        /// </summary>
        /// <param name="userEntity">Represents user entity used for storage and retrieval.</param>
        /// <returns><see cref="Task"/> Returns the status whether user entity is stored or not.</returns>
        Task<bool> StoreOrUpdateUserDetailAsync(UserEntity userEntity);

        /// <summary>
        /// Get already saved user entity from Azure Table Storage.
        /// </summary>
        /// <param name="userAadObjectId">Azure Active Directory object id of user.</param>
        /// <returns><see cref="Task"/>Returns user entity.</returns>
        Task<UserEntity> GetUserDetailAsync(string userAadObjectId);

        /// <summary>
        /// Get all user details based on role.
        /// </summary>
        /// <param name="userRole">User role like 0:New Hire, 1:Hiring Manager.</param>
        /// <returns>List of users details based on role.</returns>
        Task<IEnumerable<UserEntity>> GetAllUsersAsync(UserRole userRole);

        /// <summary>
        /// Get all new hires who opted for pair-up meeting.
        /// </summary>
        /// <returns>List of users details.</returns>
        Task<IEnumerable<UserEntity>> GetUsersOptedForPairUpMeetingAsync();

        /// <summary>
        /// Get all users where bot already installed.
        /// </summary>
        /// <param name="userRole">User role like 0:New Hire, 1:Hiring Manager.</param>
        /// <returns>List of users details.</returns>
        Task<IEnumerable<UserEntity>> GetPreInstalledAppUsersAsync(UserRole userRole);

        /// <summary>
        /// Get all users where bot is not installed.
        /// </summary>
        /// <returns>List of users details.</returns>
        Task<List<UserEntity>> GetAllUsersWhereBotIsNotInstalledAsync();

        /// <summary>
        /// Insert or merge a batch of entities in Azure table storage.
        /// A batch can contain up to 100 entities.
        /// </summary>
        /// <param name="entities">Entities to be inserted or merged in Azure table storage.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        Task BatchInsertOrMergeAsync(IEnumerable<UserEntity> entities);
    }
}
