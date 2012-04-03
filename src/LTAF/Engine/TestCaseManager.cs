using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Linq;

namespace LTAF.Engine
{
    /// <summary>
    /// The TestCaseManager is used to provide access to the test cases
    /// </summary>
    public class TestCaseManager
    {
        /// <summary>Reference to the App_Code assembly containing the tests</summary>
        private Assembly _appCodeAssembly;

        /// <summary>List of all classes containing the WebTestClassAttribute</summary>
        private List<Type> _testClasses;

        /// <summary>List of all methods containing the WebTestMethodAttribute</summary>
        private List<MethodInfo> _testMethods;

        /// <summary>Lookup used to index test classes by name</summary>
        private Dictionary<string, Type> _testClassesByName;

        /// <summary>Lookup used to index test methods by class</summary>
        private Dictionary<Type, List<MethodInfo>> _testMethodsByClass;

        /// <summary>Lookup used to index test methods by tag</summary>
        private Dictionary<string, List<MethodInfo>> _tags;

        /// <summary>Mapping of tag expressions to the list of tests they represent</summary>
        private Dictionary<string, List<MethodInfo>> _cachedTagExpressions;

        /// <summary>Lookup table used to find test failures in various browsers</summary>
        private Dictionary<MethodInfo, BrowserVersions> _testFailures;

        /// <summary>Object used to synchronize the initial load and prevent starting
        /// multiple browser simultaneously from stepping on each other</summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// Reference to the App_Code assembly containing the tests
        /// </summary>
        public Assembly AppCodeAssembly
        {
            get
            {
                EnsureLoaded();
                return _appCodeAssembly;
            }
        }

        /// <summary>
        /// List of all classes containing the WebTestClassAttribute
        /// </summary>
        public Type[] TestClasses
        {
            get
            {
                EnsureLoaded();
                return _testClasses.ToArray();
            }
        }

        /// <summary>
        /// List of all methods containing the WebTestMethodAttribute
        /// </summary>
        public MethodInfo[] TestMethods
        {
            get
            {
                EnsureLoaded();
                return _testMethods.ToArray();
            }
        }

        /// <summary>
        /// Lookup used to index test methods by tag
        /// </summary>
        public Dictionary<string, List<MethodInfo>> Tags
        {
            get 
            {
                EnsureLoaded();
                return _tags; 
            }
        }

