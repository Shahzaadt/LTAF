using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Marks a class a container of automated web testcases.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WebTestClassAttribute: Attribute
    {
        /// <summary>
        /// Testcase ID associated with the class
        /// </summary>
        public int TestId
        {
            get;
            set;
        }

        /// <summary>
        /// Description of this testcase
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Marks a class a container of automated web testcases.
        /// </summary>
        public WebTestClassAttribute()
        {
        }

        /// <summary>
        /// Marks a class a container of automated web testcases.
        /// </summary>
        public WebTestClassAttribute(int testID)
        {
            TestId = testID;
        }

        /// <summary>
        /// Marks a class a container of automated web testcases.
        /// </summary>
        /// <param name="description">Description for this test</param>
        public WebTestClassAttribute(string description)
        {
            Description = description;
        }
    }
}