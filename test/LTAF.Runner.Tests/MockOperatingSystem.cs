using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Runner;

namespace LTAF.Runner.Tests
{
    public class MockOperatingSystem: IOperatingSystem
    {
        public string CreateProcessArguments
        {
            get;
            set;
        }

        public string CreateProcessName
        {
            get;
            set;
        }

        public string KillProcessName
        {
            get;
            set;
        }

        public void CreateProcess(string path, string arguments)
        {
            CreateProcessArguments = arguments;
            CreateProcessName = path;
        }

        public void KillProcess(string p)
        {
            KillProcessName = p;
        }

    }
}
