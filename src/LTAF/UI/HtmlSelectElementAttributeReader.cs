using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Attribute reader that provides strong typed access to HtmlSelectElement properties
    /// </summary>
    public class HtmlSelectElementAttributeReader: HtmlElementAttributeReader
    {
        internal HtmlSelectElementAttributeReader(HtmlElement element)
            : base(element)
        {
        }

        /// <summary>
        /// TabIndex
        /// </summary>
        public int? TabIndex
        {
            get
            {
                return _attributes.Get<int?>("tabindex", null);
            }
        }

        /// <summary>
        /// Disabled
        /// </summary>
        public bool Disabled
        {
            get
            {
                if (_attributes.Get<string>("disabled") == "disabled")
                {
                    return true;
                }
                return _attributes.Get<bool>("disabled", false);
            }
        }

        /// <summary>
        /// Multiple
        /// </summary>
        public bool? Multiple
        {
            get
            {
                return _attributes.Get<bool?>("multiple", null);
            }
        }

        /// <summary>
        /// Size
        /// </summary>
        public int? Size
        {
            get
            {
                return _attributes.Get<int?>("size", null);
            }
        }

        /// <summary>
        /// Length
        /// </summary>
        public int? Length
        {
            get
            {
                return _attributes.Get<int?>("length", null);
            }
        }

        /// <summary>
        /// Value
        /// </summary>
        public string Value
        {
            get
            {
                return _attributes.Get<string>("value", null);
            }
        }

        /// <summary>
        /// SelectedIndex
        /// </summary>
        public int? SelectedIndex
        {
            get
            {
                return _attributes.Get<int?>("selectedIndex", null);
            }
        }
    }
}
