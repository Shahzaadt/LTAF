using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace LTAF
{
    /// <summary>
    /// HtmlAttributeDictionary
    /// </summary>
    public class HtmlAttributeDictionary: Dictionary<string, string>
    {
        internal HtmlAttributeDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        { 
        }

        /// <summary>
        /// ctor
        /// </summary>
        public HtmlAttributeDictionary(IDictionary<string, string> attributes)
            : base(attributes, StringComparer.OrdinalIgnoreCase)
        {   
        }

        #region Get
        /// <summary>
        /// Get the value of an attribute
        /// </summary>
        /// <typeparam name="T">Type of the attribute</typeparam>
        /// <param name="key">Attribute name</param>
        /// <returns>Value of the attribute</returns>
        public T Get<T>(string key)
        {
            return Get<T>(key, default(T));
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
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }

            string value = null;
            return (this.TryGetValue(key, out value)) ?
                Parse<T>(value, defaultValue) :
                defaultValue;
        }
        #endregion

        #region Parse
        /// <summary>
        /// Parse a string into a particular type of value
        /// </summary>
        /// <typeparam name="T">Type to parse the string into</typeparam>
        /// <param name="value">Value to parse</param>
        /// <param name="defaultValue">Default value to use if unable to parse</param>
        /// <returns>Parsed value</returns>
        private T Parse<T>(string value, T defaultValue)
        {
            object result;
            return (Parse(value, typeof(T), out result)) ?
                (T)result :
                defaultValue;
        }

        /// <summary>
        /// Parse a string into a particular type of value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <param name="type">Type to parse the string into</param>
        /// <param name="result">Parsed value</param>
        /// <returns>Whether or not parsing succeeded</returns>
        internal static bool Parse(string value, Type type, out object result)
        {
            result = null;

            // Handle nullable types seperately
            Type baseType = Nullable.GetUnderlyingType(type);
            if (baseType != null)
            {
                // If the string isn't empty, parse it as the base type
                // otherwise use the value null
                return (!string.IsNullOrEmpty(value)) ?
                    Parse(value, baseType, out result) :
                    true;
            }
            else
            {
                // Get the type converter
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                if (converter == null || !converter.CanConvertFrom(typeof(string)))
                {
                    return false;
                }

                // Parse the string
                result = converter.ConvertFromString(value);
                return true;
            }
        }
        #endregion Parse
    }
}
