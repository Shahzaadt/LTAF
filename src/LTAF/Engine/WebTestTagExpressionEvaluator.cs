using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LTAF.Engine
{
    /// <summary>
    /// Evaluate tag expressions
    /// </summary>
    /// <remarks>
    /// Tag expressions are derived from the following EBNF grammar:
    ///     {Expression} :=
    ///         {Expression} @ {Term} |
    ///         {Expression} - {Term} |
    ///         {Term}
    ///     {Term} :=
    ///         {Term} ^ {Factor} |
    ///         {Factor}
    ///     {Factor} :=
    ///         !{Factor} |
    ///         ({Expression}) |
    ///         {Tag}
    ///     {Tag} :=
    ///         All |
    ///         Fail |
    ///         [^InvalidCharacters]+
    ///  
    /// The non-terminals for {Expression} and {Term} will be left factored
    /// in the recursive descent parser below.
    /// </remarks>
    internal class WebTestTagExpressionEvaluator
    {
        /// <summary>Invalid characters in a tag name</summary>
        private readonly char[] InvalidCharacters = new char[] { '@', '-', '^', '!', '(', ')', '/' };

        /// <summary>Expression being evaluated</summary>
        private string _tagExpression;

        /// <summary>Current position in the expression</summary>
        private int _position;

        /// <summary>Browser being tested</summary>
        private BrowserVersions _browser;

        /// <summary>Lookup table used to find test failures in various browsers</summary>
        private Dictionary<MethodInfo, BrowserVersions> _testFailures;

        /// <summary>Lookup used to index test methods by tag</summary>
        private Dictionary<string, List<MethodInfo>> _tags;

        /// <summary>List of all methods containing the WebTestMethodAttribute</summary>
        private List<MethodInfo> _testMethods;        


        /// <summary>
        /// Create an expression evaluator
        /// </summary>
        internal WebTestTagExpressionEvaluator()
        {
            
        }

        /// <summary>
        /// Evaluate an expression
        /// </summary>
        /// <returns>Test methods described by the expression</returns>
        public List<MethodInfo> Evaluate(
            string tagExpression, 
            BrowserVersions browser,
            List<MethodInfo> testMethods,
            Dictionary<MethodInfo, BrowserVersions> testFailures,
            Dictionary<string, List<MethodInfo>> tags)
        {
            if (tagExpression == null)
            {
                throw new ArgumentNullException("tagExpression", "tagExpression cannot be null!");
            }
            else if (tagExpression.Length == 0)
            {
                throw new ArgumentException("tagExpression cannot be empty!", "tagExpression");
            }

            _tagExpression = tagExpression;
            _browser = browser;
            _testFailures = testFailures;
            _testMethods = testMethods;
            _tags = tags;
            _position = 0;

            List<MethodInfo> expression = ReadExpression();
            if (_position >= 0 && _position < _tagExpression.Length)
            {
                throw new FormatException(string.Format("Expected end of tag expression \"{0}\" at position {1}!", _tagExpression, _position));
            }
            return expression;
        }

        /// <summary>
        /// Match a sequence of characters
        /// </summary>
        /// <param name="expected">String to match</param>
        private void Match(string expected)
        {
            if (!TryMatch(expected))
            {
                throw new FormatException(string.Format("Invalid tag expression \"{0}\" (expected \"{1}\" at position {2})!",
                    _tagExpression, expected, _position));
            }
        }

        /// <summary>
        /// Try to match a sequence of characters
        /// </summary>
        /// <param name="expected">String to match</param>
        /// <returns></returns>
        private bool TryMatch(string expected)
        {
            if (_position + expected.Length > _tagExpression.Length)
            {
                return false;
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (_tagExpression[i + _position] != expected[i])
                {
                    return false;
                }
            }

            _position += expected.Length;
            return true;
        }

        /// <summary>
        /// Evaluate an expression
        /// </summary>
        /// <returns>Test methods described by the expression</returns>
        /// <remarks>
        /// We need to factor out left recursion, so:
        ///     {Expression} :=
        ///         {Expression} @ {Term} |
        ///         {Expression} - {Term} |
        ///         {Term}
        /// becomes:
        ///     {Expression} :=
        ///     	{Term}{Expression'}
        ///     
        ///     {Expression'} :=
        ///     	#empty#
        ///     	@ {Term}{Expression'}
        ///     	- {Term}{Expression'}
        /// </remarks>
        private List<MethodInfo> ReadExpression()
        {
            return ReadExpression(ReadTerm());
        }

        /// <summary>
        /// Evaluate an expression
        /// </summary>
        /// <param name="term">
        /// Left term already read as part of the expression
        /// </param>
        /// <returns>Test methods described by the expression</returns>
        /// <remarks>
        /// Non-terminal created for left-factoring:
        ///     {Expression'} :=
        ///     	#empty#
        ///     	@ {Term}{Expression'}
        ///     	- {Term}{Expression'}
        /// </remarks>
        private List<MethodInfo> ReadExpression(List<MethodInfo> term)
        {
            if (TryMatch("@"))
            {
                return ReadExpression(Union(term, ReadTerm()));
            }
            else if (TryMatch("-"))
            {
                return ReadExpression(Difference(term, ReadTerm()));
            }
            else
            {
                return term;
            }
        }

        /// <summary>
        /// Evaluate a term
        /// </summary>
        /// <returns>Test methods described by the expression</returns>
        /// <remarks>
        /// We need to factor out left recursion, so:
        ///     {Term} :=
        ///         {Factor} ^ {Term} |
        ///         {Factor}
        /// becomes:
        ///     {Term} :=
        ///         {Factor}{Term'}
        /// 
        ///     {Term'} :=
        ///     	#empty#
        ///     	^ {Factor}{Term'}
        /// </remarks>
        private List<MethodInfo> ReadTerm()
        {
            return ReadTerm(ReadFactor());
        }

        /// <summary>
        /// Evaluate a term
        /// </summary>
        /// <param name="factor">
        /// Left term already read as part of the expression
        /// </param>
        /// <returns>Test methods described by the expression</returns>
        /// <remarks>
        /// Non-terminal created for left-factoring:
        ///     {Term'} :=
        ///     	#empty#
        ///     	^ {Factor}{Term'}
        /// </remarks>
        private List<MethodInfo> ReadTerm(List<MethodInfo> factor)
        {
            if (TryMatch("^"))
            {
                return ReadTerm(Intersection(factor, ReadFactor()));
            }
            else
            {
                return factor;
            }
        }

        /// <summary>
        /// Evaluate a factor
        /// </summary>
        /// <returns>Test methods described by the expression</returns>
        /// <remarks>
        /// {Factor} :=
        ///     !{Factor} |
        ///     ({Expression}) |
        ///     {Tag}
        /// </remarks>
        private List<MethodInfo> ReadFactor()
        {
            if (TryMatch("!"))
            {
                return Complement(ReadFactor());
            }
            else if (TryMatch("("))
            {
                List<MethodInfo> expression = ReadExpression();
                Match(")");
                return expression;
            }
            else
            {
                return ReadTag();
            }
        }

        /// <summary>
        /// Evaluate a tag
        /// </summary>
        /// <returns>Test methods described by the expression</returns>
        /// <remarks>
        /// {Tag} :=
        ///     All |
        ///     Fail |
        ///     [^InvalidCharacters]+
        /// </remarks>
        private List<MethodInfo> ReadTag()
        {
            int end = _tagExpression.IndexOfAny(InvalidCharacters, _position);
            if (end < 0)
            {
                end = _tagExpression.Length;
            }
            string tag = _tagExpression.Substring(_position, end - _position);
            if (string.IsNullOrEmpty(tag))
            {
                throw new FormatException(string.Format("Tag expected in expression \"{0}\" at position {1}!", _tagExpression, _position));
            }
            _position = end;
            if (string.Compare(tag, "all", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new List<MethodInfo>(this._testMethods);
            }
            else if (string.Compare(tag, "fail", StringComparison.OrdinalIgnoreCase) == 0)
            {
                List<MethodInfo> failures = new List<MethodInfo>();
                foreach (KeyValuePair<MethodInfo, BrowserVersions> pair in this._testFailures)
                {
                    if ((pair.Value & _browser) > 0)
                    {
                        failures.Add(pair.Key);
                    }
                }
                return failures;
            }
            else
            {
                List<MethodInfo> methods;
                if (!this._tags.TryGetValue(tag, out methods))
                {
                    throw new FormatException(string.Format("No tests corresponding to tag \"{0}\" in expression \"{1}\" were found!", tag, _tagExpression));
                }
                return methods;
            }
        }

        /// <summary>
        /// Union of two sets of test methods
        /// </summary>
        /// <param name="first">First set of test methods</param>
        /// <param name="second">Second set of test methods</param>
        /// <returns>Union of the two sets</returns>
        private List<MethodInfo> Union(List<MethodInfo> first, List<MethodInfo> second)
        {
            List<MethodInfo> union = new List<MethodInfo>(first);
            foreach (MethodInfo method in second)
            {
                if (!first.Contains(method))
                {
                    union.Add(method);
                }
            }
            return union;
        }

        /// <summary>
        /// Difference between two sets of test methods
        /// </summary>
        /// <param name="first">First set of test methods</param>
        /// <param name="second">Second set of test methods</param>
        /// <returns>Difference between the two sets</returns>
        private List<MethodInfo> Difference(List<MethodInfo> first, List<MethodInfo> second)
        {
            List<MethodInfo> difference = new List<MethodInfo>(first);
            foreach (MethodInfo method in second)
            {
                difference.Remove(method);
            }
            return difference;
        }

        /// <summary>
        /// Intersection of two sets of test methods
        /// </summary>
        /// <param name="first">First set of test methods</param>
        /// <param name="second">Second set of test methods</param>
        /// <returns>Intersection of the two sets</returns>
        private List<MethodInfo> Intersection(List<MethodInfo> first, List<MethodInfo> second)
        {
            List<MethodInfo> intersection = new List<MethodInfo>();
            foreach (MethodInfo method in first)
            {
                if (second.Contains(method))
                {
                    intersection.Add(method);
                }
            }
            return intersection;
        }

        /// <summary>
        /// Complement of a set of test methods
        /// </summary>
        /// <param name="methods">Test methods</param>
        /// <returns>Complement of a set of test methods</returns>
        private List<MethodInfo> Complement(List<MethodInfo> methods)
        {
            List<MethodInfo> complement = new List<MethodInfo>(this._testMethods);
            foreach (MethodInfo method in methods)
            {
                complement.Remove(method);
            }
            return complement;
        }
    }
}
