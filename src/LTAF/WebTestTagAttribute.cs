using System;

namespace LTAF
{
    /// <summary>
    /// WebTestTag attribute used to associate individual test cases
    /// with tags to easily test related functionality
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class WebTestTagAttribute : Attribute
    {
        /// <summary>
        /// Tag used to associate test cases together
        /// </summary>
        public string Tag
        {
            get { return _tag; }
        }
        private string _tag;

        /// <summary>
        /// WebTestTag attribute used to associate individual test
        /// cases with tags to easily test related functionality
        /// </summary>
        /// <param name="tag">Tag used to associate test cases together</param>
        public WebTestTagAttribute(string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException("tag", "Tag cannot be null!");
            }
            else if (tag.Length == 0)
            {
                throw new ArgumentException("Tag cannot be empty!", "tag");
            }
            else if (string.Compare(tag, "all", StringComparison.OrdinalIgnoreCase) == 0)
            {
                throw new ArgumentException("Tag \"All\" is reserved!", "tag");
            }

            int invaldIndex = tag.IndexOfAny(new char[] { '@', '-', '^', '!', '(', ')', '/', '&', '=', '+' });
            if (invaldIndex >= 0)
            {
                throw new ArgumentException(
                    string.Format("The tag \"{0}\" contains the invalid character \"{1}\"!", tag, tag[invaldIndex]),
                    "tag");
            }

            _tag = tag;
        }
    }
}