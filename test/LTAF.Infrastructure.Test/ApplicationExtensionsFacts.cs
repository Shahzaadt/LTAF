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
    public class ApplicationExtensionsFacts
    {
        public class DeployFacts
        {
            ServerManager _serverManager = null;
            public DeployFacts()
            {
                _serverManager = Helper.ServerManager;
            }

            [Fact]
            public void WhenSourceDoesNotExist_ShouldThrow_Fact()
            {
                // Arrange               
                var app = this._serverManager.Sites[0].Applications[0];
                var fileSystem = new Mock<FileSystemBase>(MockBehavior.Strict);
                fileSystem.Setup<bool>(f => f.DirectoryExists(@"c:\Temp\MySite")).Returns(false);

                // Act, Assert
                var exception = Assert.Throws<Exception>(() => ApplicationExtensions.Deploy(app, @"c:\Temp\MySite", fileSystem.Object));
                Xunit.Assert.Equal(@"Failed to deploy files to application, source directory does not exist: 'c:\Temp\MySite'.", exception.Message);
                fileSystem.VerifyAll();
            }

            [Fact]
            public void WhenVirtualDirectoryDoesNotExist_ShouldThrow_Fact()
            {
                // Arrange
                var appName = Helper.Randomize("/test");
                var app = this._serverManager.Sites[0].Applications.Add(appName, "");
                app.VirtualDirectories.Clear();

                var fileSystem = new Mock<FileSystemBase>(MockBehavior.Strict);
                fileSystem.Setup<bool>(f => f.DirectoryExists(@"c:\Temp\MySite")).Returns(true);

                // Act, Assert
                var exception = Assert.Throws<Exception>(() => ApplicationExtensions.Deploy(app, @"c:\Temp\MySite", fileSystem.Object));
                Xunit.Assert.Equal(string.Format(@"Application '{0}' does not have a virtual directory.", appName), exception.Message);
                fileSystem.VerifyAll();
            }

            [Fact]
            public void WhenVirtualDirectoryPathNotExist_ShouldCreate_Fact()
            {
                // Arrange
                var app = this._serverManager.Sites[0].Applications.Add(Helper.Randomize("/test"), @"c:\temp\target");

                var fileSystem = new Mock<FileSystemBase>(MockBehavior.Strict);
                fileSystem.Setup<bool>(f => f.DirectoryExists(@"c:\Temp\MySite")).Returns(true);
                fileSystem.Setup<bool>(f => f.DirectoryExists(@"c:\temp\target")).Returns(false);
                fileSystem.Setup(f => f.DirectoryCreate(@"c:\temp\target"));
                fileSystem.Setup(f => f.DirectoryCopy(@"c:\Temp\MySite", @"c:\temp\target"));

                // Act
                ApplicationExtensions.Deploy(app, @"c:\Temp\MySite", fileSystem.Object);

                // Assert
                fileSystem.VerifyAll();
            }

            [Fact]
            public void WhenVirtualDirectoryPathExist_ShouldJustCopy_Fact()
            {
                // Arrange
                var app = this._serverManager.Sites[0].Applications.Add(Helper.Randomize("/test"), @"c:\temp\target");

                var fileSystem = new Mock<FileSystemBase>(MockBehavior.Strict);
                fileSystem.Setup<bool>(f => f.DirectoryExists(@"c:\Temp\MySite")).Returns(true);
                fileSystem.Setup<bool>(f => f.DirectoryExists(@"c:\temp\target")).Returns(true);
                fileSystem.Setup(f => f.DirectoryCopy(@"c:\Temp\MySite", @"c:\temp\target"));

                // Act
                ApplicationExtensions.Deploy(app, @"c:\Temp\MySite", fileSystem.Object);

                // Assert
                fileSystem.VerifyAll();
            }
        }
    }
}