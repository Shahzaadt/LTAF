using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LTAF.Emulators
{
    internal interface IBrowserEmulatorLog
    {
        void WriteLine(string text);
        void WriteLine(byte[] data);
        bool SilentMode { get; set; }
    }
}
