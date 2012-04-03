using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Provides access to interact with the test console
    /// </summary>
    public class WebTestConsole
    {
        private static List<string> _messages;

        static WebTestConsole()
        {
            _messages = new List<string>();
        }

        /// <summary>
        /// Adds a message to the list to be sent to the console with next command.
        /// </summary>
        /// <param name="message">The message to write</param>
        public static void Write(string message)
        {
            _messages.Add(message);
        }

        /// <summary>
        /// Clears the list of messages
        /// </summary>
        internal static void Clear()
        {
            _messages.Clear();
        }

        /// <summary>
        /// Retrieves the list of messages
        /// </summary>
        internal static string[] GetTraces()
        {
            return _messages.ToArray();
        }
    }
}
