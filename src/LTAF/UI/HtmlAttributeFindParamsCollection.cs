using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Collection of HtmlAttributeFindParms to use when finding an element
    /// </summary>
    public class HtmlAttributeFindParamsCollection : List<HtmlAttributeFindParams>
    {
        /// <summary>
        /// Add an attribute name/value pair to use when searching
        /// </summary>
        public void Add(string name, string value)
        {
            this.Add(name, value, MatchMethod.Literal);
        }

        /// <summary>
        /// Add an attribute name/value pair with a specific match method to use when searching
        /// </summary>
        public void Add(string name, string value, MatchMethod valueMatchMethod)
        {
            this.Add(new HtmlAttributeFindParams(name, value, valueMatchMethod));
        }
    }
}
