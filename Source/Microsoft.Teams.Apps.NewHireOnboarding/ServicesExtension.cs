// <copyright file="ServicesExtension.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.Bot.Builder.BotFramework;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Identity.Client;
    using Microsoft.Teams.Apps.NewHireOnboarding.BackgroundService;
    using Microsoft.Teams.Apps.NewHireOnboarding.Bot;
    using Microsoft.Teams.Apps.NewHireOnboarding.Dialogs;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;

    /// <summary>
    /// Class which helps to extend ServiceCollection.
    /// </summary>
    public static class ServicesExtension
    {
        /// <summary>
        /// Adds application configuration settings to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BotOptions>(options =>
            {
                options.AppBaseUri = configuration.GetValue<string>("App:AppBaseUri");
                options.TenantId = configuration.GetValue<string>("App:TenantId");
                options.ManifestId = configuration.GetValue<string>("App:ManifestId");
                options.TeamsAppId = configuration.GetValue<string>("App:TeamsAppId");
                options.MicrosoftAppId = configuration.GetValue<string>("MicrosoftAppId");
                options.MicrosoftAppPassword = configuration.GetValue<string>("MicrosoftAppPassword");
                options.HumanResourceTeamId = ParseTeamIdFromDeepLink(configuration.GetValue<string>("App:HumanResourceTeamLink"));
                options.CacheDurationInMinutes = configuration.GetValue<int>("Cache:CacheDurationInMinutes");
            });

            services.Configure<TokenSettings>(options =>
            {
                options.ConnectionName = configuration.GetValue<string>("App:ConnectionName");
            });

            services.Configure<SharePointSettings>(options =>
            {
                options.SiteName = configuration.GetValue<string>("SharePoint:SiteName");
                options.NewHireCheckListName = configuration.GetValue<string>("SharePoint:NewHireCheckListName");
                options.SiteTenantName = configuration.GetValue<string>("SharePoint:SiteTenantName");
                options.ShareFeedbackFormUrl = configuration.GetValue<string>("SharePoint:ShareFeedbackFormUrl");
                options.CompleteLearningPlanUrl = configuration.GetValue<string>("SharePoint:CompleteLearningPlanUrl");
                options.NewHireQuestionListName = configuration.GetValue<string>("SharePoint:NewHireQuestionListName");
                options.NewHireLearningPlansInWeeks = configuration.GetValue<int>("SharePoint:NewHireLearningPlansInWeeks");
            });

            services.Configure<AadSecurityGroupSettings>(options =>
            {
                options.Id = configuration.GetValue<string>("SecurityGroup:Id");
            });

            services.Configure<TelemetrySettings>(options =>
            {
                options.InstrumentationKey = configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");
            });

            services.Configure<StorageSettings>(options =>
            {
                options.ConnectionString = configuration.GetValue<string>("Storage:ConnectionString");
            });

            services.Configure<PairUpBackgroundServiceSettings>(options =>
            {
                options.DelayInPairUpNotificationInDays = configuration.GetValue<int>("PairUpBackgroundService:DelayInPairUpNotificationInDays");
                options.NewHireRetentionPeriodInDays = configuration.GetValue<int>("PairUpBackgroundService:NewHireRetentionPeriodInDays");
            });
        }

        /// <summary>
        /// Add confidential credential provider to access API.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddConfidentialCredentialProvider(this IServiceCollection services, IConfiguration configuration)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            IConfidentialClientApplication confidentialClientApp = ConfidentialClientApplicationBuilder.Create(configuration["MicrosoftAppId"])
                .WithClientSecret(configuration["MicrosoftAppPassword"])
                .Build();
            services.AddSingleton<IConfidentialClientApplication>(confidentialClientApp);
        }

        /// <summary>
        /// Adds credential providers for authentication.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddCredentialProviders(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();
            services.AddSingleton(new MicrosoftAppCredentials(configuration.GetValue<string>("MicrosoftAppId"), configuration.GetValue<string>("MicrosoftAppPassword")));

#pragma warning disable CA2000 // This is singleton which has lifetime same as the app
            services.AddSingleton(new OAuthClient(new MicrosoftAppCredentials(configuration.GetValue<string>("MicrosoftAppId"), configuration.GetValue<string>("MicrosoftAppPassword"))));
