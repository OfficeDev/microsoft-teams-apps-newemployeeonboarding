// <copyright file="ConfigurationData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData
{
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;

    /// <summary>
    /// Configuration data settings class.
    /// </summary>
    public static class ConfigurationData
    {
        public static readonly IOptions<BotOptions> botOptions = Options.Create(new BotOptions()
        {
            MicrosoftAppId = "",
            MicrosoftAppPassword = "",
            TenantId = "12345",
            AppBaseUri = "<<App base URL>>",
            HumanResourceTeamId = "12345"
        });

        public static readonly IOptions<SharePointSettings> sharePointOptions = Options.Create(new SharePointSettings()
        {
            CompleteLearningPlanUrl = "<<Complete learning plan Url>>",
            ShareFeedbackFormUrl = "<<Share feedbackform Url>>",
        });

        public static readonly IOptions<AadSecurityGroupSettings> aadSecurityGroupSettings = Options.Create(new AadSecurityGroupSettings()
        {
            Id = "12345",
        });

        public static readonly IOptions<TokenSettings> tokenOptions = Options.Create(new TokenSettings()
        {
            ConnectionName = "<<bot connection name>>"
        });

        public static readonly IOptions<StorageSettings> storageOptions = Options.Create(new StorageSettings()
        {
            ConnectionString = "<<Storage connection string>>",
        });

        /// <summary>
        /// Azure Active Directory id of team.
        /// </summary>
        public static readonly string TeamId = "<<Team Id>>";
    }
}