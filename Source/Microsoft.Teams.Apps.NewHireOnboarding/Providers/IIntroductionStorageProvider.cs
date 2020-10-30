// <copyright file="IIntroductionStorageProvider.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Providers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;

    /// <summary>
    /// This interface will contain helper methods for new hire introduction storage provider.
    /// </summary>
    public interface IIntroductionStorageProvider
    {
        /// <summary>
        /// Store or update new hire introduction details in the storage.
        /// </summary>
        /// <param name="introductionEntity">Represents new hire introduction entity used for storage and retrieval.</param>
        /// <returns><see cref="Task"/> Returns the status whether new hire introduction entity is stored or not.</returns>
        Task<bool> StoreOrUpdateIntroductionDetailAsync(IntroductionEntity introductionEntity);

        /// <summary>
        /// Get already saved new hire introduction entity from the storage.
        /// </summary>
        /// <param name="newJoinerAadObjectId">Azure Active Directory object id of new hire.</param>
        /// <param name="hiringManagerAadObjectId">Azure Active Directory object id of hiring manager.</param>
        /// <returns><see cref="Task"/>Returns new hire introduction entity.</returns>
        Task<IntroductionEntity> GetIntroductionDetailAsync(string newJoinerAadObjectId, string hiringManagerAadObjectId);

        /// <summary>
        /// This method is used to get review introductions details for a given hiring manager.
        /// </summary>
        /// <param name="hiringManagerAadObjectId">Azure Active Directory object id of hiring manager .</param>
        /// <returns>List of new hire introduction details.</returns>
        Task<IEnumerable<IntroductionEntity>> GetAllIntroductionsAsync(string hiringManagerAadObjectId);

        /// <summary>
        /// This method is used to get all approved introduction entities where survey notification sent status is pending.
        /// </summary>
        /// <returns>List of introduction entities.</returns>
        Task<IEnumerable<IntroductionEntity>> GetAllPendingSurveyIntroductionAsync();

        /// <summary>
        /// This method is used to get filtered review introductions details for a given hiring manager.
        /// </summary>
        /// <param name="hiringManagerAadObjectId">Azure Active Directory object id of hiring manager .</param>
        /// <returns>List of filtered new hire introduction details.</returns>
        Task<IEnumerable<IntroductionEntity>> GetFilteredIntroductionsAsync(string hiringManagerAadObjectId);
    }
}
