// <copyright file="FileExtensionsTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class to test file extension method.
    /// </summary>
    [TestClass]
    public class FileExtensionsTest
    {
        [TestMethod]
        public void GetFileExtension()
        {
            var Result = FileExtensions.GetFileExtensionFromUrl("/test.png");
            Assert.AreEqual(Result, "png");
        }

        [TestMethod]
        public void GetFileExtensionFromEncryptedUrl()
        {
            var Result = FileExtensions.GetFileExtensionFromUrl("/:w:");
            Assert.AreEqual(Result, "docx");
        }

        [TestMethod]
        public void GetEmptyFileExtension()
        {
            var Result = FileExtensions.GetFileExtensionFromUrl("test");
            Assert.AreEqual(Result,"");
        }
    }
}
