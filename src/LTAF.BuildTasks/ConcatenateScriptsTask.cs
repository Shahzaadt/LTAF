using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace LTAF.BuildTasks
{
    public class ConcatenateScriptsTask: Task
    {
        private string[] _scriptFiles = null;
        private string _pathToScriptFiles = null;
        private string _pathToOutputFile = null;

        public override bool Execute()
        {
            Log.LogMessage("--CONCATENATING SCRIPT FILES--");
            string baseDir = Path.GetDirectoryName(this.BuildEngine.ProjectFileOfTaskNode);
            string scriptsDir = Path.Combine(baseDir, _pathToScriptFiles);
            Log.LogMessage("Path to script files:" + scriptsDir);

            StringBuilder script = new StringBuilder();
            script.AppendLine("/// --------------------------------------");
            script.AppendLine("/// Script Concatenated by a Tool");
            script.AppendFormat("/// Time Stamp:{0}", DateTime.Now.ToUniversalTime());
            script.AppendLine();
            script.AppendLine("/// --------------------------------------");
            script.AppendLine();

            foreach (string file in _scriptFiles)
            {
                string scriptPath = Path.Combine(scriptsDir, file.Trim());
                script.Append(File.ReadAllText(scriptPath));
            }

            string outputFile = Path.Combine(baseDir, _pathToOutputFile);

            File.WriteAllText(outputFile, script.ToString());
            Log.LogMessage("Output file:" + outputFile);
            Log.LogMessage("--DONE--");
            return true;
        }

        [Required]
        public string ScriptFiles
        {
            get
            {
                return String.Join(" ", _scriptFiles);
            }
            set
            {
                _scriptFiles = value.Split(' ');
            }
        }

        [Required]
        public string PathToScriptFiles
        {
            get
            {
                return _pathToScriptFiles;
            }
            set
            {
                _pathToScriptFiles = value;
            }
        }

        public string PathToOutputFile
        {
            get
            {
                return _pathToOutputFile;
            }
            set
            {
                _pathToOutputFile = value;
            }
        }
    }
}
