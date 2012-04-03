using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Reperesents an event to be dispatched to an element
    /// </summary>
    public class HtmlEvent
    {
        private string _eventName;

        /// <summary>
        /// ctor
        /// </summary>
        public HtmlEvent(string eventName)
        {
            _eventName = eventName.ToLower();
        }

        /// <summary>
        /// Create
        /// </summary>        
        public static HtmlEvent Create(HtmlEventName eventName)
        {
            switch (eventName)
            {
                case HtmlEventName.Click:
                case HtmlEventName.DoubleClick:
                case HtmlEventName.MouseDown:
                case HtmlEventName.MouseMove:
                case HtmlEventName.MouseOut:
                case HtmlEventName.MouseOver:
                case HtmlEventName.MouseUp:
                    return new HtmlMouseEvent(eventName.ToString());
                default:
                    return new HtmlEvent(eventName.ToString());
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return _eventName; }
            set { _eventName = value; }
        }
    }
}
