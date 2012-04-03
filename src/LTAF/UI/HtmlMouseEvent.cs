using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Reperesents a mouse event to be dispatched to an element.
    /// </summary>
    public class HtmlMouseEvent: HtmlEvent
    {
        /// <summary>
        /// ctor
        /// </summary>
        public HtmlMouseEvent(string eventName): base(eventName)
        {

        }
    }
}
