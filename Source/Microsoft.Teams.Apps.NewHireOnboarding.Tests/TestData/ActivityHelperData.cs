// <copyright file="ActivityHelperData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData
{
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Activity helper test data.
    /// </summary>
    public static class ActivityHelperData
    {
        public static readonly List<LearningPlanListItemField> learningPlanListDetail = new List<LearningPlanListItemField>()
        {
            new LearningPlanListItemField()
            {
                CompleteBy = "Week 1",
                Topic = "Technology",
                TaskName = "ReactJS",
                Link = new LearningPlanResource()
                {
                    Description = "",
                    Url = ""
                }

            },
            new LearningPlanListItemField()
            {
                CompleteBy = "Week 2",
                Topic = "Management",
                TaskName = "Team management",
                Link = new LearningPlanResource()
                {
                    Description = "",
                    Url = ""
                }
            }
        };

        public static readonly List<TeamEntity> teamEntities = new List<TeamEntity>()
        {
            new TeamEntity()
            {
                AadGroupId = "12345",
                InstalledByAadObjectId = "45641",
                Name = "xyz",
                TeamId = "12345",
                ServiceUrl = "https://test.com",
            },
            new TeamEntity()
            {
                AadGroupId = "678910",
                InstalledByAadObjectId = "98756",
                Name = "abc",
                TeamId = "52479",
                ServiceUrl = "https://test.com",
            }
        };

        public static readonly List<TeamEntity> emptyTeamCollection = new List<TeamEntity>();

        public static readonly List<Microsoft.Graph.Team> teamCollection = new List<Microsoft.Graph.Team>()
        {
            new Team
            {
                Description = "Test",
                DisplayName = "Test",
                Id = "12345"
            },
            new Team
            {
                Description = "Test1",
                DisplayName = "Test1",
                Id = "23456"
            }
        };

        public static readonly List<Channel> channelCollection = new List<Channel>()
        {
            new Channel
            {
                Description = "Test",
                DisplayName = "Test",
                Id = "123456",
                Email = "asdfsadf"
            },
            new Channel
            {
                Description = "Test1",
                DisplayName = "Test1",
                Id = "7898798",
                Email = "878fgddf"
            }
        };

        public static readonly IntroductionEntity introductionEntity = new IntroductionEntity()
        {
            ManagerAadObjectId = "45678",
            NewHireAadObjectId = "12345",
            NewHireName = "xyz",
            ApprovalStatus = 1,
            Comments = "Test",
            ManagerConversationId = "789456123",
            NewHireConversationId = "13456898",
            ApprovedOn = DateTime.UtcNow,
            PartitionKey = "45678",
            RowKey = "12345",
            NewHireProfileNote = "",
            NewHireQuestionnaire = "",
            SurveyNotificationSentOn = DateTime.UtcNow,
            NewHireUserPrincipalName = "Test",
            SurveyNotificationSentStatus = 0,
            UserProfileImageUrl = "https://test.com"
        };

        public static readonly List<Models.TeamDetail> teamDetailCollection = new List<Models.TeamDetail>()
        {
            new Models.TeamDetail()
            {
                Channels = new List<Models.ChannelDetail>()
                {
                    new Models.ChannelDetail()
                    {
                        ChannelId = "456123",
                        ChannelName = "Test"
                    },
                    new Models.ChannelDetail()
                    {
                        ChannelId = "12345",
                        ChannelName = "Test5"
                    },
                },
                TeamName = "Test",
                TeamId = "12345",
            },
            new Models.TeamDetail()
            {
                Channels = new List<Models.ChannelDetail>()
                {
                    new Models.ChannelDetail()
                    {
                        ChannelId = "45612300",
                        ChannelName = "Test00"
                    },
                    new Models.ChannelDetail()
                    {
                        ChannelId = "1234500",
                        ChannelName = "Test500"
                    },
                },
                TeamName = "Test00",
                TeamId = "1234500",
            },
        };

        public static readonly TeamsChannelAccount teamsChannelAccount = new TeamsChannelAccount()
        {
            GivenName = "test",
            Surname = "Test",
            Email = "Test",
            UserPrincipalName = "Test",
            UserRole = "asdfasf",
            TenantId = "12345",
            AadObjectId = "12345"
        };

        public static readonly UserProfileDetail userProfileDetail = new UserProfileDetail()
        {
            OdataContext = "test",
            Id = "45678",
            JobTitle = "Test"
        };

        public static readonly IEnumerable<IntroductionDetail> IntroductionQuestions = new List<IntroductionDetail>()
        {
            new IntroductionDetail()
            {
                Question = "Test question 01",
                Answer = "Test answer 01"
            },
            new IntroductionDetail()
            {
                Question = "Test question 02",
                Answer = "Test answer 02"
            }
        };

        public static readonly UserEntity userEntity = new UserEntity()
        {
            AadObjectId = "12345",
            ConversationId = "23456",
            BotInstalledOn = DateTime.UtcNow,
            ServiceUrl = "https://test.com",
            UserRole = 1,
        };

        /// <summary>
        /// Task module response type.
        /// </summary>
        public static readonly string TaskModuleResponseType = "responseType";

        /// <summary>
        /// New joiner Azure Active Directory id.
        /// </summary>
        public static readonly string NewJoinerAadObjectId = "12345";

        /// <summary>
        /// Hiring manager Azure Active Directory id.
        /// </summary>
        public static readonly string HiringManagerAadObjectId = "45678";

        /// <summary>
        /// Validation error text.
        /// </summary>
        public static readonly string ValidationErrorCardText = "Error text";

        /// <summary>
        /// Task module success response type.
        /// </summary>
        public static readonly string TaskModuleSuccessResponseType = "continue";
    }
}
