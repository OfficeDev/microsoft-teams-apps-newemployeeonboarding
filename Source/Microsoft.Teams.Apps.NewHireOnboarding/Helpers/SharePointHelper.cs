// <copyright file="SharePointHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Constants;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Implements the methods that are defined in <see cref="ISharePointHelper"/>.
    /// </summary>
    public class SharePointHelper : ISharePointHelper
    {
        /// <summary>
        /// Microsoft Graph API base url.
        /// </summary>
        private const string GraphAPIBaseURL = "https://graph.microsoft.com/";

        /// <summary>
        /// Display name for topic field in SharePoint.
        /// There are multiple mappings for Topic column so use it's internal name directly.
        /// </summary>
        private const string TopicFieldText = "Title";

        /// <summary>
        /// Display name for task name field in SharePoint.
        /// </summary>
        private const string TaskNameFieldText = "Task name";

        /// <summary>
        /// Display name for complete by field  in SharePoint.
        /// </summary>
        private const string CompleteByFieldText = "Complete by";

        /// <summary>
        /// Display name for notes field in SharePoint.
        /// </summary>
        private const string NotesFieldText = "Notes";

        /// <summary>
        /// Display name for resource link.
        /// </summary>
        private const string LinkFieldText = "Link";

        /// <summary>
        /// Display name for task image.
        /// </summary>
        private const string TaskImageFieldText = "Task image";

        /// <summary>
        /// Instance to log details in application insights.
        /// </summary>
        private readonly ILogger<SharePointHelper> logger;

        /// <summary>
        /// A set of key/value application configuration properties for SharePoint.
        /// </summary>
        private readonly IOptions<SharePointSettings> options;

        /// <summary>
        /// Instance to work with Microsoft Graph methods.
        /// </summary>
        private readonly IGraphUtilityHelper graphUtility;

        /// <summary>
        /// Cache for storing SharePoint result.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// A set of key/value configuration of bot settings.
        /// </summary>
        private readonly IOptions<BotOptions> botSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointHelper"/> class.
        /// </summary>
        /// <param name="logger">Instance of ILogger for telemetry logging.</param>
        /// <param name="options">A set of key/value application configuration properties for SharePoint.</param>
        /// <param name="graphUtility">Instance of Microsoft Graph utility helper.</param>
        /// <param name="memoryCache">MemoryCache instance for caching SharePoint result.</param>
        /// <param name="botSettings">Represents a set of key/value bot settings.</param>
        public SharePointHelper(
            ILogger<SharePointHelper> logger,
            IOptions<SharePointSettings> options,
            IGraphUtilityHelper graphUtility,
            IMemoryCache memoryCache,
            IOptions<BotOptions> botSettings)
        {
            this.logger = logger;
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.graphUtility = graphUtility;
            this.memoryCache = memoryCache;
            this.botSettings = botSettings ?? throw new ArgumentNullException(nameof(botSettings));
        }

        /// <summary>
        /// Get list of complete learning plan details.
        /// </summary>
        /// <param name="token">Azure Active Directory (AAD) token to access Microsoft Graph API.</param>
        /// <returns>A task that returns list of complete learning plan details.</returns>
        public async Task<List<LearningPlanListItemField>> GetCompleteLearningPlanDataAsync(string token)
        {
            this.memoryCache.TryGetValue(this.GetLearningPlanCacheKey(), out List<LearningPlanListItemField> cacheLearningPlan);
            if (cacheLearningPlan != null)
            {
                return cacheLearningPlan;
            }

            Dictionary<string, string> columnMappingDictionary = new Dictionary<string, string>();
            this.memoryCache.TryGetValue(this.GetColumnMappingCacheKey(), out Dictionary<string, string> cacheColumnMappingDictionary);

            if (cacheColumnMappingDictionary == null)
            {
                var columnMappingResponse = await this.graphUtility.GetAsync(token, $"{GraphAPIBaseURL}/v1.0/sites/{this.options.Value.SiteTenantName}:/sites/{this.options.Value.SiteName}:/lists/{this.options.Value.NewHireCheckListName}/columns");
                if (columnMappingResponse.IsSuccessStatusCode)
                {
                    var responseContent = await columnMappingResponse.Content.ReadAsStringAsync();
                    var learningPlanColumns = JsonConvert.DeserializeObject<LearningPlanColumnDetail>(responseContent);

                    foreach (var columnMapping in learningPlanColumns.ColumnMappings.Distinct())
                    {
                        if (!columnMappingDictionary.ContainsKey(columnMapping.DisplayName))
                        {
                            columnMappingDictionary.Add(columnMapping.DisplayName, columnMapping.ActualName);
                        }
                    }

                    this.memoryCache.Set(this.GetColumnMappingCacheKey(), columnMappingDictionary, TimeSpan.FromMinutes(this.botSettings.Value.CacheDurationInMinutes));
                }
            }
            else
            {
                columnMappingDictionary = cacheColumnMappingDictionary;
            }

            var response = await this.graphUtility.GetAsync(token, $"{GraphAPIBaseURL}/v1.0/sites/{this.options.Value.SiteTenantName}:/sites/{this.options.Value.SiteName}:/lists/{this.options.Value.NewHireCheckListName}/items?expand=fields");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                JObject siteListDataResponse = JObject.Parse(responseContent);
                List<LearningPlanListItemField> learningPlanFieldsData = new List<LearningPlanListItemField>();

                var learningPlanValues = siteListDataResponse["value"];
                foreach (var learningPlan in learningPlanValues)
                {
                    var learningPlanFields = learningPlan["fields"];

                    learningPlanFieldsData.Add(
                        new LearningPlanListItemField()
                        {
                            Topic = learningPlanFields[TopicFieldText]?.ToString(),
                            TaskName = learningPlanFields[columnMappingDictionary[TaskNameFieldText]]?.ToString(),
                            CompleteBy = learningPlanFields[columnMappingDictionary[CompleteByFieldText]]?.ToString(),
                            Notes = learningPlanFields[columnMappingDictionary[NotesFieldText]]?.ToString(),
                            Link = string.IsNullOrEmpty(learningPlanFields[LinkFieldText]?.ToString())
                            ? null : JsonConvert.DeserializeObject<LearningPlanResource>(learningPlanFields[LinkFieldText]?.ToString()),
                            TaskImage = string.IsNullOrEmpty(learningPlanFields[columnMappingDictionary[TaskImageFieldText]]?.ToString())
                            ? null : JsonConvert.DeserializeObject<LearningPlanTaskImage>(learningPlanFields[columnMappingDictionary[TaskImageFieldText]]?.ToString()),
                        });
                }

                this.memoryCache.Set(this.GetLearningPlanCacheKey(), learningPlanFieldsData, TimeSpan.FromMinutes(this.botSettings.Value.CacheDurationInMinutes));

                return learningPlanFieldsData;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            this.logger.LogInformation($"Graph API get site list data error: {errorMessage} statusCode: {response.StatusCode}");

            return null;
        }

        /// <summary>
        /// Get new hire introduction questions from SharePoint site.
        /// </summary>
        /// <param name="token">Azure Active Directory (AAD) token to access Microsoft Graph API.</param>
        /// <returns>A task that returns list of introduction questions.</returns>
        public async Task<IEnumerable<IntroductionDetail>> GetIntroductionQuestionsAsync(string token)
        {
            this.memoryCache.TryGetValue(this.GetIntroductionQuestionsCacheKey(), out IEnumerable<IntroductionDetail> cacheIntroductionQnA);
            if (cacheIntroductionQnA != null)
            {
                var introductionQuestions = cacheIntroductionQnA.Select(row => new IntroductionDetail() { Question = row.Question }).ToList();
                return introductionQuestions;
            }

            var response = await this.graphUtility.GetAsync(token, $"{GraphAPIBaseURL}/v1.0/sites/{this.options.Value.SiteTenantName}:/sites/{this.options.Value.SiteName}:/lists/{this.options.Value.NewHireQuestionListName}/items?expand=fields");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var introductionQuestionDataResponse = JsonConvert.DeserializeObject<IntroductionQuestionListDetail>(responseContent);

                var introductionQuestionnaire = introductionQuestionDataResponse
                    .ListItems.Where(question => question.IntroductionQuestionData.IsActive == true).Select(x => new IntroductionDetail
                    {
                        Question = x.IntroductionQuestionData.Question,
                    }).ToList();

                this.memoryCache.Set(
                    this.GetIntroductionQuestionsCacheKey(),
                    introductionQuestionnaire,
                    TimeSpan.FromMinutes(this.botSettings.Value.CacheDurationInMinutes));

                return introductionQuestionnaire;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            this.logger.LogInformation($"Graph API get site list data error: {errorMessage} statusCode: {response.StatusCode}");

            return null;
        }

        /// <summary>
        /// Get learning plan cache key.
        /// </summary>
        /// <returns>Returns learning plan cache key</returns>
        private string GetLearningPlanCacheKey()
        {
            return CacheKeysConstants.LearningPlanCacheKey;
        }

        /// <summary>
        /// Get introduction questions cache key.
        /// </summary>
        /// <returns>Returns introduction questions cache key.</returns>
        private string GetIntroductionQuestionsCacheKey()
        {
            return CacheKeysConstants.IntroductionQuestionsCacheKey;
        }

        /// <summary>
        /// Get column mapping cache key.
        /// </summary>
        /// <returns>Returns column mapping cache key.</returns>
        private string GetColumnMappingCacheKey()
        {
            return CacheKeysConstants.LearningPlanColumnMappingCacheKey;
        }
    }
}