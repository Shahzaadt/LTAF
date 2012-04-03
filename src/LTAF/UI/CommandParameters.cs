
namespace LTAF
{
    /// <summary>
    /// Object that is sent to Click() (and maybe other) commands that encapsulate generic command options
    /// </summary>
    public class CommandParameters
    {
        private string _popupText;

        /// <summary>
        /// CommandParamaters
        /// </summary>
        public CommandParameters()
        {
        }

        /// <summary>
        /// CommandParameters
        /// </summary>
        public CommandParameters(WaitFor waitFor, PopupAction popupAction)
        {
            this.WaitFor = waitFor;
            this.PopupAction = popupAction;
        }

        /// <summary>WaitFor</summary>
        public WaitFor WaitFor { get; set; }

        /// <summary>PopupAction</summary>
        public PopupAction PopupAction { get; set; }

        /// <summary>PopupText</summary>
        public string PopupText
        {
            get
            {
                return _popupText;
            }
            internal set
            {
                _popupText = value;
            }
        }
    }
}
