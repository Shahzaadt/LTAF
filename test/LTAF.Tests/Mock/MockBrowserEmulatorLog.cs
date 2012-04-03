using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Emulators;

namespace LTAF.UnitTests
{
    internal class MockBrowserEmulatorLog : IBrowserEmulatorLog
    {
        private bool _silentMode = false;
        private StringBuilder _output = new StringBuilder();

        public StringBuilder Output
        {
            get
            {
                return _output;
            }
        }

        public bool SilentMode
        {
            get
            {
                return _silentMode;
            }
            set
            {
                _silentMode = value;
            }
        }

        #region IBrowserEmulatorLog Members

        public void WriteLine(string text)
        {
            if (!SilentMode)
            {
                _output.AppendLine(text);
            }
        }

        public void WriteLine(byte[] data)
        {
            if (!SilentMode)
            {
                _output.Append(data);
            }
        }

        #endregion
    }
}
