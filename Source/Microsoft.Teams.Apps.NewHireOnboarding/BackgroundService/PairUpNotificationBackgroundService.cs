// <copyright file="PairUpNotificationBackgroundService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.BackgroundService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Cards;
    using Microsoft.Teams.Apps.NewHireOnboarding.Helpers;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Microsoft.Teams.Apps.NewHireOnboarding.Providers;

    /// <summary>
    /// BackgroundService class that inherits IHostedService and implements the methods related to background tasks for sending pair-up message once a day.
    /// </summary>
    public class PairUpNotificationBackgroundService : BackgroundService
    {
        /// <summary>
        /// Instance to send logs to the Application Insights service.
        /// </summary>
        private readonly ILogger<PairUpNotificationBackgroundService> logger;

        /// <summary>
        /// The current cultures' string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Gets configuration setting for max pair-ups and notification duration.
        /// </summary>
        private readonly IOptionsMonitor<PairUpBackgroundServiceSettings> pairUpBackgroundServiceOption;

        /// <summary>
        /// Provider for fetching information about user details from storage.
        /// </summary>
        private readonly IUserStorageProvider userStorageProvider;

        /// <summary>
        /// Helper for working with bot notification card.
        /// </summary>
        private readonly INotificationCardHelper notificationCardHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PairUpNotificationBackgroundService"/> class.
        /// BackgroundService class that inherits IHostedService and implements the methods related to sending notification tasks.
        /// </summary>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        /// <param name="localizer">The current cultures' string localizer.</param>
        /// <param name="backgroundServiceOption">Configuration setting for max pair-ups and notification duration.</param>
        /// <param name="userStorageProvider">Provider for fetching information about user details from storage.</param>
        /// <param name="notificationCardHelper">Helper for working with bot notification card.</param>
        public PairUpNotificationBackgroundService(
            ILogger<PairUpNotificationBackgroundService> logger,
            IStringLocalizer<Strings> localizer,
            IOptionsMonitor<PairUpBackgroundServiceSettings> backgroundServiceOption,
            IUserStorageProvider userStorageProvider,
            INotificationCardHelper notificationCardHelper)
        {
            this.logger = logger;
            this.localizer = localizer;
            this.pairUpBackgroundServiceOption = backgroundServiceOption;
            this.userStorageProvider = userStorageProvider;
            this.notificationCardHelper = notificationCardHelper;
        }

        /// <summary>
        /// This method is called when the Microsoft.Extensions.Hosting.IHostedService starts.
        /// The implementation should return a task that represents the lifetime of the long
        /// running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when Microsoft.Extensions.Hosting.IHostedService. StopAsync(System.Threading.CancellationToken) is called.</param>
        /// <returns>A System.Threading.Tasks.Task that represents the long running operations.</returns>
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                int delayInNextPairUpNotification = this.pairUpBackgroundServiceOption.CurrentValue.DelayInPairUpNotificationInDays > 0 ? this.pairUpBackgroundServiceOption.CurrentValue.DelayInPairUpNotificationInDays : 1;
                try
                {
                    this.logger.LogInformation("Pair notification background job execution has started.");
                    await this.MakePairAndSendNotificationAsync();
                }
#pragma warning disable CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
                {
                    this.logger.LogError(ex, $"Error while sending pair-up message card at {nameof(this.MakePairAndSendNotificationAsync)}: {ex}");
                }
                finally
                {
                    this.logger.LogInformation($"Pair up notification service execution completed and will resume after {TimeSpan.FromDays(delayInNextPairUpNotification)} delay.");
                    await Task.Delay(TimeSpan.FromDays(delayInNextPairUpNotification), stoppingToken);
                }
            }
        }

        /// <summary>
        /// Make pair-up with random users and send notification once in a day for each team where app is installed.
        /// </summary>
        /// <returns>A task that make pair-up and send notification to random users for each team where app is installed.</returns>
        private async Task MakePairAndSendNotificationAsync()
        {
            this.logger.LogInformation("Making pair-ups for all user who opted for pair-up meetings.");

            // Now notify each pair found in 1:1 and ask them to reach out to the other person
            // When contacting the user in 1:1, give them the button to opt-out
            try
            {
                // get all users who opted for pair up meetings
                var optedInUsers = await this.userStorageProvider.GetUsersOptedForPairUpMeetingAsync();

                if (optedInUsers != null)
                {
                    this.logger.LogInformation($"Total users: {optedInUsers.Count()} found for pair up meetings.");

                    // 1:1 pair (existing employee : new hire)
                    var pair = this.MakePairs(optedInUsers.ToList());
                    if (pair == null)
                    {
                        this.logger.LogInformation("Pairs could not be made because there is no match found for pair up meetings.");
                    }
                    else
                    {
                        await this.NotifyPairAsync(pair.Item1, pair.Item2);
                    }
                }
            }
#pragma warning disable CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
            catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions that might arise during execution to avoid blocking next run.
            {
                this.logger.LogError(ex, $"Error pairing up members: {ex.Message}", SeverityLevel.Warning);
            }
        }

        /// <summary>
        /// Make pair based on users who opted for pair up meetings.
        /// </summary>
        /// <param name="users">All users who opted for pair up meetings.</param>
        /// <returns>Returns pair-up users.</returns>
        private Tuple<UserEntity, UserEntity> MakePairs(List<UserEntity> users)
        {
            // selecting a random existing user to create a 1:1 pair up with New hire
            var optedInExistingUser = this.Randomize(users.Where(row => (DateTime.UtcNow - row.BotInstalledOn)?.Days > this.pairUpBackgroundServiceOption.CurrentValue.NewHireRetentionPeriodInDays).ToList()).FirstOrDefault();
            if (optedInExistingUser == null)
            {
                this.logger.LogInformation("There is no existing users who opted for pair up meetings.");

                // Getting new hires who opted for pair-up meetings
                var optedInNewHires = users.Where(row => (DateTime.UtcNow - row.BotInstalledOn)?.Days <= this.pairUpBackgroundServiceOption.CurrentValue.NewHireRetentionPeriodInDays).ToList();
                if (optedInNewHires == null || !optedInNewHires.Any())
                {
                    this.logger.LogInformation("There is no new hires who opted for pair up meetings.");
                    return null;
                }

                // Select random new hires for pair-up meetings
                var person1 = this.Randomize(optedInNewHires).FirstOrDefault();
                var person2 = this.Randomize(optedInNewHires.Where(row => row.AadObjectId != person1.AadObjectId).ToList()).FirstOrDefault();

                if (person2 == null)
                {
                    this.logger.LogInformation("Pairs could not be made because there is no match found for pair up meetings.");
                    return null;
                }

                return new Tuple<UserEntity, UserEntity>(item1: person1, item2: person2);
            }

            // pick one random new hire to pair up with existing users.
            var optedInNewHireForPairUp = this.Randomize(users
                .Where(row => (DateTime.UtcNow - row.BotInstalledOn)?.Days <= this.pairUpBackgroundServiceOption.CurrentValue.NewHireRetentionPeriodInDays).ToList()).FirstOrDefault();

            if (optedInNewHireForPairUp == null)
            {
                this.logger.LogInformation($"Pairs could not be made because there is no new hire who opted for pair up meetings.");
                return null;
            }

            return new Tuple<UserEntity, UserEntity>(item1: optedInExistingUser, item2: optedInNewHireForPairUp);
        }

        /// <summary>
        /// Select random users.
        /// </summary>
        /// <param name="items">Items.</param>
        /// <returns>Randomized list</returns>
        private IList<UserEntity> Randomize(IList<UserEntity> items)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            // For each spot in the array, pick
            // a random item to swap into that spot.
            for (int i = 0; i < items.Count - 1; i++)
            {
                int j = rand.Next(i, items.Count);
                UserEntity temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }

            return items;
        }

        /// <summary>
        /// Notify a pair-up meeting to users.
        /// </summary>
        /// <param name="person1">The pair-up person 1.</param>
        /// <param name="person2">The pair-up person 2.</param>
        /// <returns>A task that sends notification card.</returns>
        private async Task NotifyPairAsync(UserEntity person1, UserEntity person2)
        {
            this.logger.LogInformation($"Sending pair-up notification to {person1.AadObjectId} and {person2.AadObjectId}");

            // Fill in person2's info in the card for person1
            var cardForPerson1 = PairUpNotificationAdaptiveCard.GetPairUpNotificationCard(person1, person2, this.localizer);

            // Fill in person1's info in the card for person2
            var cardForPerson2 = PairUpNotificationAdaptiveCard.GetPairUpNotificationCard(person2, person1, this.localizer);

            // Send notifications and return the number that was successful
            await this.notificationCardHelper.SendProActiveNotificationCardAsync(cardForPerson1, person1.ConversationId, person1.ServiceUrl);
            await this.notificationCardHelper.SendProActiveNotificationCardAsync(cardForPerson2, person2.ConversationId, person2.ServiceUrl);
            this.logger.LogInformation($"Pair-up notification sent to {person1.AadObjectId} and {person2.AadObjectId}");
        }
    }
}