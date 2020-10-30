// <copyright file="ImageUploadProvider.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Providers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Implements the methods that are defined in <see cref="IImageUploadProvider"/>.
    /// Interface for handling Azure Blob Storage operations like uploading image to blob.
    /// </summary>
    public class ImageUploadProvider : IImageUploadProvider
    {
        /// <summary>
        /// Instance to send logs to the Application Insights service.
        /// </summary>
        private readonly ILogger<ImageUploadProvider> logger;

        /// <summary>
        /// Instance to hold Microsoft Azure Storage data.
        /// </summary>
        private readonly IOptionsMonitor<StorageSettings> storageOptions;

        /// <summary>
        /// Constant value for policy name.
        /// </summary>
        private readonly string sharedAccessPolicyName = "accessPolicy";

        /// <summary>
        /// Root container name on azure blob
        /// </summary>
        private readonly string baseContainerName = "user-image";

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageUploadProvider"/> class.
        /// </summary>
        /// <param name="storageOptions">A set of key/value application storage configuration properties.</param>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        public ImageUploadProvider(IOptionsMonitor<StorageSettings> storageOptions, ILogger<ImageUploadProvider> logger)
        {
            this.logger = logger;
            this.storageOptions = storageOptions ?? throw new ArgumentNullException(nameof(storageOptions));
        }

        /// <summary>
        /// Upload file to specified container and path on Azure Storage Blob.
        /// </summary>
        /// <param name="imageByteArray">Image stream.</param>
        /// <param name="fileName">Image file name.</param>
        /// <returns>Returns file URI on blob if file upload on blob is successful.</returns>
        public async Task<string> UploadImageAsync(Stream imageByteArray, string fileName)
        {
            try
            {
                CloudBlobContainer container = await this.GetContainerAsync();
                await this.SetContainerPermissionsAsync(container, this.sharedAccessPolicyName);
                var cloudBlobContainer = container.GetBlockBlobReference(fileName);

                await cloudBlobContainer.UploadFromStreamAsync(imageByteArray);

                return cloudBlobContainer.Uri.ToString();
            }
            catch (StorageException ex)
            {
                this.logger.LogError(ex, $"Error while uploading image to Azure Blob Storage.");
                throw;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while uploading image to Azure Blob Storage.");
                throw;
            }
        }

        /// <summary>
        /// Get Blob container details
        /// </summary>
        /// <returns>Blob container details</returns>
        private async Task<CloudBlobContainer> GetContainerAsync()
        {
            // Create a blob client for interacting with the blob service.
            CloudBlobClient blobClient = this.InitializeBlobClient();

            // Create a container for organizing blobs within the storage account.
            CloudBlobContainer container = blobClient.GetContainerReference(this.baseContainerName);

            BlobRequestOptions requestOptions = new BlobRequestOptions();
            await container.CreateIfNotExistsAsync(requestOptions, null);

            return container;
        }

        /// <summary>
        /// Sets the access permissions to blob container.
        /// </summary>
        /// <param name="container">A reference to the container.</param>
        /// <param name="storedPolicyName">A string containing the name of the stored access policy. If null, an ad-hoc SAS is created.</param>
        private async Task SetContainerPermissionsAsync(CloudBlobContainer container, string storedPolicyName)
        {
            // Create a new shared access policy and define its constraints.
            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read |
                    SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create,
            };

            // Get the container's existing permissions.
            BlobContainerPermissions permissions = await container.GetPermissionsAsync();

            // Add the new policy to the container's permissions, and set the container's permissions.
            permissions.SharedAccessPolicies.Clear();
            permissions.SharedAccessPolicies.Add(storedPolicyName, sharedPolicy);
            permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            await container.SetPermissionsAsync(permissions);
        }

        /// <summary>
        /// Initialize a blob client for interacting with the blob service.
        /// </summary>
        /// <returns>Returns blob client for blob operations.</returns>
        private CloudBlobClient InitializeBlobClient()
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.storageOptions.CurrentValue.ConnectionString);

                // Create a blob client for interacting with the blob service.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                return blobClient;
            }
            catch (FormatException ex)
            {
                this.logger.LogError(ex, "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid.");
                throw;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while creating storage account.");
                throw;
            }
        }
    }
}
