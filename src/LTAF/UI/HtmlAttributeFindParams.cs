using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Holds information about how to match html attributes when finding elements
    /// </summary>
    public class HtmlAttributeFindParams
    {
        /// <summary>The name of the attribute to find</summary>
        public string Name { get; set; }

        /// <summary>The value of the attribute to find</summary>
        public string Value { get; set; }

        /// <summary>The match method to apply when comparing the value</summary>
        public MatchMethod ValueMatchMethod { get; set; }

        /// <summary>
        /// Constructor that takes name/value pair and defaults to Literal match method
        /// </summary>
        public HtmlAttributeFindParams(string name, string value)
            : this(name, value, MatchMethod.Literal)
        {
        }

        /// <summary>
        /// Constructor that takes name/value pair and the match method to use
        /// </summary>
        public HtmlAttributeFindParams(string name, string value, MatchMethod valueMatchMethod)
        {
            this.Name = name;
            this.Value = value;
            this.ValueMatchMethod = valueMatchMethod;
        }
    }
}