        /// <summary>
        /// Load the test cases
        /// </summary>
        private void EnsureLoaded()
        {
            // Make sure we only load this once and ignore simultaneous requests
            lock (_syncRoot)
            {
                // If it's already been loaded, ignore subsequent loads
                if (_appCodeAssembly != null)
                {
                    return;
                }

                Type[] testTypes = GetTestTypes(out _appCodeAssembly);

                _testClasses = new List<Type>();
                _testClassesByName = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
                _testMethods = new List<MethodInfo>();
                _testMethodsByClass = new Dictionary<Type, List<MethodInfo>>();
                foreach (Type type in testTypes)
                {
                    // Add the type
                    _testClasses.Add(type);
                    _testClassesByName.Add(type.FullName, type);

                    // Add the methods associated with the type
                    List<MethodInfo> methodsForType = new List<MethodInfo>();
                    _testMethodsByClass.Add(type, methodsForType);
                    foreach (MethodInfo method in type.GetMethods())
                    {
                        if (!method.IsDefined(typeof(WebTestMethodAttribute), true))
                        {
                            continue;
                        }

                        _testMethods.Add(method);
                        methodsForType.Add(method);
                    }
                }

                // Sort the list of types alphabetically
                _testClasses.Sort(delegate(Type a, Type b)
                    {
                        return string.CompareOrdinal(
                            a != null ? a.FullName : null,
                            b != null ? b.FullName : null);
                    });

                // Compute the tags and known failures lookups
                _tags = new Dictionary<string, List<MethodInfo>>(StringComparer.OrdinalIgnoreCase);
                _cachedTagExpressions = new Dictionary<string, List<MethodInfo>>(StringComparer.OrdinalIgnoreCase);
                _testFailures = new Dictionary<MethodInfo, BrowserVersions>();

                // Compute the tags for each class
                Dictionary<Type, List<string>> classTags = new Dictionary<Type, List<string>>();
                Dictionary<Type, BrowserVersions> classFailures = new Dictionary<Type, BrowserVersions>();
                foreach (Type type in _testClasses)
                {
                    // Add the tags
                    List<string> tagsForClass = new List<string>();
                    classTags.Add(type, tagsForClass);

                    // Include the class name as one of it's tags so they can
                    // be easily isolated during development without adding addtional
                    // temporary tags
                    tagsForClass.Add(type.FullName);

                    foreach (Attribute attribute in type.GetCustomAttributes(typeof(WebTestTagAttribute), true))
                    {
                        WebTestTagAttribute testTag = attribute as WebTestTagAttribute;
                        if (testTag != null)
                        {
                            tagsForClass.Add(testTag.Tag);
                        }
                    }

                    // Add the known failures
                    BrowserVersions failures = BrowserVersions.None;
                    foreach (Attribute attribute in type.GetCustomAttributes(typeof(WebTestFailureTagAttribute), true))
                    {
                        WebTestFailureTagAttribute failureTag = attribute as WebTestFailureTagAttribute;
                        if (failureTag != null)
                        {
                            failures |= failureTag.Browsers;
                        }
                    }
                    classFailures.Add(type, failures);
                }

                // Compute the tags and known failures for each method
                foreach (MethodInfo method in _testMethods)
                {
                    // Start the list of tags with all those declared on its type
                    List<string> methodTags = new List<string>(classTags[method.ReflectedType]);

                    // Include the method's name and full name as one of it's tags so they can
                    // be easily isolated during development without adding addtional
                    // temporary tags.  Also include Class.Method as well to make it easy to
                    // isolate common tests.
                    methodTags.Add(method.Name);
                    methodTags.Add(string.Format("{0}.{1}", method.ReflectedType.FullName, method.Name));

                    // Add the tags declared on the method
                    foreach (Attribute attribute in method.GetCustomAttributes(typeof(WebTestTagAttribute), true))
                    {
                        WebTestTagAttribute testTag = attribute as WebTestTagAttribute;
                        if (testTag != null)
                        {
                            methodTags.Add(testTag.Tag);
                        }
                    }

                    // Associate this method with each of its tags
                    foreach (string tag in methodTags)
                    {
                        List<MethodInfo> methods;
                        if (!_tags.TryGetValue(tag, out methods))
                        {
                            methods = new List<MethodInfo>();
                            _tags.Add(tag, methods);
                        }
                        methods.Add(method);
                    }

                    // Add the known failures on the method and class
                    BrowserVersions failures = classFailures[method.ReflectedType];
                    foreach (Attribute attribute in method.GetCustomAttributes(typeof(WebTestFailureTagAttribute), true))
                    {
                        WebTestFailureTagAttribute failureTag = attribute as WebTestFailureTagAttribute;
                        if (failureTag != null)
                        {
                            failures |= failureTag.Browsers;
                        }
                    }
                    _testFailures.Add(method, failures);
                }
            }
        }

