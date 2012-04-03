using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.IO;
using System.Linq;
using Moq;

namespace LTAF.UnitTests.UI
{
    [TestClass]
    public class HtmlFormElementTest
    {
        private Mock<IBrowserCommandExecutorFactory> _commandExecutorFactory;

        [TestInitialize]
        public void Initialize()
        {
            _commandExecutorFactory = new Mock<IBrowserCommandExecutorFactory>();
            ServiceLocator.BrowserCommandExecutorFactory = _commandExecutorFactory.Object;
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
        }

        [TestMethod]
        public void WhenSubmit_ShouldSendSubmitCommand()
        {
            //arrange
            var html = @"
<html>
    <body>
        <form id='form1'>
            <input type='hidden' id='input1' value='thevalue' />
        </form>
    </body>
</html>";
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);

            var testPage = new HtmlPage();
            var document = HtmlElement.Create(html, testPage, false);
            
            //act
            var form = (HtmlFormElement)document.ChildElements.Find("form1");      
            form.Submit();

            //assert
            MSAssert.AreEqual("FormSubmit", commandExecutor.ExecutedCommands[0].Handler.ClientFunctionName);
            MSAssert.AreEqual("FormSubmit", commandExecutor.ExecutedCommands[0].Description);
            MSAssert.IsTrue(commandExecutor.ExecutedCommands[0].Handler.RequiresElementFound);
            MSAssert.AreEqual("form1", commandExecutor.ExecutedCommands[0].Target.Id);
            MSAssert.IsNull(commandExecutor.ExecutedCommands[0].Handler.Arguments);
        }

