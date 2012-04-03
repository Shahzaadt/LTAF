using System;
using System.Collections.Generic;
using System.Text;
using LTAF.Engine;

namespace LTAF
{
    /// <summary>
    /// Attribute reader that provides strong typed access to HtmlElement properties
    /// </summary>
    public class HtmlElementAttributeReader
    {
        private HtmlStyle _style;
        private HtmlElement _element;
        /// <summary>HtmlAttributeDictionary</summary>
        protected HtmlAttributeDictionary _attributes;

        internal HtmlElementAttributeReader(HtmlElement element)
        {
            _element = element;
            _attributes = _element.AttributeDictionary;
            // rebuild the style object
            _style = new HtmlStyle();
            if (_attributes.ContainsKey("style"))
            {
                _style.LoadDescriptors(_attributes["style"]);
            }
        }

        /// <summary>
        /// Gets the attribute value for the specified key name
        /// </summary>
        public string this[string attributeName]
        {
            get
            {
                return _attributes[attributeName];
            }
        }


        internal IDictionary<string, string> Dictionary
        {
            get
            {
                return _attributes;
            }
        }

        #region Get<T>
        /// <summary>
        /// Get the value of an attribute
        /// </summary>
        /// <typeparam name="T">Type of the attribute</typeparam>
        /// <param name="key">Attribute name</param>
        /// <returns>Value of the attribute</returns>
        public T Get<T>(string key)
        {
            return _attributes.Get<T>(key);
        }

        /// <summary>
        /// Get the value of an attribute 
        /// </summary>
        /// <typeparam name="T">Type of the attribute</typeparam>
        /// <param name="key">Attribute name</param>
        /// <param name="defaultValue">Default value if attribute not found</param>
        /// <returns>Value of the attribute</returns>
        public T Get<T>(string key, T defaultValue)
        {
            return _attributes.Get<T>(key, defaultValue);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The id of this element as exposed by the dom
        /// </summary>
        public string Id
        {
            get
            {
                return _attributes.Get<string>("id", null);
            }
        }

        /// <summary>
        /// The name of this element as exposed by the dom.
        /// </summary>
        public string Name
        {
            get
            {
                return _attributes.Get<string>("name", null);
            }
        }

        /// <summary>
        /// The class of this element as exposed by the dom.
        /// </summary>
        public string Class
        {
            get
            {
                return _attributes.Get<string>("class", null);
            }
        }

        /// <summary>
        /// The title of this element as exposed by the dom.
        /// </summary>
        public string Title
        {
            get
            {
                return _attributes.Get<string>("title", null);
            }
        }

        /// <summary>
        /// An HtmlStyle object that encapsulates the style propertis of this element.
        /// </summary>
        public HtmlStyle Style
        {
            get 
            { 
                return _style; 
            }
        }
        #endregion
    }
}
