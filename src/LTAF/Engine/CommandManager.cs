using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LTAF.Engine
{
    /// <summary>
    /// BrowserCommands are created by tests running on the server and passed
    /// down to the client to be executed.  The client executes the command and
    /// passes the result back as a BrowserInfo object.  The CommandManager is
    /// used to synchronize the communication between the server tests and the
    /// web services called by the client browsers.
    /// </summary>
    public static class CommandManager
    {
        /// <summary>
        /// The CommandManager is used to synchronize multiple different test
        /// runs in different browsers.  The SynchronizationEntry class is used
        /// to synchronize a single browser's test pass.
        /// </summary>
        public class SynchronizationEntry
        {
            /// <summary>
            /// Command that was sent to the client
            /// </summary>
            public BrowserCommand Command
            {
                get { return _command; }
                set { _command = value; }
            }
            private BrowserCommand _command;

            /// <summary>
            /// Result of executing the command on the client
            /// </summary>
            public BrowserInfo Result
            {
                get { return _result; }
                set { _result = value; }
            }
            private BrowserInfo _result;

            /// <summary>
            /// Whether the first command has been sent down for processing
            /// </summary>
            public bool Started
            {
                get { return _started; }
                set { _started = true; }
            }
            private bool _started = false;

            /// <summary>
            /// Whether the last command was a TestRunFinished command and the
            /// client web service should not wait for additional commands
            /// </summary>
            public bool Finished
            {
                get { return _finished; }
                set { _finished = true; }
            }
            private bool _finished = false;

            /// <summary>
            /// Object used to synchronize communication between the test code
            /// on the server and the web service called by the client
            /// </summary>
            public object SyncRoot
            {
                get { return _syncRoot; }
            }
            private object _syncRoot = new object();
        }

        /// <summary>
        /// Length of time in milliseconds to wait for the next command before
        /// timing out
        /// </summary>
        private const int NextCommandTimeout = 60000;

        /// <summary>
        /// Global list of synchronization entries for all browsers
        /// </summary>
        private static Dictionary<int, SynchronizationEntry> _entries = new Dictionary<int, SynchronizationEntry>();

        /// <summary>
        /// Explicit synchronization root for the global list of entries
        /// </summary>
        private static object _syncRoot = new object();

        /// <summary>
        /// Get the synchronization entry for a given browser
        /// </summary>
        /// <param name="threadId">Thread of the browser</param>
        /// <returns>SynchronizationEntry for the browser</returns>
        private static SynchronizationEntry GetEntry(int threadId)
        {
            return GetEntry(threadId, true);
        }

        /// <summary>
        /// Get the synchronization entry for a given browser
        /// </summary>
        /// <param name="threadId">Thread of the browser</param>
        /// <param name="throwIfNotFound">Whether to throw an exception if there is no entry for the thread or just return null</param>
        /// <returns>SynchronizationEntry for the browser</returns>
        public static SynchronizationEntry GetEntry(int threadId, bool throwIfNotFound)
        {
            lock (_syncRoot)
            {
                SynchronizationEntry entry;
                if (!_entries.TryGetValue(threadId, out entry))
                {
                    if (throwIfNotFound)
                    {
                        throw new InvalidOperationException("CommandManager does not have a BrowserQueue registered for threadId = " + threadId.ToString());
                    }
                    return null;
                }
                return entry;
            }
        }
        
        /// <summary>
        /// Setup the CommandManager for a new test pass for a browser on
        /// the given thread
        /// </summary>
        /// <param name="threadId">Server thread handling the tests for the browser</param>
        public static void CreateBrowserQueue(int threadId)
        {
            lock (_syncRoot)
            {
                SynchronizationEntry entry = GetEntry(threadId, false);
                if (entry != null)
                {
                    // Destroy the old queue (which will release anyone waiting
                    // on its synchronization object)
                    DestroyBrowserQueue(threadId);
                }

                entry = new SynchronizationEntry();
                _entries.Add(threadId, entry);
            }
        }

        /// <summary>
        /// Destroy all browser queues
        /// </summary>
        internal static void DestroyAllBrowserQueues()
        {
            lock (_syncRoot)
            {
                List<int> keys = new List<int>(_entries.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    DestroyBrowserQueue(keys[i]);
                }
            }
        }

        /// <summary>
        /// Destroy a browser's command queue
        /// </summary>
        /// <param name="threadId">Server thread handling the tests for the browser</param>
        internal static void DestroyBrowserQueue(int threadId)
        {
            lock (_syncRoot)
            {
                SynchronizationEntry entry = GetEntry(threadId);
                lock (entry.SyncRoot)
                {
                    entry.Command = null;
                    entry.Result = null;

                    // Release anyone waiting to synchronize on the entry
                    Monitor.PulseAll(entry.SyncRoot);

                    _entries.Remove(threadId);
                }
            }
        }

        /// <summary>
        /// Execute a commmand on the client and wait for its result
        /// </summary>
        /// <param name="threadId">Server thread handling the tests for the browser</param>
        /// <param name="command">Command to execute</param>
        /// <param name="secondsTimeout">Maximum timeout </param>
        /// <returns>Result of executing the command on the client</returns>
        /// <remarks>
        /// This method works hand in hand with SetResultAndGetNextCommand.
        /// They wait and signal each other.
        /// </remarks>
        public static BrowserInfo ExecuteCommand(int threadId, BrowserCommand command, int secondsTimeout)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command", "BrowserCommand cannot be null!");
            }

            SynchronizationEntry entry = null;
            lock (_syncRoot)
            {
                entry = GetEntry(threadId);
            }

            lock (entry.SyncRoot)
            {
                // Set the command and notify anyone waiting for the next command
                // (also setting whether this command marks the end of a test run)
                entry.Command = command;
                if (string.CompareOrdinal(entry.Command.Handler.ClientFunctionName, BrowserCommand.FunctionNames.TestRunFinished) == 0)
                {
                    entry.Finished = true;
                }
                Monitor.Pulse(entry.SyncRoot);

                // Wait for the command's result
                if (!Monitor.Wait(entry.SyncRoot, secondsTimeout * 1000))
                {
                    // Use null if the command timed out
                    entry.Result = null;
                }

                BrowserInfo result = entry.Result;
                return result;
            }
        }

        /// <summary>
        /// Post the result of the last executed command and wait for the next
        /// command or return no command if 
        /// </summary>
        /// <param name="threadId">Server thread handling the tests for the browser</param>
        /// <param name="result">Result of the executing the last command</param>
        /// <returns>Next command to execute (or null if there are no more commands or we timed out while waiting)</returns>
        /// <remarks>
        /// This method works hand in hand with ExecuteCommand. They wait and
        /// signal each other.
        /// </remarks>
        public static BrowserCommand SetResultAndGetNextCommand(int threadId, BrowserInfo result)
        {
            SynchronizationEntry entry = null;
            lock (_syncRoot)
            {
                entry = GetEntry(threadId);
            }

            lock (entry.SyncRoot)
            {
                // Get the next command to execute
                BrowserCommand command = null;
                if (!entry.Started)
                {
                    // If this is the first request, a command has already been queued
                    entry.Started = true;
                    command = entry.Command;
                }
                else
                {
                    // Update the command that was waiting for its result
                    entry.Result = result;
                    Monitor.Pulse(entry.SyncRoot);

                    // Wait for the next command (only if the last command didn't
                    // signal the end of the test run)
                    if (!entry.Finished && Monitor.Wait(entry.SyncRoot, NextCommandTimeout))
                    {
                        command = entry.Command;
                    }
                }

                entry.Command = null;
                return command;
            }
        }

        /// <summary>
        /// Retrieve the result of running the last test 
        /// </summary>
        /// <param name="threadId">Server thread handling the tests for the browser</param>
        /// <returns>BrowserInfo for the last executed command</returns>
        internal static BrowserInfo GetBrowserInfo(int threadId)
        {
            lock (_syncRoot)
            {
                return GetEntry(threadId).Result;
            }
        }
    }
}