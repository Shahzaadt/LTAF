using System;
using System.Web;
using System.IO;
using System.Reflection;
using Microsoft.Web.Administration;
using LTAF.Infrastructure;
using LTAF.Infrastructure.Abstractions;
using Xunit;
using Xunit.Extensions;
using Moq;
using LTAF.Infrastructure.Test.Abstractions;

namespace LTAF.Infrastructure.Test
{
    public class SiteExtensionsFacts
    {
        public class GetUniqueApplicaionNameFacts
        {
            [Fact]
            public void WhenAppNameExist_ShouldReturnNameWithNewIndex_Fact()
            {
                // Arrange
                var appName = Helper.Randomize("/test");
                var serverManager = Helper.ServerManager;
                serverManager.Sites[0].Applications.Add(appName, @"c:\");
                serverManager.Sites[0].Applications.Add(appName + "_1", "");

                // Act
                var newAppName = serverManager.Sites[0].GetUniqueApplicaionName(appName);

                // Assert
                Xunit.Assert.Equal(appName.Trim('/') + "_2", newAppName); 
            }       
        }

        public class GetVirtualPathFacts
        {
            [Fact]
            public void WhenSourceDoesNotExist_ShouldThrow_Fact()
            {
                // Arrange, Act, Assert
                var exception = Assert.Throws<Exception>(() =>
                    SiteExtensions.GetVirtualPath(Helper.ServerManager.Sites[0], "unknown", ""));

                Xunit.Assert.Equal("Binding for protocol 'unknown' is not defined for the website 'WebSite1'.", exception.Message);
            }
        }
    }
}