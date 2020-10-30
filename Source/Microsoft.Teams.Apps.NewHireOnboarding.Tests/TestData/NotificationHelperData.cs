// <copyright file="NotificationHelperData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData
{
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using System.Collections.Generic;

    /// <summary>
    /// Notification helper class methods test data.
    /// </summary>
    public static class NotificationHelperData
    {
        public static readonly IEnumerable<IntroductionEntity> introductionEntities = new List<IntroductionEntity>()
        {
            new IntroductionEntity()
            {
                ManagerAadObjectId = "123450",
                NewHireAadObjectId = "456780",
                ManagerConversationId = "13200",
                NewHireConversationId = "464656",
            },
            new IntroductionEntity()
            {
                ManagerAadObjectId = "123450",
                NewHireAadObjectId = "456780",
                ManagerConversationId = "13200",
                NewHireConversationId = "464656",
            }
        };

        public static readonly IEnumerable<IntroductionEntity> emptyIntroductionList = new List<IntroductionEntity>();

        public static readonly TeamEntity teamEntity = new TeamEntity()
        {
            TeamId = "12345",
            AadGroupId = "56789",
            InstalledByAadObjectId = "12345",
            Name = "Test",
            ServiceUrl = "https://www.test.com"
        };
    }
}
