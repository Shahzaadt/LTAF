using System.Diagnostics;

namespace LTAF.Infrastructure.Abstractions
{
    internal interface IProcessRunner
    {
        bool Start(Process process);
        void Stop(Process process);
        bool WaitForExit(Process process, int milliseconds);
    }
}
