using System;

namespace LTAF
{
    /// <summary>
    /// Amount of detail to include in the test log
    /// </summary>
    public enum WebTestLogDetail
    {
        /// <summary>
        /// Just a list of tests
        /// </summary>
        Concise = 0,

        /// <summary>
        /// Standard list of tests and commands
        /// </summary>
        Default = 1,

        /// <summary>
        /// List of tests, commands, and details for commands
        /// </summary>
        Verbose = 2
    }
}