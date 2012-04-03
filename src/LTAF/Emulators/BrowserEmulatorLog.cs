using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using LTAF.Engine;

namespace LTAF.Emulators
{
    /// <summary>
    /// Parameters for generic BrowserCommand
    /// </summary>
    internal class BrowserEmulatorLog : IBrowserEmulatorLog
    {
        private bool _silentMode = false;

        public bool SilentMode
        {
            get
            {
                return this._silentMode;
            }
            set
            {
                this._silentMode = value;
            }
        }

        public void WriteLine(string text)
        {
            if (!SilentMode)
            {
                Console.WriteLine(text);
            }
        }

        public void WriteLine(byte[] data)
        {
            if (!SilentMode)
            {
                Console.WriteLine(data);
            }
        }
    }
}
