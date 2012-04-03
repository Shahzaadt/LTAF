using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Marks a method as an automated web testcase.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class WebTestMethodAttribute : Attribute
    {

        /// <summary>
        /// Description of the failure
        /// </summary>
        public string Description
        {
            get { return _description; }
        }
        private string _description;

        /// <summary>
        /// Marks a method as an automated web testcase.
        /// </summary>
        public WebTestMethodAttribute()
        {
        }

        /// <summary>
        /// Marks a method as an automated web testcase and sets a scenario description.
        /// </summary>
        /// <param name="description">Description of the scenario</param>
        public WebTestMethodAttribute(string description)
        {
            _description = description;
        }

    }
}