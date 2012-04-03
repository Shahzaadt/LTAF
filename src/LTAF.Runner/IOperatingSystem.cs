using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.Runner
{
    public interface IOperatingSystem
    {
        void CreateProcess(string path, string arguments);

        void KillProcess(string p);
    }
}
