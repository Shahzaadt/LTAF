using System;
using System.Collections.Generic;
using System.Text;
using LTAF;

using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using MWTAssert = LTAF.Assert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace LTAF.UnitTests
{
    public class CustomAssert : Assert
    {
        public static void MyCustomCheck(bool pass)
        {
            if (pass)
            {
                OnAssertPassed("Passed");
            }
            else
            {
                OnAssertFailed("Failed");
            }
        }
    }

    [TestClass]
    public class AssertTest
    {
        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void InheritFromAssertClass_VerifyUsingProtectedMethodsToReportFailure()
        {
            ServiceLocator.AssertResultHandler = null;
            CustomAssert.MyCustomCheck(false);
        }

        [TestMethod]
        public void AreEqual_NoErrorIfStringsAreEqual()
        {
            ServiceLocator.AssertResultHandler = null;
            MWTAssert.AreEqual("Foo bar!  Whee!", "Foo bar!  Whee!");
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void AreEqual_ErrorIsStringsAreNotEqual()
        {
            ServiceLocator.AssertResultHandler = null;
            MWTAssert.AreEqual("Test string1", "Test string2");
        }

        [TestMethod]
        public void AreEqual_VerifyErrorMessageWhenStringsAreNotEqualFirstCharacter()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreEqual("a String one", "b String two");
            
            UnitTestAssert.AreEqual(
@"Assert.AreEqual<System.String>: strings are not equal.  Strings differ at index 0.  Expected char 'a', got char 'b'.
Expected: <""a String one"">
  Actual: <""b String two"">",
                    mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreEqual_VerifyErrorMessageWhenStringsAreNotEqualThirdCharacter()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreEqual("String one", "StAing two");
            UnitTestAssert.AreEqual(
@"Assert.AreEqual<System.String>: strings are not equal.  Strings differ at index 2.  Expected char 'r', got char 'A'.
Expected: <""String one"">
  Actual: <""StAing two"">",
                    mockAssert.LastMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void Faile_ThrowsAnException()
        {
            ServiceLocator.AssertResultHandler = null;
            MWTAssert.Fail();
        }
        
        [TestMethod]
        [ExpectedException(typeof(WebTestException), "This is a message {0}")]
        public void Fail_ThrowsExceptionWithPlaceholdersInTheMessage()
        {
            ServiceLocator.AssertResultHandler = null;
            MWTAssert.Fail("This is a message {0}");
        }

        [TestMethod]
        public void AreEqual_PassWithMessage()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreEqual("foo", "foo", "Error Message");
            UnitTestAssert.AreEqual("Assert.AreEqual<System.String>: Expected <foo>", mockAssert.LastMessage);

            MWTAssert.AreEqual(1, 1, "Error Message");
            UnitTestAssert.AreEqual("Assert.AreEqual<System.Int32>: Expected <1>", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreEqual_Object_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            object o1 = new List<string>();
            MWTAssert.AreEqual(o1, o1, "Error Message");
            UnitTestAssert.AreEqual("Assert.AreEqual<System.Object>: Expected <System.Collections.Generic.List`1[System.String]>", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreEqual_Integers_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreEqual(1, 2, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreEqual_Float_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreEqual(1.1, 1.5, 0.8);
            MWTAssert.AreEqual(1.1, 1.5, 0.2, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreEqual_String_PassIfIgnoreCase()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreEqual("foo", "FOO", true, "Error Message");
            UnitTestAssert.AreEqual("Assert.AreEqual: Expected <foo>", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreEqual_String_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreEqual("foo", "FOO", false, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreNotEqual_String_PassWithMessage()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreNotEqual("foo", "bar", "Error Message");
            UnitTestAssert.AreEqual("Assert.AreNotEqual<System.String>: Not Expected <foo>, Actual <bar>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreNotEqual_Int_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreNotEqual(1, 2, "Error Message");
            UnitTestAssert.AreEqual("Assert.AreNotEqual<System.Int32>: Not Expected <1>, Actual <2>!", mockAssert.LastMessage );
        }

        [TestMethod]
        public void AreNotEqual_Int_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreNotEqual(1, 1, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreNotEqual_String_PassCaseInsensitive()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreNotEqual("foo", "FOO", false, "Error Message");
            UnitTestAssert.AreEqual("Assert.AreNotEqual: Not Expected <foo>, Actual <FOO>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreNotEqual_String_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreNotEqual("foo", "FOO", false);
            MWTAssert.AreNotEqual("foo", "FOO", true, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreNotEqual_Float_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreNotEqual((float)1.0, (float) 1.9, (float) 0.2, "Error Message");
            UnitTestAssert.AreEqual("Assert.AreNotEqual: Not Expected <1>, Actual <1.9>, Delta <0.2>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreNotEqual_Float_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.AreNotEqual(1.0, 1.3, 0.8, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreNotEqual_Object_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            object o1 = new List<string>();
            object o2 = new List<int>();
            MWTAssert.AreNotEqual(o1, o2, "Error Message");
            UnitTestAssert.AreEqual("Assert.AreNotEqual<System.Object>: Not Expected <System.Collections.Generic.List`1[System.String]>, Actual <System.Collections.Generic.List`1[System.Int32]>!", mockAssert.LastMessage);
     
        }

        [TestMethod]
        public void AreNotSame_PassWithMessage()
        {
            object o1 = new object();
            object o2 = new object();

            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;

            MWTAssert.AreNotSame(o1, o2, "Error Message");
            UnitTestAssert.AreEqual("Assert.AreNotSame: Not Expected <System.Object>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreNotSame_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;

            object o1 = new object();
            object o2 = new object();
            MWTAssert.AreNotSame(o1, o1, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreSame_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            object o1 = new object();
            object o2 = new object();
            MWTAssert.AreSame(o1, o1, "Error Message");
            UnitTestAssert.AreEqual("Assert.AreSame: Expected <System.Object>", mockAssert.LastMessage);
        }

        [TestMethod]
        public void AreSame_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            object o1 = new object();
            object o2 = new object();
            MWTAssert.AreSame(o1, o2, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void Inconclusive_ThrowsException()
        {
            ServiceLocator.AssertResultHandler = null;
            MWTAssert.Inconclusive();
        }

        [TestMethod]
        public void IsFalse_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.IsFalse(false, "Error Message");
            UnitTestAssert.AreEqual("Assert.IsFalse", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsFalse_Nullable_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            bool? condition = false;
            MWTAssert.IsFalse(condition, "Error Message");
            UnitTestAssert.AreEqual("Assert.IsFalse", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsFalse_Nullable_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            bool? condition = null;
            MWTAssert.IsFalse(condition, "Error Message");
            UnitTestAssert.AreEqual("Error Message", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsFalse_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.IsFalse(true, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsInstanceOfType_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            ApplicationPathFinder finder = new ApplicationPathFinder("foo");
            MWTAssert.IsInstanceOfType(finder, typeof(ApplicationPathFinder), "Error Message");
            UnitTestAssert.AreEqual("Assert.IsInstanceOfType: Expected Type <LTAF.ApplicationPathFinder>, Value <LTAF.ApplicationPathFinder>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsInstanceOfType_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            ApplicationPathFinder finder = new ApplicationPathFinder("foo");
            MWTAssert.IsInstanceOfType(finder, typeof(int), "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsNotInstanceOfType_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            ApplicationPathFinder finder = new ApplicationPathFinder("foo");
            MWTAssert.IsNotInstanceOfType(finder, typeof(int), "Error Message");
            UnitTestAssert.AreEqual("Assert.IsNotInstanceOfType: Unexpected Type <System.Int32>, Actual Type <LTAF.ApplicationPathFinder>, Value <LTAF.ApplicationPathFinder>!",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsNotInstanceOfType_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            ApplicationPathFinder finder = new ApplicationPathFinder("foo");
            MWTAssert.IsNotInstanceOfType(finder, typeof(ApplicationPathFinder), "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsNotNull_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            ApplicationPathFinder finder = new ApplicationPathFinder("foo");
            MWTAssert.IsNotNull(finder, "Error Message");
            UnitTestAssert.AreEqual("Assert.IsNotNull", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsNotNull_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            ApplicationPathFinder finder = new ApplicationPathFinder("foo");
            object o = null;
            MWTAssert.IsNotNull(o, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsNull_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            object o = null;
            MWTAssert.IsNull(o, "Error Message");
            UnitTestAssert.AreEqual("Assert.IsNull", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsNull_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            ApplicationPathFinder finder = new ApplicationPathFinder("foo");
            MWTAssert.IsNull(finder, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsTrue_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.IsTrue(true, "Error Message");
            UnitTestAssert.AreEqual("Assert.IsTrue", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsTrue_Nullable_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            bool? condition = true;
            MWTAssert.IsTrue(condition, "Error Message");
            UnitTestAssert.AreEqual("Assert.IsTrue", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsTrue_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.IsTrue(false, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void IsTrue_Nullable_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            bool? condition = null;
            MWTAssert.IsFalse(condition, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        #region StringContainsInOrder

        [TestMethod]
        public void StringContainsInOrder_ValueNull_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContainsInOrder(null, new string[] { "token1", "token3", "token2" });
            UnitTestAssert.AreEqual("Assert.StringContainsInOrder: Value: <null>, source string can not be null.",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContainsInOrder_NoSubstringsToSearch_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContainsInOrder("some string", null);
            UnitTestAssert.AreEqual("Assert.StringContainsInOrder: substrings, to search for, were not specified.",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContainsInOrder_OneSubstring_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;

            // at the very begining of the string
            MWTAssert.StringContainsInOrder("foobar", new string[] { "foo" });
            UnitTestAssert.AreEqual("Assert.StringContainsInOrder: Value: <foobar>, contains multi substrings in correct order: <foo>!", 
                mockAssert.LastMessage);

            // at the very end of the string
            MWTAssert.StringContainsInOrder("barfoo", new string[] { "foo" });
            UnitTestAssert.AreEqual("Assert.StringContainsInOrder: Value: <barfoo>, contains multi substrings in correct order: <foo>!", 
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContainsInOrder_SeveralSubstrings_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContainsInOrder("token1sometext token2 some another text token3 some text2", new string[] { "token1", "token2", "token3" });
            UnitTestAssert.AreEqual("Assert.StringContainsInOrder: Value: <token1sometext token2 some another text token3 some text2>, contains multi substrings in correct order: <token1, token2, token3>!",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContainsInOrder_SeveralSubstrings_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContainsInOrder("token1sometext token2 some another text token3 some text2", new string[] { "token1", "token3", "token2" });
            UnitTestAssert.AreEqual("Assert.StringContainsInOrder: Value: <token1sometext token2 some another text token3 some text2>, Failed at substring: <token2>, Expected multi substrings <token1, token3, token2>.",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContainsInOrder_OneSubstring_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContainsInOrder("token1sometext token2 some another text token3 some text2", new string[] { "token4" });
            UnitTestAssert.AreEqual("Assert.StringContainsInOrder: Value: <token1sometext token2 some another text token3 some text2>, Failed at substring: <token4>, Expected multi substrings <token4>.",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContainsInOrder_SeveralEqualSubstrings_Fail()
        {
            // Note: the logic here is that if we specified two token two times, we expect token to be in the string two times.
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContainsInOrder("token1sometext token2 some another text token3 some text2", new string[] { "token1", "token1", "token3" });
            UnitTestAssert.AreEqual("Assert.StringContainsInOrder: Value: <token1sometext token2 some another text token3 some text2>, Failed at substring: <token1>, Expected multi substrings <token1, token1, token3>.",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContainsInOrder_SeveralOverlappingSubstrings_Fail()
        {
            // Note: the logic here is that if we specified two token two times, we expect token to be in the string two times.
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContainsInOrder("ababab", new string[] { "aba", "abab" });
            UnitTestAssert.AreEqual("Assert.StringContainsInOrder: Value: <ababab>, Failed at substring: <abab>, Expected multi substrings <aba, abab>.",
                mockAssert.LastMessage);
        }

        #endregion

        #region StringContains

        [TestMethod]
        public void StringContains_ValueNull_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContains(null, "token1", 1);
            UnitTestAssert.AreEqual("Assert.StringContains: Value: <null>, source string can not be null.",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContains_SubstringNull_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContains("some search string", null, 1);
            UnitTestAssert.AreEqual("Assert.StringContains: substring, to search for, can not be null or empty.",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContains_SubstringEmpty_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContains("some search string", "", 1);
            UnitTestAssert.AreEqual("Assert.StringContains: substring, to search for, can not be null or empty.",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContains_SubstringRecur_1_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContains("some search string", "string", 1);
            UnitTestAssert.AreEqual("Assert.StringContains: Value: <some search string>, contains substring <string> correct number of times: <1>!",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContains_SubstringRecur_3_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContains("some token1 search string token1 some antoher text token token1", "token1", 3);
            UnitTestAssert.AreEqual("Assert.StringContains: Value: <some token1 search string token1 some antoher text token token1>, contains substring <token1> correct number of times: <3>!",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContains_SubstringRecur_3_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContains("some token1 search string token1 some antoher text token token2", "token1", 3);
            UnitTestAssert.AreEqual("Assert.StringContains: Value: <some token1 search string token1 some antoher text token token2>, contains substring: <token1> incorrect number of times. Expected number of times: <3>, Actual: <2>.",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContains_SubstringRecur_0_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContains("some search string", "string1", 0);
            UnitTestAssert.AreEqual("Assert.StringContains: Value: <some search string>, contains substring <string1> correct number of times: <0>!",
                mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContains_SubstringRecur_Overlapping_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContains("ababababa", "aba", 2);
            UnitTestAssert.AreEqual("Assert.StringContains: Value: <ababababa>, contains substring <aba> correct number of times: <2>!",
                mockAssert.LastMessage);
        }

        #endregion


        [TestMethod]
        public void StringContains_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContains("foobar", "foo", "Error Message");
            UnitTestAssert.AreEqual("Assert.StringContains: Value <foobar>, contains Substring <foo>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringContains_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringContains("foobar", "baz", "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringDoesNotContains_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringDoesNotContain("foobar", "baz", "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("Assert.StringContains: Value <foobar>, Does Not Contain Substring <baz>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringDoesNotContains_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringDoesNotContain("foobar", "bar", "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringDoesNotMatch_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringDoesNotMatch("foobar", new Regex("baz"), "Error Message");
            UnitTestAssert.AreEqual("Assert.StringDoesNotMatch: Value <foobar>, Expression That Does Not Match <baz>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringDoesNotMatch_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringDoesNotMatch("foobar", new Regex("foo"), "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringEndsWith_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringEndsWith("foobar", "bar", "Error Message");
            UnitTestAssert.AreEqual("Assert.StringEndsWith: Value <foobar>, Ends with substring <bar>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringEndsWith_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringEndsWith("foobar", "foo", "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringIsNotNullOrEmpty_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringIsNotNullOrEmpty("foobar", "Error Message");
            UnitTestAssert.AreEqual("Assert.StringIsNotNullOrEmpty: Expected: <non-null and not String.Empty>, Actual Value <String.Empty>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringIsNotNullOrEmpty_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            string s = null;
            MWTAssert.StringIsNotNullOrEmpty(s, "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringIsNullOrEmpty_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            string s = null;
            MWTAssert.StringIsNullOrEmpty(s, "Error Message");
            UnitTestAssert.AreEqual("Assert.StringIsNullOrEmpty", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringIsNullOrEmpty_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringIsNullOrEmpty("foo", "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringMatches_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringMatches("foobar", new Regex("bar"), "Error Message");
            UnitTestAssert.AreEqual("Assert.StringMatches: Value <foobar>, Expression Matched <bar>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringMatches_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringMatches("foobar", new Regex("baz"), "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringStartsWith_Pass()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringStartsWith("foobar", "foo", "Error Message");
            UnitTestAssert.AreEqual("Assert.StringStartsWith: Value <foobar>, Starts with <foo>!", mockAssert.LastMessage);
        }

        [TestMethod]
        public void StringStartsWith_Fail()
        {
            MockAssertResultHandler mockAssert = new MockAssertResultHandler();
            ServiceLocator.AssertResultHandler = mockAssert;
            MWTAssert.StringStartsWith("foobar", "bar", "This is a message '{0}'.", "foo");
            UnitTestAssert.AreEqual("This is a message 'foo'.", mockAssert.LastMessage);
        }
    }
}
