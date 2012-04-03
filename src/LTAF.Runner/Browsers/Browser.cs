using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.Runner
{
    public abstract class Browser
    {
        public abstract void Open(IOperatingSystem operatingSystem, string url);

        public abstract void Close(IOperatingSystem operatingSystem);
    }
}