        /// <summary>
        /// Returns the types that have WebTestClassAttribute inside the currently loaded assemblies.
        /// First checks App_Code and App_SubCode, then checks all available assemblies
        /// </summary>
        /// <param name="appCodeAssembly">Returns the assembly in which the test types were found</param>
        /// <returns>Array of test types found in the first assembly looked at</returns>
        private Type[] GetTestTypes(out Assembly appCodeAssembly)
        {
            // First try to find tests in App_Code or App_SubCode_ assemblies.
            var appCodeAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(
                assembly => assembly.FullName.StartsWith("App_Code") || assembly.FullName.StartsWith("App_SubCode_")
                    );

            foreach (Assembly assembly in appCodeAssemblies)
            {
                var testTypes = assembly.GetTypes().Where(t => t.IsDefined(typeof(WebTestClassAttribute), true)).ToArray();
                if (testTypes.Length > 0)
                {
                    appCodeAssembly = assembly;
                    return testTypes;
                }
            }

            // If we can't find it above, try to find it in all assemblies
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var testTypes = assembly.GetTypes().Where(t => t.IsDefined(typeof(WebTestClassAttribute), true)).ToArray();
                if (testTypes.Length > 0)
                {
                    appCodeAssembly = assembly;
                    return testTypes;
                }
            }

            throw new WebTestException("Could not find any types marked with WebTestClass attribute!");
        }

        /// <summary>
        /// Populate a TreeView's root node with all of the test cases
        /// </summary>
        /// <param name="root">Root node of the TreeView</param>
        public void PopulateTreeView(TreeNode root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root", "Root cannot be null!");
            }
            root.ChildNodes.Clear();
            root.ToolTip = "Toggle selection of all tests";

