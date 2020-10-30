// <copyright file="FileExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding
{
    using System;
    using System.Linq;

    /// <summary>
    /// A class that holds file extensions used in multiple files.
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// Extension type for excel file.
        /// </summary>
        private const string Excel = "xlsx";

        /// <summary>
        /// Extension type for power point file.
        /// </summary>
        private const string PowerPoint = "pptx";

        /// <summary>
        /// Extension type for word file.
        /// </summary>
        private const string Word = "docx";

        /// <summary>
        /// Get file extension from content URL.
        /// </summary>
        /// <param name="url">Learning content URL.</param>
        /// <returns>File extension.</returns>
#pragma warning disable CA1054 // Uri parameters should not be strings
        public static string GetFileExtensionFromUrl(string url)
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            url = url ?? throw new ArgumentNullException(nameof(url));

            string fileExtension = string.Empty;

            // Check if url contains file extension. e.g.(.xlsx, .pptx, .docx)
            if (url.Split('/').Last().Contains(".", StringComparison.InvariantCultureIgnoreCase))
            {
                url = url.Split('/').Last();
                fileExtension = url.Substring(url.LastIndexOf('.')).Split(".")[1];
            }

            // Check if encrypted url contains file extension in form of e.g.(:p:, :w:, :x:)
            else if (url.Contains("/:", StringComparison.InvariantCultureIgnoreCase))
            {
                string urlType = url.Split("/:")[1].Substring(0, 1);
                switch (urlType)
                {
                    case "p":
                        fileExtension = PowerPoint;
                        break;
                    case "w":
                        fileExtension = Word;
                        break;
                    case "x":
                        fileExtension = Excel;
                        break;
                    default:
                        break;
                }
            }

            return fileExtension;
        }
    }
}
