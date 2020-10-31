// <copyright file="LearningPlanNotificationData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData
{
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Learning plan notification background service test data.
    /// </summary>
    public static class LearningPlanNotificationData
    {
        public static readonly IEnumerable<UserEntity> userEntities = new List<UserEntity>()
        {
            new UserEntity()
            {
                AadObjectId = "12345",
                BotInstalledOn = DateTime.Now,
                UserRole = 1,
                ConversationId = "12345",
                UserProfileImageUrl = "test",
                ServiceUrl = "test"
            },
            new UserEntity()
            {
                AadObjectId = "54321",
                BotInstalledOn = DateTime.Now,
                UserRole = 1,
                ConversationId = "54321",
                UserProfileImageUrl = "test1",
                ServiceUrl = "test1"
            },
            new UserEntity()
            {
                AadObjectId = "678910",
                BotInstalledOn = DateTime.Now,
                UserRole = 1,
                ConversationId = "678910",
                UserProfileImageUrl = "test2",
                ServiceUrl = "test2"
            }
        };

        public static readonly IEnumerable<LearningPlanListItemField> learningPlanListDetail = new List<LearningPlanListItemField>()
        {
            new LearningPlanListItemField()
            {
                Topic = "Technology",
                TaskName = "C# Training",
                CompleteBy = "Week 1",
                Notes = "C# Technology",
                TaskImage  =  new LearningPlanTaskImage()
                {
                    Url = "Test",
                },
                Link =  new LearningPlanResource()
                {
                    Description = "",
                    Url = ""
                }
            },
            new LearningPlanListItemField()
            {
                Topic = "Technology",
                TaskName = "React Training",
                CompleteBy = "Week 1",
                Notes = "React Technology",
                TaskImage  =  new LearningPlanTaskImage()
                {
                    Url = "Test",
                },
                Link =  new LearningPlanResource()
                {
                    Description = "",
                    Url = ""
                }
            }
        };

        public static readonly IEnumerable<LearningPlanListItemField> learningPlanEmptyList = new List<LearningPlanListItemField>();
    }
}
