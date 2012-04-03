using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LTAF.Engine
{
    /// <summary>
    /// Writes test execution log to disk
    /// </summary>
    internal class TestcaseLogger
    {
        private const string LIGHTRUNNER_LOG_NAME = "TestLog.txt";
        private const string STARTUP_FILE_NAME = "Startup.txt";

        private readonly string _testPath;
        private StringBuilder _lightRunnerErrorLog;
        private IFileSystem _fileSystem;

        public TestcaseLogger(string pathToLog, TestcaseExecutor testExecutor): 
            this(pathToLog, testExecutor, new FileSystem())
        {
        }

        public TestcaseLogger(string pathToLog, TestcaseExecutor testExecutor, IFileSystem fileSystem)
        {
            _testPath = pathToLog;
            _lightRunnerErrorLog = new StringBuilder();
            _fileSystem = fileSystem;
            testExecutor.TestcaseExecuted += new EventHandler<TestcaseExecutionEventArgs>(LogTestcaseExecuted);
            testExecutor.TestRunFinished += new EventHandler<TestRunFinishedEventArgs>(LogTestRunFinished);
        }

        /// <summary>
        /// Writes the startup file
        /// </summary>
        public void WriteStartupFile()
        {
            string startupFilePath = Path.Combine(_testPath, STARTUP_FILE_NAME);
            _fileSystem.WriteAllText(startupFilePath, DateTime.Now.ToString());
        }

        public void LogTestRunFinished(object sender, TestRunFinishedEventArgs e)
        {
            _lightRunnerErrorLog.Append(String.Format("Test Run Finished. [{0}]", DateTime.Now));
            _fileSystem.WriteAllText(Path.Combine(_testPath, LIGHTRUNNER_LOG_NAME), _lightRunnerErrorLog.ToString());
        }

        public void LogTestcaseExecuted(object sender, TestcaseExecutionEventArgs e)
        {
            if (!e.Passed)
            {
                _lightRunnerErrorLog.AppendLine("---- Testcase FAILED '" + e.WebTestName + "' ----");
                _lightRunnerErrorLog.AppendLine(e.Exception.ToString());
            }
            else
            {
                _lightRunnerErrorLog.AppendLine("---- Testcase PASSED '" + e.WebTestName + "' ----");
            }
        }

    }
}