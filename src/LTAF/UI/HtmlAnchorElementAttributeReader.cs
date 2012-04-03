using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Attribute reader that provides strong typed access to HtmlAnchorElement properties
    /// </summary>
    public class HtmlAnchorElementAttributeReader: HtmlElementAttributeReader
    {
        internal HtmlAnchorElementAttributeReader(HtmlElement element)
            : base(element)
        {
        }

        /// <summary>
        /// AccessKey
        /// </summary>
        public string AccessKey
        {
            get
            {
                return _attributes.Get<string>("accesskey", null);
            }
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
        /// HRef
        /// </summary>
        public string HRef
        {
            get
            {
                return _attributes.Get<string>("href", null);
            }
        }

        /// <summary>
        /// Target
        /// </summary>
        public string Target
        {
            get
            {
                return _attributes.Get<string>("target", null);
            }
        }
    }
}
