// <copyright file="IImageUploadProvider.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Providers
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for handling Azure Blob Storage operations like uploading image to blob.
    /// </summary>
    public interface IImageUploadProvider
    {
        /// <summary>
        /// Upload file to specified container and path on Azure Storage Blob.
        /// </summary>
        /// <param name="imageByteArray">Image stream.</param>
        /// <param name="fileName">Image file name.</param>
        /// <returns>Returns file URI on blob if file upload on blob is successful.</returns>
        Task<string> UploadImageAsync(Stream imageByteArray, string fileName);
    }
}
