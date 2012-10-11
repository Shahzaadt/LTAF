using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Globalization;
using NuGet;


namespace LTAF.Infrastructure
{
    internal class NuGetWebProjectSystem : PhysicalFileSystem, IProjectSystem
    {
        #region static helper methods

        internal static string ResolvePartialAssemblyName(string name)
        {
            foreach (var key in _knownPublicKeys)
            {
                var assemblyFullName = String.Format(CultureInfo.InvariantCulture, "{0}, Version={1}, Culture=neutral, PublicKeyToken={2}",
                    name, VersionUtility.DefaultTargetFrameworkVersion, key);

                try
                {
                    Assembly.Load(assemblyFullName);
                    // Assembly.Load throws a FileNotFoundException if the assembly name cannot be resolved. If we managed to successfully locate the assembly, return it.
                    return assemblyFullName;
                }
                catch
                {
                    // Do nothing. We don't want to throw from this method.
                }
            }
            return null;
        }

        internal static void AddReferencesToConfig(NuGet.IFileSystem fileSystem, string references)
        {
            var webConfigPath = Path.Combine(fileSystem.Root, "web.config");
            XDocument document;
            // Read the web.config file from the AppRoot if it exists.
            if (fileSystem.FileExists(webConfigPath))
            {
                using (Stream stream = fileSystem.OpenFile(webConfigPath))
                {
                    document = XDocument.Load(stream, LoadOptions.PreserveWhitespace);
                }
            }
            else
            {
                document = new XDocument(new XElement("configuration"));
            }

            var assemblies = GetOrCreateChild(document.Root, "system.web/compilation/assemblies");

            // Get the name of the existing references 
            // References are stored in the format <add assembly="System.Web.Abstractions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
            bool existingAssembly = (from item in assemblies.Elements()
                                     where !String.IsNullOrEmpty(item.GetOptionalAttributeValue("assembly"))
                                     let assemblyName = new AssemblyName(item.Attribute("assembly").Value).Name
                                     where String.Equals(assemblyName, references, StringComparison.OrdinalIgnoreCase)
                                     select item).Any();

            if (!existingAssembly)
            {
                assemblies.Add(new XElement("add", new XAttribute("assembly", references)));
                SaveDocument(fileSystem, webConfigPath, document);
            }
        }

        private static void SaveDocument(NuGet.IFileSystem fileSystem, string webConfigPath, XDocument document)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                fileSystem.AddFile(webConfigPath, stream);
            }
        }

        private static XElement GetOrCreateChild(XElement element, string childName)
        {
            foreach (var item in childName.Split('/'))
            {
                XElement child = element.Element(item);
                if (child == null)
                {
                    child = new XElement(item);
                    element.Add(child);
                }
                element = child;
            }
            return element;
        }

        private static bool RequiresAppCodeRemapping(string path)
        {
            return !IsUnderStandardCodeFolder(path) && IsSourceFile(path) && !IsCodeBehindFile(path);
        }

        private bool IsUnderAppCode(string path)
        {
            return path.StartsWith(AppCodeFolder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsUnderStandardCodeFolder(string path)
        {
            return _standardAspNetCodeFolders.Where(
                a => path.StartsWith(a + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)).Count() > 0;
        }

        private static bool IsSourceFile(string path)
        {
            return _sourceFileExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsCodeBehindFile(string path)
        {
            string extension = Path.GetExtension(path);

            return _sourceFileExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase)
                && _codeBehindFileExtensions.Where(e => path.EndsWith(e + extension, StringComparison.OrdinalIgnoreCase)).Count() > 0;
        }

        #endregion

        private const string BinDir = "bin";
        private const string AppCodeFolder = "App_Code";
        private static readonly string[] _standardAspNetCodeFolders = new[] { "App_Code", "App_Start" };

        private static readonly string[] _generatedFilesFolder = new[] { "Generated___Files" };
        private static readonly string[] _codeBehindFileExtensions = new[] { ".aspx", ".ascx", ".designer", ".master" };
        private static readonly string[] _sourceFileExtensions = new[] { ".cs", ".vb" };
        // Keys taken from the 4.0 RedistList. 
        private static readonly string[] _knownPublicKeys = new[] { "b03f5f7f11d50a3a", "b77a5c561934e089", "31bf3856ad364e35" };

        public NuGetWebProjectSystem(string root)
            : base(root)
        {
        }

        protected virtual string GetReferencePath(string name)
        {
            return Path.Combine(BinDir, name);
        }

        #region IProjectSystem

        public void AddReference(string referencePath, Stream stream)
        {
            // Copy to bin by default
            string referenceName = Path.GetFileName(referencePath);
            string dest = GetFullPath(GetReferencePath(referenceName));

            // Copy the reference over
            AddFile(dest, stream);
        }

        public bool IsSupportedFile(string path)
        {
            return !path.StartsWith("tools", StringComparison.OrdinalIgnoreCase) && !Path.GetFileName(path).Equals("app.config", StringComparison.OrdinalIgnoreCase);
        }

        public string ProjectName
        {
            get { return Root; }
        }

        public bool IsBindingRedirectSupported
        {
            get
            {
                return false;
            }
        }

        public bool ReferenceExists(string name)
        {
            string path = GetReferencePath(name);
            return FileExists(path);
        }

        public void RemoveReference(string name)
        {
            DeleteFile(GetReferencePath(name));

            // Delete the bin directory if this was the last reference
            if (!GetFiles(BinDir, true).Any())
            {
                DeleteDirectory(BinDir);
            }
        }

        private FrameworkName _targetFramework = VersionUtility.DefaultTargetFramework;
        public FrameworkName TargetFramework
        {
            get { return this._targetFramework; }
            set { this._targetFramework = value; }
        }

        public void AddFrameworkReference(string name)
        {
            // Before we add a framework assembly to web.config, verify that it exists in the GAC. This is important because a website would be completely unusable if the assembly reference
            // does not exist and is added to web.config. Since the assembly name may be a partial name, We use the ResolveAssemblyReference task in Msbuild to identify a full name and if it is 
            // installed in the GAC. 
            var fullName = ResolvePartialAssemblyName(name);
            if (fullName == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "Unknwon framework reference.", name));
            }
            AddReferencesToConfig(this, fullName);
        }

        public string ResolvePath(string path)
        {
            if (RequiresAppCodeRemapping(path))
            {
                path = Path.Combine(AppCodeFolder, path);
            }
            return path;
        }

        #endregion

        #region IPropertyProvider

        public dynamic GetPropertyValue(string propertyName)
        {
            if (propertyName == null)
            {
                return null;
            }

            // Return empty string for the root namespace of this project.
            if (propertyName.Equals("RootNamespace", StringComparison.OrdinalIgnoreCase))
            {
                return String.Empty;
            }

            return null;
        }

        #endregion

        #region PhysicalFileSystem

        public override IEnumerable<string> GetDirectories(string path)
        {
            if (IsUnderAppCode(path))
            {
                // There is an invisible folder called Generated___Files under app code that we want to exclude from our search
                return base.GetDirectories(path).Except(_generatedFilesFolder, StringComparer.OrdinalIgnoreCase);
            }
            return base.GetDirectories(path);
        }

        #endregion
    }
}
