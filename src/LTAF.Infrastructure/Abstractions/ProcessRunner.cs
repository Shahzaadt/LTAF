using System.Diagnostics;

namespace LTAF.Infrastructure.Abstractions
{
    internal class ProcessRunner : IProcessRunner
    {
        public bool Start(Process process)
        {
            return process.Start();
        }

        public void Stop(Process process)
        {
            process.Kill();

            process.Close();
        }

        public bool WaitForExit(Process process, int milliseconds)
        {
            return process.WaitForExit(milliseconds);
        }
    }
}
