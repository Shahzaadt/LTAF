using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Attribute reader that provides strong typed access to HtmlInputElement properties
    /// </summary>
    public class HtmlInputElementAttributeReader: HtmlElementAttributeReader
    {
        /// <summary>The current "value" attribute</summary>
        private string _value;
        /// <summary>Specifies if checked attribute was set or present</summary>
        private bool _checked;
        /// <summary>Specifies if disabled attribute was set or present</summary>
        private bool _disabled;

        internal HtmlInputElementAttributeReader(HtmlElement element)
            : base(element)
        {
            _value = _attributes.Get<string>("value", "");

            var checkedValue = _attributes.Get<string>("checked");
            if (checkedValue != null &&
                (checkedValue.Equals("checked", StringComparison.OrdinalIgnoreCase) || checkedValue == string.Empty))
            {
                _checked = true;
            }
            else
            {
                _checked = _attributes.Get<bool>("checked", false);
            }

            var disabledValue = _attributes.Get<string>("disabled");
            if (disabledValue != null &&
                (disabledValue.Equals("disabled", StringComparison.OrdinalIgnoreCase) || disabledValue == string.Empty))
            {
                _disabled = true;
            }
            else
            {
                _disabled = _attributes.Get<bool>("disabled", false);
            }

        }

        /// <summary>
        /// AccessKey
        /// </summary>
        public string AccessKey
        {
            get
            {
                return _attributes.Get<string>("accesskey",null);
            }
        }

        /// <summary>
        /// Alt
        /// </summary>
        public string Alt
        {
            get
            {
                return _attributes.Get<string>("alt",null);
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
        /// MaxLength
        /// </summary>
        public int? MaxLength
        {
            get
            {
                return _attributes.Get<int?>("maxlength", null);
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
        /// Value
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
            internal set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Checked
        /// </summary>
        public bool Checked
        {
            get
            {
                return _checked;
            }
            internal set
            {
                _checked = value;
            }
        }

        /// <summary>
        /// Disabled
        /// </summary>
        public bool Disabled
        {
            get
            {
                return _disabled;
            }
        }

        /// <summary>ReadOnly</summary>
        public bool? ReadOnly
        {
            get
            {
                return _attributes.Get<bool?>("readonly", null);
            }
        }

        /// <summary>
        /// DefaultValue
        /// </summary>
        public string DefaultValue
        {
            get
            {
                return _attributes.Get<string>("defaultvalue", null);
            }
        }

        /// <summary>
        /// DefaultChecked
        /// </summary>
        public bool? DefaultChecked
        {
            get
            {
                return _attributes.Get<bool?>("defaultchecked", null);
            }
        }

        /// <summary>
        /// Type
        /// </summary>
        public HtmlInputElementType Type
        {
            get
            {
                return _attributes.Get<HtmlInputElementType>("type", HtmlInputElementType.NotSet);
            }
        }
    }
}
