using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Method of matching strings when finding HtmlElements
    /// </summary>
    public enum MatchMethod
    {
        /// <summary>Literal</summary>
        Literal,
        /// <summary>EndsWidth</summary>
        EndsWith,
        /// <summary>Contains</summary>
        Contains,
        /// <summary>Regex</summary>
        Regex
    }
}
