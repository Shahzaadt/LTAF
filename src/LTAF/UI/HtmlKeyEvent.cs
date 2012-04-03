using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Reperesents a key event to be dispatched to an element.
    /// </summary>
    public class HtmlKeyEvent : HtmlEvent
    {
        /// <summary>
        /// CtrlKey
        /// </summary>
        public bool CtrlKey
        {
            get { return _ctrlKey; }
            set { _ctrlKey = value; }
        }
        private bool _ctrlKey = false;

        /// <summary>
        /// AltKey
        /// </summary>
        public bool AltKey
        {
            get { return _altKey; }
            set { _altKey = value; }
        }
        private bool _altKey = false;

        /// <summary>
        /// ShiftKey
        /// </summary>
        public bool ShiftKey
        {
            get { return _shiftKey; }
            set { _shiftKey = value; }
        }
        private bool _shiftKey = false;

        /// <summary>
        /// MetaKey
        /// </summary>
        public bool MetaKey
        {
            get { return _metaKey; }
            set { _metaKey = value; }
        }
        private bool _metaKey = false;

        /// <summary>
        /// KeyCode
        /// </summary>
        public ulong KeyCode
        {
            get { return _keyCode; }
            set { _keyCode = value; }
        }
        private ulong _keyCode = 0;

        /// <summary>
        /// CharCode
        /// </summary>
        public ulong CharCode
        {
            get { return _charCode; }
            set { _charCode = value; }
        }
        private ulong _charCode = 0;

        /// <summary>
        /// ctor
        /// </summary>
        public HtmlKeyEvent(string eventName): base(eventName)
        {
        }
    }
}
