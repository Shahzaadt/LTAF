using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Visibility
    /// </summary>
    public enum Visibility
    {
        /// <summary>Visible</summary>
        Visible,
        /// <summary>Hidden</summary>        	 
        Hidden,
        /// <summary>Collapse</summary>
        Collapse,
        /// <summary>NotSet</summary>
        NotSet
    }

    /// <summary>Display</summary>
    public enum Display
    {
        /// <summary>Block</summary>
        Block,
        /// <summary>Inline</summary>
        Inline,
        /// <summary>None</summary>
        None,
        /// <summary>NotSet</summary>
        NotSet
    }

    /// <summary>Position</summary>
    public enum Position
    {
        /// <summary>Static</summary>
        Static,
        /// <summary>Relative</summary>
        Relative,
        /// <summary>Absolute</summary>
        Absolute,
        /// <summary>Fixed</summary>
        Fixed,
        /// <summary>NotSet</summary>
        NotSet
    }

    /// <summary>WhiteSpace</summary>
    public enum WhiteSpace
    {
        /// <summary>Normal</summary>
        Normal,
        /// <summary>Pre</summary>
        Pre,
        /// <summary>NoWrap</summary>
        NoWrap,
        /// <summary>NotSet</summary>
        NotSet
    }

    /// <summary>Overflow</summary>
    public enum Overflow
    {
        /// <summary>Visible</summary>
        Visible,
        /// <summary>Hidden</summary>
        Hidden,
        /// <summary>Scroll</summary>
        Scroll,
        /// <summary>Auto</summary>
        Auto,
        /// <summary>NotSet</summary>
        NotSet
    }
}
