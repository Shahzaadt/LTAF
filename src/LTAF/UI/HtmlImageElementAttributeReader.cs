using System;
using System.Web;

namespace LTAF
{
    /// <summary>
    /// Attribute reader for HtmlImageElement
    /// </summary>
    public class HtmlImageElementAttributeReader : HtmlElementAttributeReader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">Attributes</param>
        internal HtmlImageElementAttributeReader(HtmlElement element)
            : base(element)
        {
        }

        /// <summary>
        /// Image source attribute
        /// </summary>
        public string Source
        {
            get { return _attributes.Get<string>("src", null) ?? _attributes.Get<string>("href", null); }
        }

        /// <summary>
        /// Image alternate text attribute
        /// </summary>
        public string AlternateText
        {
            get { return _attributes.Get<string>("alt", null); }
        }
    }
}