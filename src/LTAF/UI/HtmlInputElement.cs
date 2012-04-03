using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Class that represents the 'input' html element.
    /// </summary>
    public class HtmlInputElement: HtmlElement
    {
        internal HtmlInputElement(
            IDictionary<string, string> attributes,
            HtmlElement parentElement,
            HtmlPage parentPage)
            : base("input", attributes, parentElement, parentPage)
        {
        }

        /// <summary>
        /// Whether the elements supports the set text command
        /// </summary>
        protected override bool CanSetText
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Property that returns the inmemory attribute reader for this element, containing only the attributes that were retrieved on last GetAttributes. 
        /// Meant to be used by infraestructure utilities.
        /// </summary>
        public new HtmlInputElementAttributeReader CachedAttributes
        {
            get
            {
                _attributeReader = _attributeReader as HtmlInputElementAttributeReader;
                if (_attributeReader == null)
                {
                    _attributeReader = new HtmlInputElementAttributeReader(this);
                }

                return (HtmlInputElementAttributeReader)_attributeReader;                
            }
        }

        /// <summary>
        /// Get the attributes for the input element
        /// </summary>
        /// <returns>HtmlInputElementAttributeReader</returns>
        public new HtmlInputElementAttributeReader GetAttributes()
        {
            this.RefreshAttributesDictionary();
            _attributeReader = new HtmlInputElementAttributeReader(this);
            return (HtmlInputElementAttributeReader)_attributeReader;
        }
    }
}