        [TestMethod]
        public void BuildPostData_IfNoInputElementsExistReturnsEmpty()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.IsTrue(postData.Count == 0);
            MSAssert.IsTrue(string.IsNullOrEmpty(postData.GetPostDataString()));
        }

        [TestMethod]
        public void BuildPostData_ReturnsNameValuePairsForInputElements()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='text' name='input2' value='value2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&input2=value2", postData.GetPostDataString());
            MSAssert.AreEqual(2, postData.Count);
        }

        [TestMethod]
        public void BuildPostData_UrlEncodesValueOfInputElements()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='val&ue1' />
                            <input type='text' name='input2' value='val>ue2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=val%26ue1&input2=val%3eue2", postData.GetPostDataString());
            MSAssert.AreEqual(2, postData.Count);
        }

        [TestMethod]
        public void IfFormContainsUnknownElements_BuildPostData_ShouldContainOnlyKnownElements()
        {
            //Arrange
            string html = @"
                <html>
                        <body>
                            <form id='form1' action='action' method='put'>
                                <input type='text' name='textbox1' value='textvalue' />
                                <input type='password' name='password1' value='passvalue' />
                                <input type='checkbox' name='checkbox1' value='checkvalue' />
                                <input type='radio' name='radio1' value='radiovalue' />
                                <input type='reset' name='reset1' value='resetvalue' />
                                <input type='file' name='file1' value='filevalue' />
                                <input type='hidden' name='hidden1' value='hiddenvalue' />
                                <input type='submit' name='button1' value='button1' />
                                <input type='search' name='search1' value='search1' />
                                <input type='tel' name='tel1' value='tel1' />
                                <input type='url' name='url1' value='url1' />
                                <input type='email' name='email1' value='email1' />
                                <input type='datetime' name='datetime1' value='datetime1' />
                                <input type='date' name='date1' value='10/10/1981' />
                                <input type='month' name='month1' value='month1' />
                                <input type='week' name='week1' value='week1' />
                                <input type='time' name='time1' value='time1' />
                                <input type='number' name='number1' value='11' />
                            </form>
                        </body>
                    </html>";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("textbox1=textvalue&password1=passvalue&file1=filevalue&hidden1=hiddenvalue&button1=button1&search1=search1&tel1=tel1&url1=url1&email1=email1&datetime1=datetime1&date1=10%2f10%2f1981&month1=month1&week1=week1&time1=time1&number1=11", postData.GetPostDataString());
            MSAssert.AreEqual(15, postData.Count);
        }

        [TestMethod]
        public void IfFormContainsDifferentInputElements_PostDataCollection_ShouldBeAbleToFilter()
        {
            //Arrange
            string html = @"
                <html>
                        <body>
                            <form id='form1' action='action' method='put'>
                                <input type='text' name='textbox1' value='textvalue' />
                                <input type='password' name='password1' value='passvalue' />
                                <input type='checkbox' name='checkbox1' value='checkvalue' />
                                <input type='radio' name='radio1' value='radiovalue' />
                                <input type='reset' name='reset1' value='resetvalue' />
                                <input type='file' name='file1' value='filevalue' />
                                <input type='file' name='file2' value='filevalue2' />
                            </form>
                        </body>
                    </html>";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("textbox1=textvalue&password1=passvalue", postData.GetPostDataString(PostDataFieldType.Text));
            MSAssert.AreEqual(4, postData.Count);
            MSAssert.AreEqual(2, postData.FindAll(e => (e.Type == PostDataFieldType.File)).Count());
        }

        [TestMethod]
        public void WhenGetPostData_IfCheckboxPresentAndChecked_ShouldBeIncludItInPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='checkbox' name='check1' checked />
                            <input type='text' name='input2' value='value2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&check1=on&input2=value2", postData.GetPostDataString());
            MSAssert.AreEqual(3, postData.Count);
        }

        [TestMethod]
        public void WhenGetPostData_IfCheckboxPresentAndUnchecked_ShouldNotIncludItInPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='checkbox' name='check1' />
                            <input type='text' name='input2' value='value2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&input2=value2", postData.GetPostDataString());
            MSAssert.AreEqual(2, postData.Count);
        }

        [TestMethod]
        public void WhenGetPostData_IfElementIsDisabled_ShouldNotContainCheckboxInPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='checkbox' name='check1' checked disabled />
                            <input type='text' name='input2' value='value2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&input2=value2", postData.GetPostDataString());
            MSAssert.AreEqual(2, postData.Count);
        }

        [TestMethod]
        public void WhenGetPostData_IfMultipleCheckboxesWithSameNamePresentAndChecked_ShouldBeIncludItInPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='checkbox' name='check1' checked />
                            <input type='text' name='input2' value='value2' />                            
                            <input type='checkbox' name='check1' checked />
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&check1=on&input2=value2&check1=on", postData.GetPostDataString());
            MSAssert.AreEqual(4, postData.Count);
        }

        [TestMethod]
        public void WhenGetPostData_IfMultipleCheckboxesWithSameNamePresentAndCheckedWithValue_ShouldValueBeIncludItInPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='checkbox' name='check1' value='checkvalue1' checked />
                            <input type='text' name='input2' value='value2' />                            
                            <input type='checkbox' name='check1' value='checkvalue2' checked />
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&check1=checkvalue1&input2=value2&check1=checkvalue2", postData.GetPostDataString());
            MSAssert.AreEqual(4, postData.Count);
        }

        [TestMethod]
        public void WhenGetPostData_IfRadioPresentAndChecked_ShouldBeIncludItInPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='radio' name='check1' checked />
                            <input type='text' name='input2' value='value2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&check1=on&input2=value2", postData.GetPostDataString());
            MSAssert.AreEqual(3, postData.Count);
        }

        [TestMethod]
        public void WhenGetPostData_IfRadioPresentAndUnchecked_ShouldNotIncludeItInPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='radio' name='check1' />
                            <input type='text' name='input2' value='value2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&input2=value2", postData.GetPostDataString());
            MSAssert.AreEqual(2, postData.Count);
        }

        [TestMethod]
        public void WhenGetPostData_IfRadioPresentAndChecked_ShouldValueBeIncludItInPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='radio' name='check1' checked='checked' value='radiovalue' />
                            <input type='text' name='input2' value='value2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&check1=radiovalue&input2=value2", postData.GetPostDataString());
            MSAssert.AreEqual(3, postData.Count);
        }

        [TestMethod]
        public void WhenGetPostData_IfMultipleSubmitButtons_ShouldOnlyReturnTheOneThatInitiatedThePost()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='submit' name='submit1' value='submit1' />
                            <input type='submit' name='submit2' value='submit2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            var inputElement = (HtmlInputElement)form.ChildElements.Find("submit2");
            PostDataCollection postData = form.GetPostDataCollection(inputElement);

            // Assert
            MSAssert.AreEqual("input1=value1&submit2=submit2", postData.GetPostDataString());
        }

        [TestMethod]
        public void WhenGetPostData_IfMultipleSubmitButtonsWithSameName_ShouldOnlyReturnTheOneThatInitiatedThePost()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='submit' name='submitname' value='submit1' />
                            <input type='submit' name='submitname' value='submit2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            var inputElement = (HtmlInputElement)form.ChildElements.Find(new { value = "submit2" });
            PostDataCollection postData = form.GetPostDataCollection(inputElement);

            // Assert
            MSAssert.AreEqual("input1=value1&submitname=submit2", postData.GetPostDataString());
        }

        [TestMethod]
        public void WhenGetPostData_IfMultipleSubmitButtonsAndNoSubmitId_ShouldReturnBothSubmitButtonData()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <input type='text' name='input1' value='value1' />
                            <input type='submit' name='submit1' value='submit1' />
                            <input type='submit' name='submit2' value='submit2' />                            
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&submit1=submit1&submit2=submit2", postData.GetPostDataString());
        }

        [TestMethod]
        public void WhenGetPostData_IfSelectHasNoOptions_ShouldNotAddItToPostada()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <select name='select1' />
                            <input type='text' name='input1' value='value1' />
                            <input type='submit' name='submit1' value='submit1' />
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&submit1=submit1", postData.GetPostDataString());
        }

        [TestMethod]
        public void WhenGetPostData_IfSelectHasNoSelectedOptions_ShouldNotAddItToPostada()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <select name='select1'>
                                <option value='option1value' />
                            </select>
                            <input type='text' name='input1' value='value1' />
                            <input type='submit' name='submit1' value='submit1' />
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&submit1=submit1", postData.GetPostDataString());
        }

        [TestMethod]
        public void WhenGetPostData_IfSelectHasSelectedOption_ShouldAddOptionsValueToPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <select name='select1'>
                                <option value='option1value' selected>option1text</option>
                            </select>
                            <input type='text' name='input1' value='value1' />
                            <input type='submit' name='submit1' value='submit1' />
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&submit1=submit1&select1=option1value", postData.GetPostDataString());
        }

        [TestMethod]
        public void WhenGetPostData_IfSelectHasSelectedOptionWithoutValue_ShouldAddOptionsTextToPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <select name='select1'>
                                <option selected>option1text</option>
                            </select>
                            <input type='text' name='input1' value='value1' />
                            <input type='submit' name='submit1' value='submit1' />
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&submit1=submit1&select1=option1text", postData.GetPostDataString());
        }

        [TestMethod]
        public void WhenGetPostData_IfSelectHasSelectedOptionWithNonStdSymbols_ShouldEncodeValue()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <select name='select1'>
                                <option selected>option1<text</option>
                            </select>
                            <input type='text' name='input1' value='value1' />
                            <input type='submit' name='submit1' value='submit1' />
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&submit1=submit1&select1=option1%3ctext", postData.GetPostDataString());
        }

        [TestMethod]
        public void WhenGetPostData_IfSelectHasMultipleSelectedOption_ShouldAddAllOptionsValuesToPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <select name='select1'>
                                <option value='option1value' selected>option1text</option>
                                <option value='option2value'>option2text</option>
                                <option value='option3value' selected>option3text</option>
                            </select>
                            <input type='text' name='input1' value='value1' />
                            <input type='submit' name='submit1' value='submit1' />
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&submit1=submit1&select1=option1value&select1=option3value", postData.GetPostDataString());
        }

        [TestMethod]
        public void WhenGetPostData_IfSelectHasMultipleSelectedOptionDifferentSelected_ShouldAddAllOptionsValuesToPostdata()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <select name='select1'>
                                <option value='option1value' selected>option1text</option>
                                <option value='option2value' selected='true'>option2text</option>
                                <option value='option3value' selected='selected'>option3text</option>
                            </select>
                            <input type='text' name='input1' value='value1' />
                            <input type='submit' name='submit1' value='submit1' />
                        </div>
                    </form>
                </html>
                ";

            HtmlFormElement form = (HtmlFormElement)HtmlElement.Create(html).ChildElements.Find("form1");

            //Act
            PostDataCollection postData = form.GetPostDataCollection();

            // Assert
            MSAssert.AreEqual("input1=value1&submit1=submit1&select1=option1value&select1=option2value&select1=option3value", postData.GetPostDataString());
        }
    }
}

