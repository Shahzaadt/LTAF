using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace LTAF.UnitTests
{
    public class MockWebApplication : IWebApplication
    {
        private ICollection<string> _referencedAssemblies;
        private string _assemblyName;
        private bool _throwExceptionOnGetResourceString = false;
        private bool _throwExceptionInGetAssmblies = false;
        private string _getStringReturnValue = "dummyString";
        private Assembly _assembly;

        public bool ThrowExceptionInGetAssmblies
        {
            get { return _throwExceptionInGetAssmblies; }
            set { _throwExceptionInGetAssmblies = value; }
        }

        public bool ThrowExceptionOnGetResourceString
        {
            get { return _throwExceptionOnGetResourceString; }
            set { _throwExceptionOnGetResourceString = value; }
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }

        public Assembly Assembly
        {
            get { return _assembly; }
            set { _assembly = value; }
        }

        public void SetReturnValueForGetString(string returnValue)
        {
            _getStringReturnValue = returnValue;
        }

        public void SetReferencedAssemblies(ICollection<string> referencedAssemblies)
        {
            _referencedAssemblies = referencedAssemblies;
        }

        public ICollection<string> GetReferencedAssemblyFullNames()
        {
            if (_throwExceptionInGetAssmblies)
            {
                throw new InvalidOperationException("Dummy Exception");
            }
            return _referencedAssemblies;
        }

        public string GetResourceString(string assemblyName, string resourceName, string resourceKey)
        {
            if (_throwExceptionOnGetResourceString)
            {
                throw new InvalidOperationException("On purpose exception");
            }

            _assemblyName = assemblyName;
            return _getStringReturnValue;
        }

        public string GetResourceString(Assembly assembly, string resourceName, string resourceKey)
        {
            if (_throwExceptionOnGetResourceString)
            {
                throw new InvalidOperationException("On purpose exception");
            }

            _assembly = assembly;
            return _getStringReturnValue;
        }
    }
}
