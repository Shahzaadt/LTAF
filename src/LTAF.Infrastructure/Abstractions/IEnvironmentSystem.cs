using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LTAF.Infrastructure.Abstractions
{
    internal interface IEnvironmentSystem
    {
        string ExpandEnvironmentVariables(string environmentVariable);
        int GetNextAvailablePort(int usedPort = 0);
        Version OSVersion { get; }
    }
}
