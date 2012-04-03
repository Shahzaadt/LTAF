using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// The basic events as specified by W3C.
    /// </summary>
    public enum HtmlEventName
    {
        /// <summary>Load</summary>
        Load,
        /// <summary>UnLoad</summary>
		UnLoad,
        /// <summary>Focus</summary>
		Focus,
        /// <summary>Blur</summary>
		Blur,
        /// <summary>Change</summary>
		Change,
        /// <summary>Submit</summary>
		Submit,
        /// <summary>Reset</summary>
		Reset,
        /// <summary>Select</summary>
		Select,
        /// <summary>KeyPress</summary>
		KeyPress,
        /// <summary>KeyDown</summary>
		KeyDown,
        /// <summary>KeyUp</summary>
		KeyUp,
        /// <summary>MouseOver</summary>
		MouseOver,
        /// <summary>MouseOut</summary>
		MouseOut,
        /// <summary>MouseMove</summary>
		MouseMove,
        /// <summary>MouseDown</summary>
		MouseDown,
        /// <summary>MouseUp</summary>
		MouseUp,
        /// <summary>Click</summary>
		Click,
        /// <summary>DoubleClick</summary>
		DoubleClick
    }
}
