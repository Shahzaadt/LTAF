using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Attribute reader that provides strong typed access to HtmlSelectElement properties
    /// </summary>
    public class HtmlOptionElementAttributeReader : HtmlElementAttributeReader
    {
        private bool _selected;

        internal HtmlOptionElementAttributeReader(HtmlElement element)
            : base(element)
        {
            var selectedValue = _attributes.Get<string>("selected");
            if (selectedValue != null &&
                (selectedValue.Equals("selected", StringComparison.OrdinalIgnoreCase) || selectedValue == string.Empty))
            {
                _selected = true;
            }
            else
            {
                _selected = _attributes.Get<bool>("selected", false);
            }
        }

        /// <summary>
        /// Specifies if option is selected or not
        /// </summary>
        public bool Selected
        {
            get
            {
                return _selected;
            }
            internal set
            {
                _selected = value;
            }
        }
    }
}
