using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace LTAF
{
    /// <summary>
    /// Verifies conditions in unit tests using true/false propositions.
    /// </summary>
    public class Assert
    {
        /// <summary>OnAssertFailed</summary>
        protected static void OnAssertFailed(string message)
        {
            if (ServiceLocator.AssertResultHandler != null)
            {
                ServiceLocator.AssertResultHandler.AssertFailed(message);
            }
            else
            {
                throw new WebTestException(message);
            }
        }

        /// <summary>OnAssertPassed</summary>
        protected static void OnAssertPassed(string message)
        {
            if (ServiceLocator.AssertResultHandler != null)
            {
                ServiceLocator.AssertResultHandler.AssertPassed(message);
            }
        }

        #region Fail
        /// <summary>
        /// Throws an AssertFailedException.
        /// </summary>
        public static void Fail()
        {
            Fail(string.Empty);
        }

        /// <summary>
        /// Throws an AssertFailedException.
        /// </summary>
        /// <param name="message">
        /// A message to include with the exception. This message
        /// can be seen in the test results.
        /// </param>
        public static void Fail(string message)
        {
            Fail(message, null);
        }

        /// <summary>
        /// Throws an AssertFailedException.
        /// </summary>
        /// <param name="message">
        /// A message to include with the exception. This message
        /// can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void Fail(string message, params object[] parameters)
        {
            OnAssertFailed(FormatMessage(message, parameters, "Assertion failed!"));
        }
        #endregion

        #region Inconclusive
        /// <summary>
        /// Throws an AssertInconclusiveException.
        /// </summary>
        public static void Inconclusive()
        {
            Inconclusive(string.Empty);
        }

        /// <summary>
        /// Throws an AssertInconclusiveException.
        /// </summary>
        /// <param name="message">
        /// A message to include with the exception. This message can
        /// be seen in the test results.
        /// </param>
        public static void Inconclusive(string message)
        {
            Inconclusive(string.Empty, null);
        }

        /// <summary>
        /// Throws an AssertInconclusiveException.
        /// </summary>
        /// <param name="message">
        /// A message to include with the exception. This message can
        /// be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void Inconclusive(string message, params object[] parameters)
        {
            OnAssertFailed(FormatMessage(message, parameters, "Assertion inconclusive!"));
        }
        #endregion

        #region IsTrue
        /// <summary>
        /// Tests whether the specified condition is true and fails the
        /// test if the condition is false.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is true.
        /// </param>
        public static void IsTrue(bool condition)
        {
            IsTrue(condition, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified condition is true and fails the
        /// test if the condition is false.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is true.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void IsTrue(bool condition, string message)
        {
            IsTrue(condition, message, null);
        }

        /// <summary>
        /// Tests whether the specified condition is true and fails the
        /// test if the condition is false.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is true.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void IsTrue(bool condition, string message, params object[] parameters)
        {
            if (!condition)
            {
                OnAssertFailed(FormatMessage(message, parameters, "Assert.IsTrue: Expected <True>, Actual <False>!"));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null, "Assert.IsTrue"));
            }
        }

        /// <summary>
        /// Tests whether the specified condition is true and fails the
        /// test if the condition is false.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is true.
        /// </param>
        public static void IsTrue(bool? condition)
        {
            IsTrue(condition, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified condition is true and fails the
        /// test if the condition is false.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is true.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void IsTrue(bool? condition, string message)
        {
            IsTrue(condition, message, null);
        }

        /// <summary>
        /// Tests whether the specified condition is true and fails the
        /// test if the condition is false.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is true.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void IsTrue(bool? condition, string message, params object[] parameters)
        {
            if (!condition.HasValue || !condition.Value)
            {
                OnAssertFailed(FormatMessage(message, parameters, "Assert.IsTrue: Expected <True>, Actual <{0}>!", condition.HasValue ? "False" : "Null"));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null, "Assert.IsTrue"));
            }
        }
        #endregion

        #region IsFalse
        /// <summary>
        /// Tests whether the specified condition is false and fails the
        /// test if the condition is true.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is false.
        /// </param>
        public static void IsFalse(bool condition)
        {
            IsFalse(condition, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified condition is false and fails the
        /// test if the condition is true.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is false.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void IsFalse(bool condition, string message)
        {
            IsFalse(condition, message, null);
        }

        /// <summary>
        /// Tests whether the specified condition is false and fails the
        /// test if the condition is true.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is false.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void IsFalse(bool condition, string message, params object[] parameters)
        {
            if (condition)
            {
                OnAssertFailed(FormatMessage(message, parameters, "Assert.IsFalse: Expected <False>, Actual <True>!"));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null, "Assert.IsFalse"));
            }
        }

        /// <summary>
        /// Tests whether the specified condition is false and fails the
        /// test if the condition is true.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is false.
        /// </param>
        public static void IsFalse(bool? condition)
        {
            IsFalse(condition, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified condition is false and fails the
        /// test if the condition is true.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is false.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void IsFalse(bool? condition, string message)
        {
            IsFalse(condition, message, null);
        }

        /// <summary>
        /// Tests whether the specified condition is false and fails the
        /// test if the condition is true.
        /// </summary>
        /// <param name="condition">
        /// The condition to verify is false.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void IsFalse(bool? condition, string message, params object[] parameters)
        {
            if (!condition.HasValue || condition.Value)
            {
                OnAssertFailed(FormatMessage(message, parameters, "Assert.IsFalse: Expected <False>, Actual <{0}>!", condition.HasValue ? "True" : "Null"));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null, "Assert.IsFalse"));
            }
        }
        #endregion

        #region IsNull
        /// <summary>
        /// Tests whether the specified object is null and fails
        /// the test if it is not.
        /// </summary>
        /// <param name="value">The object to verify is null.</param>
        public static void IsNull(object value)
        {
            IsNull(value, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified object is null and fails
        /// the test if it is not.
        /// </summary>
        /// <param name="value">The object to verify is null.</param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void IsNull(object value, string message)
        {
            IsNull(value, message, null);
        }

        /// <summary>
        /// Tests whether the specified object is null and fails
        /// the test if it is not.
        /// </summary>
        /// <param name="value">The object to verify is null.</param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void IsNull(object value, string message, params object[] parameters)
        {
            if (value != null)
            {
                OnAssertFailed(FormatMessage(message, parameters, "Assert.IsNull: Expected <null>, Actual <{0}>!", value));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null, "Assert.IsNull"));
            }
        }
        #endregion

        #region IsNotNull
        /// <summary>
        /// Tests whether the specified object is non-null and
        /// fails the test if it is null.
        /// </summary>
        /// <param name="value">
        /// The object to verify is not null.
        /// </param>
        public static void IsNotNull(object value)
        {
            IsNotNull(value, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified object is non-null and
        /// fails the test if it is null.
        /// </summary>
        /// <param name="value">
        /// The object to verify is not null.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void IsNotNull(object value, string message)
        {
            IsNotNull(value, message, null);
        }

        /// <summary>
        /// Tests whether the specified object is non-null and
        /// fails the test if it is null.
        /// </summary>
        /// <param name="value">
        /// The object to verify is not null.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void IsNotNull(object value, string message, params object[] parameters)
        {
            if (value == null)
            {
                OnAssertFailed(FormatMessage(message, parameters, "Assert.IsNotNull: Expected <non-null>, Actual <null>!"));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null, "Assert.IsNotNull"));
            }
        }
        #endregion

        #region AreEqual
        /// <summary>
        /// Test whether the two specified string are equal
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        public static void AreEqual(string expected, string actual)
        {
            AreEqualString(expected, actual, string.Empty);
        }

        private static void AreEqualString(string expected, string actual, string message, params object[] parameters)
        {
            if (!object.Equals(expected, actual))
            {
                char firstDifferentExpected, firstDifferentActual;
                int diffIndex = FindWhereStringsDiff(expected, actual, out firstDifferentExpected, out firstDifferentActual);
                OnAssertFailed(FormatMessage(message, parameters,
    @"Assert.AreEqual<System.String>: strings are not equal.  Strings differ at index {0}.  Expected char '{1}', got char '{2}'.
Expected: <""{3}"">
  Actual: <""{4}"">",
                    diffIndex, firstDifferentExpected, firstDifferentActual, expected, actual));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
    @"Assert.AreEqual<System.String>: Expected <{0}>", expected));
            }
        }

        /// <summary>
        /// Helper method that can find the first characted in the expected string where it differs against the actual string
        /// </summary>
        private static int FindWhereStringsDiff(string expected, string actual, out char firstDifferentExpected, out char firstDifferentActual)
        {
            Debug.Assert(expected != actual, "This should only be called when we know the strings are different");
            int lowerLength = Math.Min(expected.Length, actual.Length);
            if (lowerLength == 0)
            {
                if (expected.Length > 0)
                {
                    firstDifferentExpected = expected[0];
                    firstDifferentActual = ' ';
                }
                else
                {
                    firstDifferentActual = actual[0];
                    firstDifferentExpected = ' ';
                }
                return 0;
            }
            // Assign some dummy values - the loop below should override them.
            firstDifferentActual = '_';
            firstDifferentExpected = '_';
            for (int i = 0; i < lowerLength; i++)
            {
                if (expected[i] != actual[i])
                {
                    firstDifferentExpected = expected[i];
                    firstDifferentActual = actual[i];
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// Tests whether the specified values are equal and fails the
        /// test if the two values are not equal.
        /// </summary>
        /// <typeparam name="T">Type of objects to compare</typeparam>
        /// <param name="expected">
        /// The first value to compare. This is the value the test expects.
        /// </param>
        /// <param name="actual">
        /// The second value to compare. This is the value the test produced.
        /// </param>
        public static void AreEqual<T>(T expected, T actual)
        {
            AreEqual(expected, actual, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified values are equal and fails the
        /// test if the two values are not equal.
        /// </summary>
        /// <typeparam name="T">Type of objects to compare</typeparam>
        /// <param name="expected">
        /// The first value to compare. This is the value the test expects.
        /// </param>
        /// <param name="actual">
        /// The second value to compare. This is the value the test produced.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreEqual<T>(T expected, T actual, string message)
        {
            AreEqual(expected, actual, message, null);
        }

        /// <summary>
        /// Tests whether the specified values are equal and fails the
        /// test if the two values are not equal.
        /// </summary>
        /// <typeparam name="T">Type of objects to compare</typeparam>
        /// <param name="expected">
        /// The first value to compare. This is the value the test expects.
        /// </param>
        /// <param name="actual">
        /// The second value to compare. This is the value the test produced.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreEqual<T>(T expected, T actual, string message, params object[] parameters)
        {
            if (!object.Equals(expected, actual))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.AreEqual<{0}>: Expected <{1}>, Actual <{2}>!",
                    typeof(T).FullName, expected, actual));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.AreEqual<{0}>: Expected <{1}>",
                    typeof(T).FullName, expected));
            }
        }

        /// <summary>
        /// Tests whether the specified doubles are equal and fails
        /// the test if they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first double to compare. This is the double the tests expects.
        /// </param>
        /// <param name="actual">
        /// The second double to compare. This is the double the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. An exception will be thrown only if actual
        /// is different than expected by more than delta.
        /// </param>
        public static void AreEqual(double expected, double actual, double delta)
        {
            AreEqual(expected, actual, delta, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified doubles are equal and fails
        /// the test if they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first double to compare. This is the double the tests expects.
        /// </param>
        /// <param name="actual">
        /// The second double to compare. This is the double the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. An exception will be thrown only if actual
        /// is different than expected by more than delta.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreEqual(double expected, double actual, double delta, string message)
        {
            AreEqual(expected, actual, delta, message, null);
        }

        /// <summary>
        /// Tests whether the specified doubles are equal and fails
        /// the test if they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first double to compare. This is the double the tests expects.
        /// </param>
        /// <param name="actual">
        /// The second double to compare. This is the double the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. An exception will be thrown only if actual
        /// is different than expected by more than delta.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreEqual(double expected, double actual, double delta, string message, params object[] parameters)
        {
            if (Math.Abs(expected - actual) > delta)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.AreEqual: Expected <{0}>, Actual <{1}>, Delta <{2}>!",
                    expected, actual, delta));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.AreEqual: Expected <{0}>, Delta <{1}>!",
                    expected, delta));
            }
        }

        /// <summary>
        /// Tests whether the specified floats are equal and fails the test
        /// if they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first float to compare. This is the float the test expects.
        /// </param>
        /// <param name="actual">
        /// The second float to compare. This is the float the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. An exception will be thrown only if actual
        /// is different than expected by more than delta.
        /// </param>
        public static void AreEqual(float expected, float actual, float delta)
        {
            AreEqual(expected, actual, delta, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified floats are equal and fails the test
        /// if they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first float to compare. This is the float the test expects.
        /// </param>
        /// <param name="actual">
        /// The second float to compare. This is the float the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. An exception will be thrown only if actual
        /// is different than expected by more than delta.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreEqual(float expected, float actual, float delta, string message)
        {
            AreEqual(expected, actual, delta, message, null);
        }

        /// <summary>
        /// Tests whether the specified floats are equal and fails the test
        /// if they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first float to compare. This is the float the test expects.
        /// </param>
        /// <param name="actual">
        /// The second float to compare. This is the float the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. An exception will be thrown only if actual
        /// is different than expected by more than delta.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreEqual(float expected, float actual, float delta, string message, params object[] parameters)
        {
            if (Math.Abs(expected - actual) > delta)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.AreEqual: Expected <{0}>, Actual <{1}>, Delta <{2}>!",
                    expected, actual, delta));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.AreEqual: Expected <{0}>, Delta <{1}>!",
                    expected, delta));
            }
        }

        /// <summary>
        /// Tests whether the specified strings are equal and fails the test if
        /// they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first string to compare. This is the string the test expects.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        public static void AreEqual(string expected, string actual, bool ignoreCase)
        {
            AreEqual(expected, actual, ignoreCase, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified strings are equal and fails the test if
        /// they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first string to compare. This is the string the test expects.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreEqual(string expected, string actual, bool ignoreCase, string message)
        {
            AreEqual(expected, actual, ignoreCase, message, null);
        }

        /// <summary>
        /// Tests whether the specified strings are equal and fails the test if
        /// they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first string to compare. This is the string the test expects.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreEqual(string expected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            AreEqual(expected, actual, ignoreCase, CultureInfo.InvariantCulture, message, parameters);
        }

        /// <summary>
        /// Tests whether the specified strings are equal and fails the test if
        /// they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first string to compare. This is the string the test expects.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        /// <param name="culture">
        /// A CultureInfo object that supplies culture-specific comparison information.
        /// </param>
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture)
        {
            AreEqual(expected, actual, ignoreCase, culture, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified strings are equal and fails the test if
        /// they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first string to compare. This is the string the test expects.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        /// <param name="culture">
        /// A CultureInfo object that supplies culture-specific comparison information.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message)
        {
            AreEqual(expected, actual, ignoreCase, culture, message, null);
        }

        /// <summary>
        /// Tests whether the specified strings are equal and fails the test if
        /// they are not equal.
        /// </summary>
        /// <param name="expected">
        /// The first string to compare. This is the string the test expects.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        /// <param name="culture">
        /// A CultureInfo object that supplies culture-specific comparison information.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters)
        {
            if (string.Compare(expected, actual, ignoreCase, culture ?? CultureInfo.InvariantCulture) != 0)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.AreEqual: Expected <{0}>, Actual <{1}>!",
                    expected, actual));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.AreEqual: Expected <{0}>",
                    expected));
            }
        }
        #endregion

        #region AreNotEqual
        /// <summary>
        /// Tests whether the specified values are unequal and fails the
        /// test if the two values are equal.
        /// </summary>
        /// <typeparam name="T">Type of the objects to compare</typeparam>
        /// <param name="notExpected">
        /// The first value to compare. This is the value the test expects not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second value to compare. This is the value the test produced.
        /// </param>
        public static void AreNotEqual<T>(T notExpected, T actual)
        {
            AreNotEqual(notExpected, actual, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified values are unequal and fails the
        /// test if the two values are equal.
        /// </summary>
        /// <typeparam name="T">Type of the objects to compare</typeparam>
        /// <param name="notExpected">
        /// The first value to compare. This is the value the test expects not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second value to compare. This is the value the test produced.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreNotEqual<T>(T notExpected, T actual, string message)
        {
            AreNotEqual(notExpected, actual, message, null);
        }

        /// <summary>
        /// Tests whether the specified values are unequal and fails the
        /// test if the two values are equal.
        /// </summary>
        /// <typeparam name="T">Type of the objects to compare</typeparam>
        /// <param name="notExpected">
        /// The first value to compare. This is the value the test expects not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second value to compare. This is the value the test produced.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreNotEqual<T>(T notExpected, T actual, string message, params object[] parameters)
        {
            if (object.Equals(notExpected, actual))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.AreNotEqual<{0}>: Not Expected <{1}>, Actual <{2}>!",
                    typeof(T).FullName, notExpected, actual));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.AreNotEqual<{0}>: Not Expected <{1}>, Actual <{2}>!",
                    typeof(T).FullName, notExpected, actual));
            }
        }

        /// <summary>
        /// Tests whether the specified doubles are unequal and fails the test
        /// if they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first double to compare. This is the double the test expects not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second double to compare. This is the double the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. An exception will be thrown only if actual is different
        /// than expected by at most delta.
        /// </param>
        public static void AreNotEqual(double notExpected, double actual, double delta)
        {
            AreNotEqual(notExpected, actual, delta, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified doubles are unequal and fails the test
        /// if they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first double to compare. This is the double the test expects not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second double to compare. This is the double the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. An exception will be thrown only if actual is different
        /// than expected by at most delta.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreNotEqual(double notExpected, double actual, double delta, string message)
        {
            AreNotEqual(notExpected, actual, delta, message, null);
        }

        /// <summary>
        /// Tests whether the specified doubles are unequal and fails the test
        /// if they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first double to compare. This is the double the test expects not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second double to compare. This is the double the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. An exception will be thrown only if actual is different
        /// than expected by at most delta.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreNotEqual(double notExpected, double actual, double delta, string message, params object[] parameters)
        {
            if (Math.Abs(notExpected - actual) <= delta)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.AreNotEqual: Not Expected <{0}>, Actual <{1}>, Delta <{2}>!",
                    notExpected, actual, delta));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.AreNotEqual: Not Expected <{0}>, Actual <{1}>, Delta <{2}>!",
                    notExpected, actual, delta));
            }
        }

        /// <summary>
        /// Tests whether the specified floats are unequal and fails the test
        /// if they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first float to compare. This is the float the test expects.
        /// </param>
        /// <param name="actual">
        /// The second float to compare. This is the float the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. The test will fail only if actual is
        /// different than expected by more than delta.
        /// </param>
        public static void AreNotEqual(float notExpected, float actual, float delta)
        {
            AreNotEqual(notExpected, actual, delta, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified floats are unequal and fails the test
        /// if they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first float to compare. This is the float the test expects.
        /// </param>
        /// <param name="actual">
        /// The second float to compare. This is the float the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. The test will fail only if actual is
        /// different than expected by more than delta.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreNotEqual(float notExpected, float actual, float delta, string message)
        {
            AreNotEqual(notExpected, actual, delta, message, null);
        }

        /// <summary>
        /// Tests whether the specified floats are unequal and fails the test
        /// if they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first float to compare. This is the float the test expects.
        /// </param>
        /// <param name="actual">
        /// The second float to compare. This is the float the test produced.
        /// </param>
        /// <param name="delta">
        /// The required accuracy. The test will fail only if actual is
        /// different than expected by more than delta.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreNotEqual(float notExpected, float actual, float delta, string message, params object[] parameters)
        {
            if (Math.Abs(notExpected - actual) <= delta)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.AreNotEqual: Not Expected <{0}>, Actual <{1}>, Delta <{2}>!",
                    notExpected, actual, delta));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.AreNotEqual: Not Expected <{0}>, Actual <{1}>, Delta <{2}>!",
                    notExpected, actual, delta));
            }
        }

        /// <summary>
        /// Tests whether the specified strings are unequal and fails the
        /// test if they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first string to compare. This is the string the test expects
        /// not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase)
        {
            AreNotEqual(notExpected, actual, ignoreCase, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified strings are unequal and fails the
        /// test if they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first string to compare. This is the string the test expects
        /// not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message)
        {
            AreNotEqual(notExpected, actual, ignoreCase, message, null);
        }

        /// <summary>
        /// Tests whether the specified strings are unequal and fails the
        /// test if they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first string to compare. This is the string the test expects
        /// not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            AreNotEqual(notExpected, actual, ignoreCase, CultureInfo.InvariantCulture, message, parameters);
        }

        /// <summary>
        /// Tests whether the specified strings are unequal and fails the test if
        /// they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first string to compare. This is the string the test expects not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        /// <param name="culture">
        /// A CultureInfo object that supplies culture-specific comparison information.
        /// </param>
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture)
        {
            AreNotEqual(notExpected, actual, ignoreCase, culture, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified strings are unequal and fails the test if
        /// they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first string to compare. This is the string the test expects not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        /// <param name="culture">
        /// A CultureInfo object that supplies culture-specific comparison information.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message)
        {
            AreNotEqual(notExpected, actual, ignoreCase, culture, message, null);
        }

        /// <summary>
        /// Tests whether the specified strings are unequal and fails the test if
        /// they are equal.
        /// </summary>
        /// <param name="notExpected">
        /// The first string to compare. This is the string the test expects not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second string to compare. This is the string the test produced.
        /// </param>
        /// <param name="ignoreCase">
        /// A Boolean value that indicates a case-sensitive or insensitive
        /// comparison. true indicates a case-insensitive comparison.
        /// </param>
        /// <param name="culture">
        /// A CultureInfo object that supplies culture-specific comparison information.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters)
        {
            if (string.Compare(notExpected, actual, ignoreCase, culture ?? CultureInfo.InvariantCulture) == 0)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.AreNotEqual: Not Expected <{0}>, Actual <{1}>!",
                    notExpected, actual));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.AreNotEqual: Not Expected <{0}>, Actual <{1}>!",
                    notExpected, actual));
            }
        }
        #endregion

        #region AreSame
        /// <summary>
        /// Tests whether the specified objects both refer to the same object
        /// and fails the test if the two inputs do not refer to the same object.
        /// </summary>
        /// <param name="expected">
        /// The first object to compare. This is the value the test expects.
        /// </param>
        /// <param name="actual">
        /// The second object to compare. This is the value the test produced.
        /// </param>
        public static void AreSame(object expected, object actual)
        {
            AreSame(expected, actual, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified objects both refer to the same object
        /// and fails the test if the two inputs do not refer to the same object.
        /// </summary>
        /// <param name="expected">
        /// The first object to compare. This is the value the test expects.
        /// </param>
        /// <param name="actual">
        /// The second object to compare. This is the value the test produced.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreSame(object expected, object actual, string message)
        {
            AreSame(expected, actual, message, null);
        }

        /// <summary>
        /// Tests whether the specified objects both refer to the same object
        /// and fails the test if the two inputs do not refer to the same object.
        /// </summary>
        /// <param name="expected">
        /// The first object to compare. This is the value the test expects.
        /// </param>
        /// <param name="actual">
        /// The second object to compare. This is the value the test produced.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreSame(object expected, object actual, string message, params object[] parameters)
        {
            if (!object.ReferenceEquals(expected, actual))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.AreSame: Expected <{0}>, Actual <{1}>!",
                    expected, actual));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.AreSame: Expected <{0}>",
                    expected));
            }
        }
        #endregion

        #region AreNotSame
        /// <summary>
        /// Tests whether the specified objects refer to different objects
        /// and fails the test if the two inputs refer to the same object.
        /// </summary>
        /// <param name="notExpected">
        /// The first object to compare. This is the object the test expects
        /// not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second object to compare. This is the value the test produced.
        /// </param>
        public static void AreNotSame(object notExpected, object actual)
        {
            AreNotSame(notExpected, actual, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified objects refer to different objects
        /// and fails the test if the two inputs refer to the same object.
        /// </summary>
        /// <param name="notExpected">
        /// The first object to compare. This is the object the test expects
        /// not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second object to compare. This is the value the test produced.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void AreNotSame(object notExpected, object actual, string message)
        {
            AreNotSame(notExpected, actual, message, null);
        }

        /// <summary>
        /// Tests whether the specified objects refer to different objects
        /// and fails the test if the two inputs refer to the same object.
        /// </summary>
        /// <param name="notExpected">
        /// The first object to compare. This is the object the test expects
        /// not to match actual.
        /// </param>
        /// <param name="actual">
        /// The second object to compare. This is the value the test produced.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void AreNotSame(object notExpected, object actual, string message, params object[] parameters)
        {
            if (object.ReferenceEquals(notExpected, actual))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.AreNotSame: Not Expected <{0}>, Actual <{1}>!",
                    notExpected, actual));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.AreNotSame: Not Expected <{0}>!",
                    notExpected, actual));
            }
        }
        #endregion

        #region IsInstanceOfType
        /// <summary>
        /// Tests whether the specified object is an instance of the expected type
        /// and fails the test if the expected type is not in the inheritance
        /// hierarchy of the object.
        /// </summary>
        /// <param name="value">
        /// The object to verify is of the expected type.
        /// </param>
        /// <param name="expectedType">
        /// The expected type of value.
        /// </param>
        public static void IsInstanceOfType(object value, Type expectedType)
        {
            IsInstanceOfType(value, expectedType, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified object is an instance of the expected type
        /// and fails the test if the expected type is not in the inheritance
        /// hierarchy of the object.
        /// </summary>
        /// <param name="value">
        /// The object to verify is of the expected type.
        /// </param>
        /// <param name="expectedType">
        /// The expected type of value.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void IsInstanceOfType(object value, Type expectedType, string message)
        {
            IsInstanceOfType(value, expectedType, message, null);
        }

        /// <summary>
        /// Tests whether the specified object is an instance of the expected type
        /// and fails the test if the expected type is not in the inheritance
        /// hierarchy of the object.
        /// </summary>
        /// <param name="value">
        /// The object to verify is of the expected type.
        /// </param>
        /// <param name="expectedType">
        /// The expected type of value.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void IsInstanceOfType(object value, Type expectedType, string message, params object[] parameters)
        {
            if (expectedType == null || !expectedType.IsInstanceOfType(value))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.IsInstanceOfType: Expected Type <{0}>, Actual Type <{1}>, Value <{2}>!",
                    expectedType.FullName, value == null ? null : value.GetType().FullName, value));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.IsInstanceOfType: Expected Type <{0}>, Value <{1}>!",
                    expectedType.FullName, value));
            }
        }
        #endregion

        #region IsNotInstanceOfType
        /// <summary>
        /// Tests whether the specified object is not an instance of the wrong type
        /// and fails the test if the specified type is in the inheritance hierarchy
        /// of the object.
        /// </summary>
        /// <param name="value">The object to verify is not of the wrong type.</param>
        /// <param name="wrongType">The type that value should not be.</param>
        public static void IsNotInstanceOfType(object value, Type wrongType)
        {
            IsNotInstanceOfType(value, wrongType, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified object is not an instance of the wrong type
        /// and fails the test if the specified type is in the inheritance hierarchy
        /// of the object.
        /// </summary>
        /// <param name="value">The object to verify is not of the wrong type.</param>
        /// <param name="wrongType">The type that value should not be.</param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void IsNotInstanceOfType(object value, Type wrongType, string message)
        {
            IsNotInstanceOfType(value, wrongType, string.Empty, null);
        }

        /// <summary>
        /// Tests whether the specified object is not an instance of the wrong type
        /// and fails the test if the specified type is in the inheritance hierarchy
        /// of the object.
        /// </summary>
        /// <param name="value">The object to verify is not of the wrong type.</param>
        /// <param name="wrongType">The type that value should not be.</param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void IsNotInstanceOfType(object value, Type wrongType, string message, params object[] parameters)
        {
            if (wrongType == null || wrongType.IsInstanceOfType(value))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.IsNotInstanceOfType: Wrong Type <{0}>, Actual Type <{1}>, Value <{2}>!",
                    wrongType.FullName, value == null ? null : value.GetType().FullName, value));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.IsNotInstanceOfType: Unexpected Type <{0}>, Actual Type <{1}>, Value <{2}>!",
                    wrongType.FullName, value == null ? null : value.GetType().FullName, value));
            }
        }
        #endregion

        #region StringContains
        /// <summary>
        /// Tests whether the specified string contains the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to contain substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to occur within value.
        /// </param>
        public static void StringContains(string value, string substring)
        {
            StringContains(value, substring, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified string contains the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to contain substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to occur within value.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void StringContains(string value, string substring, string message)
        {
            StringContains(value, substring, message, null);
        }

        /// <summary>
        /// Tests whether the specified string contains the specified substring.
        /// Note: we want to keep wthis version along with new one that takes numberOfTimes parameter
        /// since this version tells if substring contained in value at least one time, when another 
        /// overload expects exactly the specified number of times. So if we would have substring in 
        /// value more that specified number of occurences, then it will fail.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to contain substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to occur within value.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void StringContains(string value, string substring, string message, params object[] parameters)
        {
            if (value == null || value.IndexOf(substring, StringComparison.Ordinal) < 0)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringContains: Value <{0}>, Expected Substring <{1}>!",
                    value, substring));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.StringContains: Value <{0}>, contains Substring <{1}>!",
                    value, substring));
            }
        }

        /// <summary>
        /// Verify whether the specified string contains given substring specified number of times
        /// </summary>
        /// <param name="value">
        /// The string that is expected to contain substring.
        /// </param>
        /// <param name="substring">
        /// the string that is expected to occur within value specified number of times.
        /// </param>
        /// <param name="numberOfOccurences">
        /// the number of times substring is expecetd to occuer within value
        /// </param>
        public static void StringContains(string value, string substring, int numberOfOccurences)
        {
            StringContains(value, substring, numberOfOccurences, null, null);
        }

        /// <summary>
        /// Verify whether the specified string contains given substring specified number of times
        /// </summary>
        /// <param name="value">
        /// The string that is expected to contain substring.
        /// </param>
        /// <param name="substring">
        /// the string that is expected to occur within value specified number of times.
        /// </param>
        /// <param name="numberOfOccurences">
        /// the number of times substring is expecetd to occuer within value
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>        
        public static void StringContains(string value, string substring, int numberOfOccurences, string message)
        {
            StringContains(value, substring, numberOfOccurences, message, null);
        }

        /// <summary>
        /// Verify whether the specified string contains mutiple time of the specified substring
        /// </summary>
        /// <param name="value">
        /// The string that is expected to contain substring.
        /// </param>
        /// <param name="substring">
        /// the strings is expected to occur within value.
        /// </param>
        /// <param name="numberOfOccurences">
        /// the number is number of times of the string to occur within value .
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void StringContains(string value, string substring, int numberOfOccurences, string message, params object[] parameters)
        {
            if (value == null)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringContains: Value: <null>, source string can not be null."));
            }
            else if (string.IsNullOrEmpty(substring))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringContains: substring, to search for, can not be null or empty."));
            }
            else
            {
                int counter = -1;
                int startIndex = -substring.Length;
                do
                {
                    counter++;
                    startIndex += substring.Length;
                    startIndex = value.IndexOf(substring, startIndex, StringComparison.Ordinal);
                } while (startIndex >= 0);

                if (counter == numberOfOccurences)
                {
                    OnAssertPassed(FormatMessage(null, null,
                        "Assert.StringContains: Value: <{0}>, contains substring <{1}> correct number of times: <{2}>!",
                        value, substring, numberOfOccurences));
                }
                else
                {
                    OnAssertFailed(FormatMessage(message, parameters,
                        "Assert.StringContains: Value: <{0}>, contains substring: <{1}> incorrect number of times. Expected number of times: <{2}>, Actual: <{3}>.",
                        value, substring, numberOfOccurences, counter));
                }
            }
        }

        #endregion

        #region StringDoesNotContain

        /// <summary>
        /// Tests whether the specified string does not contain the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is should not contain the substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to not occur within value.
        /// </param>
        public static void StringDoesNotContain(string value, string substring)
        {
            StringDoesNotContain(value, substring, null, null);
        }

        /// <summary>
        /// Tests whether the specified string does not contain the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is should not contain the substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to not occur within value.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void StringDoesNotContain(string value, string substring, string message)
        {
            StringDoesNotContain(value, substring, message, null);
        }

        /// <summary>
        /// Tests whether the specified string does not contain the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is should not contain the substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to not occur within value.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void StringDoesNotContain(string value, string substring, string message, params object[] parameters)
        {
            if (value == null || value.IndexOf(substring, StringComparison.Ordinal) != -1)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringDoesNotContain: Value <{0}>, Contains Substring <{1}>!",
                    value, substring));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.StringContains: Value <{0}>, Does Not Contain Substring <{1}>!",
                    value, substring));
            }
        }

        #endregion 

        #region StringContainsInOrder

        /// <summary>
        /// Verify whether the specified string contains specified substrings in correct order
        /// </summary>
        /// <param name="value">
        /// The string that is expected to contain substrings.
        /// </param>
        /// <param name="substrings">
        /// substrings expected to occur within value in correct order.
        /// </param>
        public static void StringContainsInOrder(string value, string[] substrings)
        {
            StringContainsInOrder(value, substrings, null, null);
        }

        /// <summary>
        /// Verify whether the specified string contains specified substrings in correct order
        /// </summary>
        /// <param name="value">
        /// The string that is expected to contain substrings.
        /// </param>
        /// <param name="substrings">
        /// substrings expected to occur within value in correct order.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void StringContainsInOrder(string value, string[] substrings, string message)
        {
            StringContainsInOrder(value, substrings, null, null);
        }

        /// <summary>
        /// Verify whether the specified string contains specified substrings in correct order
        /// </summary>
        /// <param name="value">
        /// The string that is expected to contain substrings.
        /// </param>
        /// <param name="substrings">
        /// substrings expected to occur within value in correct order.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void StringContainsInOrder(string value, string[] substrings, string message, params object[] parameters)
        {
            if (value == null)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringContainsInOrder: Value: <null>, source string can not be null."));
            }
            else if (substrings == null || substrings.Length == 0)
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringContainsInOrder: substrings, to search for, were not specified."));
            }
            else
            {
                int startIndex = 0;
                foreach (string nextString in substrings)
                {
                    startIndex = value.IndexOf(nextString, startIndex, StringComparison.Ordinal);

                    if (startIndex < 0)
                    {
                        OnAssertFailed(FormatMessage(message, parameters,
                            "Assert.StringContainsInOrder: Value: <{0}>, Failed at substring: <{1}>, Expected multi substrings <{2}>.",
                            value, nextString, string.Join(", ", substrings)));

                        return;
                    }

                    startIndex += nextString.Length;
                }

                OnAssertPassed(FormatMessage(null, null,
                    "Assert.StringContainsInOrder: Value: <{0}>, contains multi substrings in correct order: <{1}>!",
                    value, string.Join(", ", substrings)));
            }
        }

        #endregion

        #region StringStartsWith
        /// <summary>
        /// Tests whether the specified string begins with the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to begin with substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to be a prefix of value.
        /// </param>
        public static void StringStartsWith(string value, string substring)
        {
            StringStartsWith(value, substring, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified string begins with the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to begin with substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to be a prefix of value.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void StringStartsWith(string value, string substring, string message)
        {
            StringStartsWith(value, substring, message, null);
        }

        /// <summary>
        /// Tests whether the specified string begins with the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to begin with substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to be a prefix of value.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void StringStartsWith(string value, string substring, string message, params object[] parameters)
        {
            if (value == null || !value.StartsWith(substring, StringComparison.Ordinal))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringStartsWith: Value <{0}>, Expected Substring <{1}>!",
                    value, substring));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.StringStartsWith: Value <{0}>, Starts with <{1}>!",
                    value, substring));
            }
        }
        #endregion

        #region StringEndsWith
        /// <summary>
        /// Tests whether the specified string ends with the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to end with substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to be a suffix of value.
        /// </param>
        public static void StringEndsWith(string value, string substring)
        {
            StringEndsWith(value, substring, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified string ends with the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to end with substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to be a suffix of value.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void StringEndsWith(string value, string substring, string message)
        {
            StringEndsWith(value, substring, message, null);
        }

        /// <summary>
        /// Tests whether the specified string ends with the specified substring.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to end with substring.
        /// </param>
        /// <param name="substring">
        /// The string expected to be a suffix of value.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void StringEndsWith(string value, string substring, string message, params object[] parameters)
        {
            if (value == null || !value.EndsWith(substring, StringComparison.Ordinal))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringEndsWith: Value <{0}>, Expected Substring <{1}>!",
                    value, substring));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.StringEndsWith: Value <{0}>, Ends with substring <{1}>!",
                    value, substring));
            }
        }
        #endregion

        #region StringMatches
        /// <summary>
        /// Tests whether the specified string matches a regular expression.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to match pattern.
        /// </param>
        /// <param name="pattern">
        /// The regular expression that value is expected to match.
        /// </param>
        public static void StringMatches(string value, Regex pattern)
        {
            StringMatches(value, pattern, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified string matches a regular expression.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to match pattern.
        /// </param>
        /// <param name="pattern">
        /// The regular expression that value is expected to match.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void StringMatches(string value, Regex pattern, string message)
        {
            StringMatches(value, pattern, message, null);
        }

        /// <summary>
        /// Tests whether the specified string matches a regular expression.
        /// </summary>
        /// <param name="value">
        /// The string that is expected to match pattern.
        /// </param>
        /// <param name="pattern">
        /// The regular expression that value is expected to match.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void StringMatches(string value, Regex pattern, string message, params object[] parameters)
        {
            if (pattern == null || !pattern.IsMatch(value))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringMatches: Value <{0}>, Expression To Match <{1}>!",
                    value, pattern == null ? null : pattern.ToString()));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.StringMatches: Value <{0}>, Expression Matched <{1}>!",
                    value, pattern == null ? null : pattern.ToString()));
            }
        }
        #endregion

        #region StringDoesNotMatch
        /// <summary>
        /// Tests whether the specified string does not match a regular expression.
        /// </summary>
        /// <param name="value">
        /// The string that is not expected to match pattern.
        /// </param>
        /// <param name="pattern">
        /// The regular expression that value is not expected to match.
        /// </param>
        public static void StringDoesNotMatch(string value, Regex pattern)
        {
            StringDoesNotMatch(value, pattern, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified string does not match a regular expression.
        /// </summary>
        /// <param name="value">
        /// The string that is not expected to match pattern.
        /// </param>
        /// <param name="pattern">
        /// The regular expression that value is not expected to match.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void StringDoesNotMatch(string value, Regex pattern, string message)
        {
            StringDoesNotMatch(value, pattern, message, null);
        }

        /// <summary>
        /// Tests whether the specified string does not match a regular expression.
        /// </summary>
        /// <param name="value">
        /// The string that is not expected to match pattern.
        /// </param>
        /// <param name="pattern">
        /// The regular expression that value is not expected to match.
        /// </param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void StringDoesNotMatch(string value, Regex pattern, string message, params object[] parameters)
        {
            if (pattern == null || pattern.IsMatch(value))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringDoesNotMatch: Value <{0}>, Expression To Not Match <{1}>!",
                    value, pattern == null ? null : pattern.ToString()));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.StringDoesNotMatch: Value <{0}>, Expression That Does Not Match <{1}>!",
                    value, pattern == null ? null : pattern.ToString()));
            }
        }
        #endregion

        #region StringIsNullOrEmpty
        /// <summary>
        /// Tests whether the specified object is null or empty and
        /// fails the test if it is not.
        /// </summary>
        /// <param name="value">The object to verify is null or empty.</param>
        public static void StringIsNullOrEmpty(string value)
        {
            StringIsNullOrEmpty(value, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified object is null or empty and
        /// fails the test if it is not.
        /// </summary>
        /// <param name="value">The object to verify is null or empty.</param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void StringIsNullOrEmpty(string value, string message)
        {
            StringIsNullOrEmpty(value, message, null);
        }

        /// <summary>
        /// Tests whether the specified object is null or empty and
        /// fails the test if it is not.
        /// </summary>
        /// <param name="value">The object to verify is null or empty.</param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void StringIsNullOrEmpty(string value, string message, params object[] parameters)
        {
            if (!string.IsNullOrEmpty(value))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringIsNullOrEmpty: Expected: <null or String.Empty>, Value <{0}>!",
                    value));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.StringIsNullOrEmpty"));
            }
        }
        #endregion

        #region StringIsNotNullOrEmpty
        /// <summary>
        /// Tests whether the specified object is null or empty
        /// and fails the test if it is.
        /// </summary>
        /// <param name="value">The object to verify is not null or empty.</param>
        public static void StringIsNotNullOrEmpty(string value)
        {
            StringIsNotNullOrEmpty(value, string.Empty);
        }

        /// <summary>
        /// Tests whether the specified object is null or empty
        /// and fails the test if it is.
        /// </summary>
        /// <param name="value">The object to verify is not null or empty.</param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        public static void StringIsNotNullOrEmpty(string value, string message)
        {
            StringIsNotNullOrEmpty(value, message, null);
        }

        /// <summary>
        /// Tests whether the specified object is null or empty
        /// and fails the test if it is.
        /// </summary>
        /// <param name="value">The object to verify is not null or empty.</param>
        /// <param name="message">
        /// A message to include with the exception if one is thrown.
        /// This message can be seen in the test results.
        /// </param>
        /// <param name="parameters">
        /// An array of parameters to use when formatting message.
        /// </param>
        public static void StringIsNotNullOrEmpty(string value, string message, params object[] parameters)
        {
            if (string.IsNullOrEmpty(value))
            {
                OnAssertFailed(FormatMessage(message, parameters,
                    "Assert.StringIsNotNullOrEmpty: Expected: <non-null and not String.Empty>, Value <{0}>!",
                    value == null ? "null" : "String.Empty"));
            }
            else
            {
                OnAssertPassed(FormatMessage(null, null,
                    "Assert.StringIsNotNullOrEmpty: Expected: <non-null and not String.Empty>, Actual Value <{0}>!",
                    value == null ? "null" : "String.Empty"));
            }
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Format a message and include a standard message if provided
        /// </summary>
        /// <param name="message">Message format string</param>
        /// <param name="parameters">Message format string parameters</param>
        /// <param name="standardMessage">Standard message format string</param>
        /// <param name="standardParameters">Standard message format string parameters</param>
        /// <returns>Message</returns>
        private static string FormatMessage(string message, object[] parameters, string standardMessage, params object[] standardParameters)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (parameters != null)
                {
                    message = string.Format(message, parameters);
                }

                return message;
            }
            else
            {
                standardMessage = string.Format(standardMessage, standardParameters ?? new object[] { });
                if (!string.IsNullOrEmpty(standardMessage))
                {
                    return standardMessage;
                }
                else
                {
                    return "Assertion failed!";
                }
            }
        }

        /// <summary>
        /// Prevent static Object.Equals from being mistakenly called as an assertion
        /// </summary>
        /// <param name="objA">Object to compare</param>
        /// <param name="objB">Object to compare</param>
        /// <returns>True if equal, False otherwise</returns>
        [Obsolete("Assert.Equals is not an assertion!  Use Assert.AreEqual instead.")]
        public new static bool Equals(object objA, object objB)
        {
            throw new InvalidOperationException("Assert.Equals is not an assertion!  Use Assert.AreEqual instead.");
        }

        /// <summary>
        /// Prevent static Object.ReferenceEquals from being mistakenly called as an assertion
        /// </summary>
        /// <param name="objA">Object to compare</param>
        /// <param name="objB">Object to compare</param>
        /// <returns>True if equal, False otherwise</returns>
        [Obsolete("Assert.ReferenceEquals is not an assertion!  Use Assert.AreSame instead.")]
        public new static bool ReferenceEquals(object objA, object objB)
        {
            throw new InvalidOperationException("Assert.ReferenceEquals is not an assertion!  Use Assert.AreSame instead.");
        }
        #endregion Utilities
    }
}