using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Globalization;

namespace LTAF.BuildTasks
{
    public class WriteTimeStampFileTask : Task
    {
        public override bool Execute()
        {
            Log.LogMessage(String.Format("--Writting time stamp file to '{0}'--", PathToTimeStampFile));
            string projectDir = Path.GetDirectoryName(this.BuildEngine.ProjectFileOfTaskNode);
            File.WriteAllText(Path.Combine(projectDir, PathToTimeStampFile), DateTime.Now.ToString(CultureInfo.InvariantCulture)); 
            return true;
        }

        [Required]
        public string PathToTimeStampFile
        {
            get;
            set;
        }
    }
}