#pragma warning restore CA2000 // This is singleton which has lifetime same as the app
        }

        /// <summary>
        /// Adds providers to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        public static void AddProviders(this IServiceCollection services)
        {
            // Storage provider
            services.AddSingleton<ITeamStorageProvider, TeamStorageProvider>();
            services.AddSingleton<IUserStorageProvider, UserStorageProvider>();
            services.AddSingleton<IIntroductionStorageProvider, IntroductionStorageProvider>();
            services.AddSingleton<IFeedbackProvider, FeedbackProvider>();
            services.AddSingleton<IImageUploadProvider, ImageUploadProvider>();

            // Background service
            services.AddHostedService<LearningPlanNotification>();
            services.AddHostedService<SurveyNotificationBackgroundService>();
            services.AddHostedService<PairUpNotificationBackgroundService>();
            services.AddHostedService<AppManagerBackgroundService>();
        }

        /// <summary>
        /// Adds helpers to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddHelpers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry(configuration.GetValue<string>("ApplicationInsights:InstrumentationKey"));
            services.AddSingleton<ITokenHelper, TokenHelper>();
            services.AddSingleton<ILearningPlanHelper, LearningPlanHelper>();
            services.AddSingleton<ISharePointHelper, SharePointHelper>();
            services.AddSingleton<IWelcomeCardFactory, WelcomeCardFactory>();
            services.AddSingleton<IActivityHelper, ActivityHelper<MainDialog>>();
            services.AddSingleton<IIntroductionCardHelper, IntroductionCardHelper>();
            services.AddSingleton<INotificationCardHelper, NotificationCardHelper>();
            services.AddSingleton<INotificationHelper, NotificationHelper>();
            services.AddSingleton<ITeamsInfoHelper, TeamsInfoHelper>();
            services.AddSingleton<ITeamMembership, GraphApiHelper>();
            services.AddSingleton<IUserProfile, GraphApiHelper>();
            services.AddSingleton<IAppManagerService, GraphApiHelper>();
        }

        /// <summary>
        /// Adds user state and conversation state to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddBotStates(this IServiceCollection services, IConfiguration configuration)
        {
            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // For conversation state.
            services.AddSingleton<IStorage>(new AzureBlobStorage(configuration.GetValue<string>("Storage:ConnectionString"), "bot-state"));
        }

        /// <summary>
        /// Adds bot framework adapter to specified IServiceCollection.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        public static void AddBotFrameworkAdapter(this IServiceCollection services)
        {
            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // The Dialog that will be run by the bot.
            services.AddSingleton<MainDialog>();

            // Create the Middleware that will be added to the middleware pipeline in the AdapterWithErrorHandler.
            services.AddSingleton<ActivityMiddleware>();
            services.AddTransient(serviceProvider => (BotFrameworkAdapter)serviceProvider.GetRequiredService<IBotFrameworkHttpAdapter>());
            services.AddTransient<IBot, Bot.ActivityHandler<MainDialog>>();
        }

        /// <summary>
        /// Add localization.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public static void AddLocalization(this IServiceCollection services, IConfiguration configuration)
        {
            // Add i18n.
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var defaultCulture = CultureInfo.GetCultureInfo(configuration.GetValue<string>("i18n:DefaultCulture"));
                var supportedCultures = configuration.GetValue<string>("i18n:SupportedCultures").Split(',')
                    .Select(culture => CultureInfo.GetCultureInfo(culture))
                    .ToList();

                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new LocalizationCultureProvider(),
                };
            });
        }

        /// <summary>
        /// Based on deep link URL received find team id and set it.
        /// </summary>
        /// <param name="teamIdDeepLink">Deep link to get the team id.</param>
        /// <returns>A team id from the deep link URL.</returns>
        private static string ParseTeamIdFromDeepLink(string teamIdDeepLink)
        {
            // team id regex match
            // for a pattern like https://teams.microsoft.com/l/team/19%3a64c719819fb1412db8a28fd4a30b581a%40thread.tacv2/conversations?groupId=53b4782c-7c98-4449-993a-441870d10af9&tenantId=72f988bf-86f1-41af-91ab-2d7cd011db47
            // regex checks for 19%3a64c719819fb1412db8a28fd4a30b581a%40thread.tacv2
            var match = Regex.Match(teamIdDeepLink, @"teams.microsoft.com/l/team/(\S+)/");
            if (!match.Success)
            {
                throw new ArgumentException($"Invalid team found.");
            }

            return HttpUtility.UrlDecode(match.Groups[1].Value);
        }
    }
}
