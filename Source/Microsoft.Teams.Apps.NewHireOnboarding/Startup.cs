// <copyright file="Startup.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Teams.Apps.NewHireOnboarding.Authentication;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Polly;
    using Polly.Extensions.Http;

    /// <summary>
    /// The Startup class is responsible for configuring the DI container and acts as the composition root.
    /// </summary>
    public sealed class Startup
    {
        private readonly IConfiguration configuration;

        private IWebHostEnvironment CurrentEnvironment { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The environment provided configuration.</param>
        /// <param name="env">The environment.</param>
#pragma warning disable SA1201 // Declare property before initializing in constructor.
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
#pragma warning restore SA1201 // Declare property before initializing in constructor.
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.CurrentEnvironment = env;
            this.GetKeyVaultByManagedServiceIdentity();
        }

        /// <summary>
        /// Configure the composition root for the application.
        /// </summary>
        /// <param name="services">The stub composition root.</param>
        /// <remarks>
        /// For more information see: https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </remarks>
#pragma warning disable CA1506 // Composition root expected to have coupling with many components.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddNewHireOnboardingAuthentication(this.configuration);
            services.AddHttpClient<IGraphUtilityHelper, GraphUtilityHelper>().AddPolicyHandler(GetRetryPolicy());
            services.AddConfidentialCredentialProvider(this.configuration);
            services.AddHttpContextAccessor();
            services.AddConfigurationSettings(this.configuration);
            services.AddCredentialProviders(this.configuration);
            services.AddProviders();
            services.AddHelpers(this.configuration);
            services.AddBotStates(this.configuration);
            services.AddSingleton<IChannelProvider, SimpleChannelProvider>();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddMemoryCache();
            services.AddBotFrameworkAdapter();

            // Add i18n.
            services.AddLocalization(this.configuration);
        }
#pragma warning restore CA1506

        /// <summary>
        /// Configure the application request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">Hosting Environment.</param>
        public void Configure(IApplicationBuilder app, AspNetCore.Hosting.IWebHostEnvironment env)
        {
            app.UseRequestLocalization();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseMvc();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        /// <summary>
        /// Retry policy for transient error cases.
        /// If there is no success code in response, request will be sent again for two times
        /// with interval of 2 and 8 seconds respectively.
        /// </summary>
        /// <returns>Policy.</returns>
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(response => response.IsSuccessStatusCode == false)
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        /// <summary>
        /// Get KeyVault secrets and set appsettings values.
        /// </summary>
        private void GetKeyVaultByManagedServiceIdentity()
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            // Below if condition is to get access token to https://vault.azure.net in development mode
            if (this.CurrentEnvironment.IsDevelopment())
            {
                Task<string> accessToken = azureServiceTokenProvider.GetAccessTokenAsync("https://vault.azure.net");
                accessToken.Wait();
            }

            using var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            // Set storage connection string.
            var storageSecretValue = keyVaultClient.GetSecretAsync($"{this.configuration.GetSection("KeyVault")["BaseURL"]}{this.configuration["KeyVaultStrings:StorageConnection"]}");

            storageSecretValue.Wait();
            this.configuration["Storage:ConnectionString"] = storageSecretValue.Result.Value;

            // Set Application id.
            var appIdSecretValue = keyVaultClient.GetSecretAsync($"{this.configuration.GetSection("KeyVault")["BaseURL"]}{this.configuration["KeyVaultStrings:MicrosoftAppId"]}");

            appIdSecretValue.Wait();
            this.configuration["MicrosoftAppId"] = appIdSecretValue.Result.Value;

            // Set Application password.
            var appPasswordsecretValue = keyVaultClient.GetSecretAsync($"{this.configuration.GetSection("KeyVault")["BaseURL"]}{this.configuration["KeyVaultStrings:MicrosoftAppPassword"]}");

            appPasswordsecretValue.Wait();
            this.configuration["MicrosoftAppPassword"] = appPasswordsecretValue.Result.Value;
        }
    }
}
