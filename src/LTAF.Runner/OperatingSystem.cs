using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LTAF.Runner
{
    public class OperatingSystem: IOperatingSystem
    {
        public void CreateProcess(string path, string arguments)
        {
            Process.Start(path, arguments);
        }

        public void KillProcess(string processName)
        {
            foreach (Process p in Process.GetProcessesByName(processName))
			{
				p.Kill();
			}
        }
    }
}
