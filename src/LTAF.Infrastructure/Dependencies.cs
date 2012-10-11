using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Security.AccessControl;
using LTAF.Infrastructure.Abstractions;

namespace LTAF.Infrastructure
{
    internal static class Dependencies
    {
        private static IFileSystem _fileSystem = null;
        public static IFileSystem FileSystem
        {
            get
            {
                if (_fileSystem == null)
                {
                    _fileSystem = new FileSystem();
                }

                return _fileSystem;
            }
            set
            {
                _fileSystem = value;
            }
        }

        private static IEnvironmentSystem _environmentSystem = null;
        public static IEnvironmentSystem EnvironmentSystem
        {
            get
            {
                if (_environmentSystem == null)
                {
                    _environmentSystem = new EnvironmentSystem();
                }

                return _environmentSystem;
            }
            set
            {
                _environmentSystem = value;
            }
        }

        private static IProcessRunner _processRunner = null;
        public static IProcessRunner ProcessRunner
        {
            get
            {
                if (_processRunner == null)
                {
                    _processRunner = new ProcessRunner();
                }

                return _processRunner;
            }
            set
            {
                _processRunner = value;
            }
        }

        private static INuGetCore _nuGetCore = null;
        public static INuGetCore NuGetCore
        {
            get
            {
                if (_nuGetCore == null)
                {
                    _nuGetCore = new NuGetCore();
                }

                return _nuGetCore;
            }
            set
            {
                _nuGetCore = value;
            }
        }
    }
}
