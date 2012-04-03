
LTAF.TestExecutor = function()
{
    LTAF.TestExecutor.initializeBase(this);
    
    this._threadId = null; 
}

LTAF.TestExecutor.prototype = {    
    /// <summary>
    /// Method that sets the server thread id that is processing this client
    /// </summary>
    set_threadId : function(threadId)
    {
        this._threadId = threadId;
    },
    
    hideSpinner : function()
    {
        LTAF.TestExecutor.callBaseMethod(this, "hideSpinner");        
        
        var threadId = document.getElementById('ThreadId');
        if(threadId) threadId.innerHTML = '';
    },
    
    invokeGetCommand: function(browserInfo, successDelegate, errorDelegate)
    {  
        PageMethods.GetCommand(this._threadId, browserInfo, successDelegate, errorDelegate);
    }   
}

LTAF.TestExecutor.registerClass('LTAF.TestExecutor', LTAF.TestExecutorBase); 


// initialize static var
TestExecutor = new LTAF.TestExecutor();
