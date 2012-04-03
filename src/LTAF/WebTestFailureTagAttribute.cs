using System;

namespace LTAF
{
    /// <summary>
    /// WebTestFailureTag attribute used to associate test cases with failures
    /// in specific sets of browsers so they can be properly tracked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class WebTestFailureTagAttribute : Attribute
    {
        /// <summary>
        /// Browsers the test cases fail in
        /// </summary>
        public BrowserVersions Browsers
        {
            get { return _browsers; }
        }
        private BrowserVersions _browsers;

        /// <summary>
        /// Description of the failure
        /// </summary>
        public string Description
        {
            get { return _description; }
        }
        private string _description;

        /// <summary>
        /// WebTestFailureTag attribute used to associate test cases with failures
        /// in specific sets of browsers so they can be properly tracked.
        /// </summary>
        /// <param name="browsers">Browsers the test cases fail in</param>
        /// <param name="description">Description of the failure</param>
        public WebTestFailureTagAttribute(BrowserVersions browsers, string description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description", "Description cannot be null!");
            }
            else if (description.Length == 0)
            {
                throw new ArgumentException("Description cannot be empty!", "description");
            }

            _browsers = browsers;
            _description = description;
        }
    }
}