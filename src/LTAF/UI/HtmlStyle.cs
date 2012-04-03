using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Drawing;

namespace LTAF
{
    /// <summary>
    /// Represents the style information of an html element.
    /// </summary>
    /// <remarks>
    /// http://www.w3.org/TR/REC-CSS2/propidx.html
    /// </remarks>
    public class HtmlStyle
    {
        private string _style;
        private HtmlAttributeDictionary _descriptors;

        internal HtmlStyle()
        {
            _descriptors = new HtmlAttributeDictionary();
        }

        internal void LoadDescriptors(string style)
        {
            _style = style;
            string[] descriptors = _style.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string desc in descriptors)
            {
                string[] keyValue = desc.Split(':');
                if (keyValue.Length == 2)
                {
                    _descriptors.Add(keyValue[0].Trim().Replace("-","").ToLowerInvariant(), keyValue[1].Trim().ToLowerInvariant());
                }
            }
        }

        #region Get<T>
        /// <summary>
        /// Get the value of a style descriptor
        /// </summary>
        /// <typeparam name="T">Type of the style descriptor</typeparam>
        /// <param name="key">Style descriptor name</param>
        /// <returns>Value of the style descriptor</returns>
        public T Get<T>(string key)
        {
            return _descriptors.Get<T>(key);
        }

        /// <summary>
        /// Get the value of a style descriptor
        /// </summary>
        /// <typeparam name="T">Type of the style descriptor</typeparam>
        /// <param name="key">Style descriptor name</param>
        /// <param name="defaultValue">Default value if attribute not found</param>
        /// <returns>Value of the style descriptor</returns>
        public T Get<T>(string key, T defaultValue)
        {
            return _descriptors.Get<T>(key, defaultValue);
        }
        #endregion

        /// <summary>
        /// The css style as a string
        /// </summary>
        public string RawStyle
        {
            get
            {
                return _style;
            }
        }

        internal IDictionary<string, string> Descriptors
        {
            get { return _descriptors; }
        }

        /// <summary>
        /// Visibility descriptor
        /// </summary>
        public Visibility Visibility
        {
            get
            {
                return _descriptors.Get<Visibility>("visibility", Visibility.NotSet);
            }
        }

        /// <summary>
        /// Position descriptor
        /// </summary>
        public Position Position
        {
            get
            {
                return _descriptors.Get<Position>("position", Position.NotSet);
            }
        }

        /// <summary>
        /// Background descriptor
        /// </summary>
        public System.Drawing.Color BackgroundColor
        {
            get
            {
                return _descriptors.Get<Color>("backgroundcolor", Color.Empty);
            }
        }

        /// <summary>
        /// BackgroundImage descriptor
        /// </summary>
        public string BackgroundImage
        {
            get
            {
                return _descriptors.Get<string>("backgroundimage", null);
            }
        }

        /// <summary>
        /// Color descriptor
        /// </summary>
        public System.Drawing.Color Color
        {
            get
            {
                return _descriptors.Get<Color>("color", Color.Empty);
            }
        }

        /// <summary>
        /// Bottom descriptor
        /// </summary>
        public Unit Bottom
        {
            get
            {
                return _descriptors.Get<Unit>("bottom", Unit.Empty);
            }
        }

        /// <summary>
        /// Left descriptor
        /// </summary>
        public Unit Left
        {
            get
            {
                return _descriptors.Get<Unit>("left", Unit.Empty);
            }
        }

        /// <summary>
        /// Right descriptor
        /// </summary>
        public Unit Right
        {
            get
            {
                return _descriptors.Get<Unit>("right", Unit.Empty);
            }
        }

        /// <summary>
        /// Top descriptor
        /// </summary>
        public Unit Top
        {
            get
            {
                return _descriptors.Get<Unit>("top", Unit.Empty);
            }
        }

        /// <summary>
        /// BorderSpacing descriptor
        /// </summary>
        public Unit BorderSpacing
        {
            get
            {
                return _descriptors.Get<Unit>("borderspacing", Unit.Empty);
            }
        }

        /// <summary>
        /// Height descriptor
        /// </summary>
        public Unit Height
        {
            get
            {
                return _descriptors.Get<Unit>("height", Unit.Empty);
            }
        }

        /// <summary>
        /// Width descriptor
        /// </summary>
        public Unit Width
        {
            get
            {
                return _descriptors.Get<Unit>("width", Unit.Empty);
            }
        }

        /// <summary>
        /// PaddingTop descriptor
        /// </summary>
        public Unit PaddingTop
        {
            get
            {
                return _descriptors.Get<Unit>("paddingtop", Unit.Empty);
            }
        }

        /// <summary>
        /// PaddingLeft descriptor
        /// </summary>
        public Unit PaddingLeft
        {
            get
            {
                return _descriptors.Get<Unit>("paddingleft", Unit.Empty);
            }
        }

        /// <summary>
        /// PaddingRight descriptor
        /// </summary>
        public Unit PaddingRight
        {
            get
            {
                return _descriptors.Get<Unit>("paddingright", Unit.Empty);
            }
        }

        /// <summary>
        /// PaddingBottom descriptor
        /// </summary>
        public Unit PaddingBottom
        {
            get
            {
                return _descriptors.Get<Unit>("paddingbottom", Unit.Empty);
            }
        }

        /// <summary>
        /// Size descriptor
        /// </summary>
        public Unit Size
        {
            get
            {
                return _descriptors.Get<Unit>("size", Unit.Empty);
            }
        }

        /// <summary>
        /// TextIndent descriptor
        /// </summary>
        public Unit TextIndent
        {
            get
            {
                return _descriptors.Get<Unit>("textindent", Unit.Empty);
            }
        }

        /// <summary>
        /// Align descriptor
        /// </summary>
        public HorizontalAlign Align
        {
            get
            {
                return _descriptors.Get<HorizontalAlign>("align", HorizontalAlign.NotSet);
            }
        }

        /// <summary>
        /// VerticalAlign descriptor
        /// </summary>
        public VerticalAlign VerticalAlign
        {
            get
            {
                return _descriptors.Get<VerticalAlign>("verticalalign", VerticalAlign.NotSet);
            }
        }

        /// <summary>
        /// WhiteSpace descriptor
        /// </summary>
        public WhiteSpace WhiteSpace
        {
            get
            {
                return _descriptors.Get<WhiteSpace>("whitespace", WhiteSpace.NotSet);
            }
        }

        /// <summary>
        /// Display descriptor
        /// </summary>
        public Display Display
        {
            get
            {
                return _descriptors.Get<Display>("display", Display.NotSet);
            }
        }

        /// <summary>
        /// Overflow descriptor
        /// </summary>
        public Overflow Overflow
        {
            get
            {
                return _descriptors.Get<Overflow>("overflow", Overflow.NotSet);
            }
        }
    }
}
