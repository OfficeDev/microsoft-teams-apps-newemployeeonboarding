// <copyright file="CacheKeysConstants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Constants
{
    /// <summary>
    /// Constants to list keys used by cache layers in application.
    /// </summary>
    public static class CacheKeysConstants
    {
        /// <summary>
        /// Cache key for security group.
        /// </summary>
        public const string SecurityGroup = "_Sg";

        /// <summary>
        /// Cache key for user profile image.
        /// </summary>
        public const string Image = "_Img";

        /// <summary>
        /// Cache key for learning plan.
        /// </summary>
        public const string LearningPlanCacheKey = "_Sp_Lp";

        /// <summary>
        /// Cache key for learning plan.
        /// </summary>
        public const string IntroductionQuestionsCacheKey = "_Sp_Iq";

        /// <summary>
        /// Cache key for human resource .
        /// </summary>
        public const string HumanResourceCacheKey = "_Hr";

        /// <summary>
        /// Cache key for learning plan column mappings.
        /// </summary>
        public const string LearningPlanColumnMappingCacheKey = "_Sp_Cm";
    }
}
