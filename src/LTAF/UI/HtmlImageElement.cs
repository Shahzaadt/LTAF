using System;
using System.Collections.Generic;

namespace LTAF
{
    /// <summary>
    /// Represents 'img' elements
    /// </summary>
    public class HtmlImageElement : HtmlElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="parentElement">Parent element</param>
        /// <param name="parentPage">Parent page</param>
        internal HtmlImageElement(IDictionary<string, string> attributes, HtmlElement parentElement, HtmlPage parentPage)
            : base("img", attributes, parentElement, parentPage)
        {
        }

        /// <summary>
        /// Property that returns the inmemory attribute reader for this element, containing only the attributes that were retrieved on last GetAttributes. 
        /// Meant to be used by infraestructure utilities.
        /// </summary>
        public new HtmlImageElementAttributeReader CachedAttributes
        {
            get
            {
                _attributeReader = _attributeReader as HtmlImageElementAttributeReader;
                if (_attributeReader == null)
                {
                    _attributeReader = new HtmlImageElementAttributeReader(this);
                }

                return (HtmlImageElementAttributeReader)_attributeReader;
            }
        }

        /// <summary>
        /// Get the attributes for the image element
        /// </summary>
        /// <returns>HtmlImageElementAttributeReader</returns>
        public new HtmlImageElementAttributeReader GetAttributes()
        {
            this.RefreshAttributesDictionary();
            _attributeReader = new HtmlImageElementAttributeReader(this);
            return (HtmlImageElementAttributeReader)_attributeReader;
        }

    }
}