            EnsureLoaded();
            foreach (Type type in _testClasses)
            {
                TreeNode classNode = new TreeNode(type.Name, type.FullName);
                classNode.ToolTip = string.Format("Toggle selection of all \"{0}\" tests", type.Name);
                classNode.SelectAction = TreeNodeSelectAction.Select;
                
                foreach (MethodInfo method in _testMethodsByClass[type])
                {
                    TreeNode methodNode = new TreeNode(method.Name, method.Name);
                    methodNode.SelectAction = TreeNodeSelectAction.Select;
                    classNode.ChildNodes.Add(methodNode);
                }

                if (classNode.ChildNodes.Count > 0)
                {
                    root.ChildNodes.Add(classNode);
                }
            }
        }

        /// <summary>
        /// Get the selected test cases
        /// </summary>
        /// <param name="tree">Tree with the test cases</param>
        /// <returns>List of selected tests</returns>
        public IList<Testcase> GetSelectedTestCases(TreeView tree)
        {
            if (tree == null)
            {
                throw new ArgumentNullException("tree", "TreeView cannot be null!");
            }

            List<Testcase> tests = new List<Testcase>();
            foreach (TreeNode node in tree.CheckedNodes)
            {
                if (node.Checked)
                {
                    MethodInfo testCaseMethod = this.GetTestCase(node.Parent.Value, node.Value, false);
                    if (testCaseMethod != null)
                    {
                        tests.Add(new Testcase(testCaseMethod.ReflectedType, node.Value));
                    }
                }
            }

            return tests;
        }

        /// <summary>
        /// Get a test method by name
        /// </summary>
        /// <param name="testClass">Name of the method's type</param>
        /// <param name="testMethod">Name of the method</param>
        /// <returns>Method</returns>
        private MethodInfo GetTestCase(string testClass, string testMethod)
        {
            return GetTestCase(testClass, testMethod, true);
        }

        /// <summary>
        /// Get a test method by name
        /// </summary>
        /// <param name="testClass">Name of the method's type</param>
        /// <param name="testMethod">Name of the method</param>
        /// <param name="throwIfNotFound">Throw an exception if the test isn't found</param>
        /// <returns>Method</returns>
        private MethodInfo GetTestCase(string testClass, string testMethod, bool throwIfNotFound)
        {
            Type @class;
            if (!_testClassesByName.TryGetValue(testClass, out @class))
            {
                if (throwIfNotFound)
                {
                    throw new ArgumentException(string.Format("Test class \"{0}\" undefined!", testClass), "testClass");
                }
                return null;
            }

            foreach (MethodInfo method in _testMethodsByClass[@class])
            {
                if (string.Compare(method.Name, testMethod, StringComparison.Ordinal) == 0)
                {
                    return method;
                }
            }
            if (throwIfNotFound)
            {
                throw new ArgumentException(string.Format("Test method \"{0}\" undefined!", testMethod), "testMethod");
            }
            return null;
        }

        /// <summary>
        /// Get the root node of a TreeView
        /// </summary>
        /// <param name="tree">Tree</param>
        /// <returns>Root node of the TreeView</returns>
        private TreeNode GetTreeRoot(TreeView tree)
        {
            if (tree == null)
            {
                throw new ArgumentNullException("tree", "Tree cannot be null!");
            }
            else if (tree.Nodes.Count <= 0)
            {
                throw new ArgumentException("Tree does not have a root!", "tree");
            }
            return tree.Nodes[0];
        }

        /// <summary>
        /// Select all the tests in the TreeView
        /// </summary>
        /// <param name="tree">TreeView</param>
        public void SelectAllTests(TreeView tree)
        {
            SelectAllTests(GetTreeRoot(tree), BrowserVersions.None);
        }

        /// <summary>
        /// Select all the tests in the TreeView
        /// </summary>
        /// <param name="tree">TreeView</param>
        /// <param name="browser">Browser running the tests</param>
        public void SelectAllTests(TreeView tree, BrowserVersions browser)
        {
            SelectAllTests(GetTreeRoot(tree), browser);
        }

        /// <summary>
        /// Select all the tests in the TreeView
        /// </summary>
        /// <param name="root">Root node of the TreeView</param>
        public void SelectAllTests(TreeNode root)
        {
            SelectAllTests(root, BrowserVersions.None);
        }

        /// <summary>
        /// Select all the tests in the TreeView
        /// </summary>
        /// <param name="root">Root node of the TreeView</param>
        /// <param name="browser">Browser running the tests</param>
        public void SelectAllTests(TreeNode root, BrowserVersions browser)
        {
            SelectTaggedTests(root, null, browser);
        }

        /// <summary>
        /// Select all tests in the TreeView that are tagged by the tagExpression
        /// </summary>
        /// <param name="tree">TreeView</param>
        /// <param name="tagExpression">Tag expression</param>
        /// <param name="browser">Browser running the tests</param>
        public void SelectTaggedTests(TreeView tree, string tagExpression, BrowserVersions browser)
        {
            SelectTaggedTests(GetTreeRoot(tree), tagExpression, browser);
        }

        /// <summary>
        /// Select all tests in the TreeView that are tagged by the tagExpression
        /// </summary>
        /// <param name="root">Root node of the TreeView</param>
        /// <param name="tagExpression">Tag expression</param>
        /// <param name="browser">Browser running the tests</param>
        public void SelectTaggedTests(TreeNode root, string tagExpression, BrowserVersions browser)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root", "Root cannot be null!");
            }

            EnsureLoaded();

            // Get the tests that should be tagged
            bool selectAllTests = string.IsNullOrEmpty(tagExpression);
            IList<MethodInfo> taggedTests = null;
            if (!selectAllTests)
            {
                taggedTests = GetTestsByTag(tagExpression, browser);
            }

            // Select the nodes
            foreach (TreeNode classNode in root.ChildNodes)
            {
                foreach (TreeNode methodNode in classNode.ChildNodes)
                {
                    MethodInfo testCase = GetTestCase(classNode.Value, methodNode.Value);
                    bool selected = selectAllTests || taggedTests.Contains(testCase);
                    methodNode.Checked = selected;
                }
            }
        }

        /// <summary>
        /// Ignore any tests that fail in the given browsers
        /// </summary>
        /// <param name="tree">TreeView</param>
        /// <param name="browsers">Browsers to skip failures</param>
        public void IgnoreFailures(TreeView tree, BrowserVersions browsers)
        {
            IgnoreFailures(GetTreeRoot(tree), browsers);
        }

        /// <summary>
        /// Ignore any tests that fail in the given browsers
        /// </summary>
        /// <param name="root">Root node of the TreeView</param>
        /// <param name="browsers">Browsers to skip failures</param>
        public void IgnoreFailures(TreeNode root, BrowserVersions browsers)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root", "Root cannot be null!");
            }

            EnsureLoaded();

            // Select the nodes
            foreach (TreeNode @class in root.ChildNodes)
            {
                foreach (TreeNode method in @class.ChildNodes)
                {
                    MethodInfo testCase = GetTestCase(@class.Value, method.Value);
                    BrowserVersions failures;
                    if (this._testFailures.TryGetValue(testCase, out failures) && ((browsers & failures) > 0))
                    {
                        method.Checked = false;
                    }
                }
            }
        }

        /// <summary>
        /// Remove any unchecked tests
        /// </summary>
        /// <param name="tree">TreeView</param>
        public void FilterIgnoredTests(TreeView tree)
        {
            FilterIgnoredTests(GetTreeRoot(tree));
        }

        /// <summary>
        /// Remove any unchecked tests
        /// </summary>
        /// <param name="root">Root node of the TreeView</param>
        public void FilterIgnoredTests(TreeNode root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root", "Root cannot be null!");
            }

            // Find the unchecked nodes
            List<KeyValuePair<TreeNode, TreeNode>> nodesToFilter = new List<KeyValuePair<TreeNode, TreeNode>>();
            foreach (TreeNode @class in root.ChildNodes)
            {
                bool allMethodsUnchecked = true;
                foreach (TreeNode method in @class.ChildNodes)
                {
                    if (!method.Checked)
                    {
                        nodesToFilter.Add(new KeyValuePair<TreeNode, TreeNode>(method, @class));
                    }
                    else
                    {
                        allMethodsUnchecked = false;
                    }
                }
                if (allMethodsUnchecked)
                {
                    nodesToFilter.Add(new KeyValuePair<TreeNode, TreeNode>(@class, root));
                }
            }

            // Remove the unchecked nodes
            foreach (KeyValuePair<TreeNode, TreeNode> pair in nodesToFilter)
            {
                pair.Value.ChildNodes.Remove(pair.Key);
            }
        }

        /// <summary>
        /// Get the test methods that correspond to a tag expression
        /// </summary>
        /// <param name="tagExpression">Tag expression</param>
        /// <returns>Test methods for the tag expression</returns>
        public IList<MethodInfo> GetTestsByTag(string tagExpression)
        {
            return GetTestsByTag(tagExpression, BrowserVersions.None);
        }

        /// <summary>
        /// Get the test methods that correspond to a tag expression
        /// </summary>
        /// <param name="tagExpression">Tag expression</param>
        /// <param name="browser">Browser running the tests</param>
        /// <returns>Test methods for the tag expression</returns>
        public IList<MethodInfo> GetTestsByTag(string tagExpression, BrowserVersions browser)
        {
            if (tagExpression == null)
            {
                throw new ArgumentNullException("tagExpression", "Tag expression cannot be null!");
            }
            else if (tagExpression.Length == 0)
            {
                throw new ArgumentException("Tag expression cannot be empty!", "tagExpression");
            }

            EnsureLoaded();

            // Try to use the cached methods
            List<MethodInfo> methods;
            string cacheKey = string.Format("{0} + {1}", tagExpression, browser);
            if (_cachedTagExpressions.TryGetValue(cacheKey, out methods))
            {
                return methods;
            }

            // Evaluate the tag
            WebTestTagExpressionEvaluator webTestTagExpressionEvaluator = new WebTestTagExpressionEvaluator();
            methods = webTestTagExpressionEvaluator.Evaluate(
                tagExpression, 
                browser, 
                _testMethods,
                _testFailures, 
                _tags);

            // Cache the expression and return it
            _cachedTagExpressions.Add(cacheKey, methods);
            return methods;
        }

    }
}