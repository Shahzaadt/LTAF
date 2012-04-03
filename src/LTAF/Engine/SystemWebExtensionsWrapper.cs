using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.UI;

namespace LTAF.Engine
{
    internal class SystemWebExtensionsWrapper
    {
        private bool _methodsInitialized;
        private readonly object _reflectionLock;
        private IAspNetPageService _aspNetPageService;
        private MethodInfo _regiterStartupScriptMethod;

        public SystemWebExtensionsWrapper(IAspNetPageService aspNetPageService)
        {
            _reflectionLock = new object();
            _aspNetPageService = aspNetPageService;
        }

        /// <summary>
        /// Reference to the System.Web.Extensions assembly
        /// </summary>
        /// <remarks>
        /// Functionality in System.Web.Extensions is being accessed via
        /// reflection because we don't want the testing framework to be tied
        /// to a specific version.
        /// </remarks>
        public Assembly SystemWebExtensionsAssembly { get; set; }

        internal MethodInfo RegisterStartupScriptMethodInfo
        {
            get
            {
                return _regiterStartupScriptMethod;
            }
            set
            {
                _regiterStartupScriptMethod = value;
            }
        }

        public void Initialize(Page page)
        {
            if (!_methodsInitialized)
            {
                lock (_reflectionLock)
                {
                    if (!_methodsInitialized)
                    {
                        Type scriptManagerType = _aspNetPageService.FindControl(page, "DriverPageScriptManager").GetType();
                        SystemWebExtensionsAssembly = scriptManagerType.Assembly;
                        _regiterStartupScriptMethod = scriptManagerType.GetMethod(
                            "RegisterStartupScript", 
                            new Type[] { 
                                typeof(Page), 
                                typeof(Type), 
                                typeof(string), 
                                typeof(string), 
                                typeof(bool) });
                        _methodsInitialized = true;
                    }
                }
            }
        }

        public virtual void RegisterStartupScript(Control control, Type type, string key, string script, bool addScriptTags)
        {
            RegisterStartupScriptMethodInfo.Invoke(null, new object[] { control, type, key, script, addScriptTags });
        }

        /// <summary>
        /// Deserialize a JSON string into a CLR object
        /// </summary>
        /// <param name="json">JSON</param>
        /// <returns>Deserialized JSON</returns>
        public object DeserializeJson(string json)
        {
            if (SystemWebExtensionsAssembly == null)
            {
                throw new InvalidOperationException("SystemWebExtesnionsReference must be provided!");
            }

            Type serializerType = SystemWebExtensionsAssembly.GetType("System.Web.Script.Serialization.JavaScriptSerializer");
            if (serializerType == null)
            {
                throw new InvalidOperationException("Invalid SystemWebExtesnionsReference does not contain System.Web.Script.Serialization.JavaScriptSerializer!");
            }

            MethodInfo deserialize = serializerType.GetMethod("DeserializeObject");
            if (deserialize == null)
            {
                throw new InvalidOperationException("System.Web.Script.Serialization.JavaScriptSerializer does not contain DeserializeObject method!");
            }

            object serializer = Activator.CreateInstance(serializerType);
            if (serializer == null)
            {
                throw new InvalidOperationException("Failed to create System.Web.Script.Serialization.JavaScriptSerializer!");
            }

            return deserialize.Invoke(serializer, new object[] { json });
        }
    }
}
