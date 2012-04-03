
Microsoft.Web.Testing.RemoteTestcaseExecutor = function()
{
    Microsoft.Web.Testing.RemoteTestcaseExecutor.initializeBase(this);    
    
    ///<summary>Contains the queue of commands for visual display.</summary>
    this._commandHistory = new Array();
    
}

Microsoft.Web.Testing.RemoteTestcaseExecutor.prototype = {    
    invokeGetCommand: function(browserInfo, successDelegate, errorDelegate)
    {   
        Microsoft.Web.Testing.WebService.GetCommand(browserInfo, successDelegate, errorDelegate);        
    },
    
    /// <summary>
    /// Render the list of commands, highlighting the currently executing command (if any)
    /// </summary>
    logTestCommand : function(currentCommand)
    {  
        if(currentCommand == null) 
            return;
        var prettyHistory = "<span class='curCmd'>" + currentCommand.Description + "</span><span class='oldCmd'>";
        for( i = this._commandHistory.length - 1; i >= 0; i-- )
        {
            prettyHistory += " " + this._commandHistory[i];
        }
        prettyHistory += "</span>";
        this._commandHistory.push(currentCommand.Description);
        document.getElementById('Messages').innerHTML = prettyHistory;
    }
}

Microsoft.Web.Testing.RemoteTestcaseExecutor.registerClass('Microsoft.Web.Testing.RemoteTestcaseExecutor', Microsoft.Web.Testing.TestExecutorBase);

// initialize static var
TestExecutor = new Microsoft.Web.Testing.RemoteTestcaseExecutor();