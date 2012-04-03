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
    public class BrowserCommandEventArgs : EventArgs
    {
        private int _threadId;
        BrowserCommand _command;
        int _secondsTimeout;
        bool _abortCommand;

        /// <summary>
        /// Public ctor
        /// </summary>
        public BrowserCommandEventArgs(int threadId, BrowserCommand command, int secondsTimeout)
        {
            this._threadId = threadId;
            this._command = command;
            this._secondsTimeout = secondsTimeout;
            this._abortCommand = false;
        }

        /// <summary>ThreadId </summary>
        public int ThreadId
        {
            get { return this._threadId; }
            set { this._threadId = value; }
        }

        /// <summary>Command </summary>
        public BrowserCommand Command
        {
            get { return this._command; }
            set { this._command = value; }
        }

        /// <summary>SecondsTimeout </summary>
        public int SecondsTimeout
        {
            get { return this._secondsTimeout; }
            set { this._secondsTimeout = value; }
        }

        /// <summary> Allow to AbortCommand </summary>
        public bool AbortCommand
        {
            get { return this._abortCommand; }
            set { this._abortCommand = value; }
        }
    }
}
