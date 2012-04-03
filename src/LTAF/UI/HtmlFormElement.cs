using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Web;
using LTAF.Engine;

namespace LTAF
{
    /// <summary>
    /// Class that represents the 'form' html element.
    /// </summary>
    public class HtmlFormElement : HtmlElement
    {
        internal HtmlFormElement(
            IDictionary<string, string> attributes,
            HtmlElement parentElement,
            HtmlPage parentPage)
            : base("form", attributes, parentElement, parentPage)
        {
        }

        /// <summary>
        /// Builds a post data string based on its child elements
        /// </summary>
        public PostDataCollection GetPostDataCollection(HtmlInputElement submitInputElement = null)
        {
            PostDataCollection postDataCollection = new PostDataCollection();

            foreach (HtmlInputElement element in this.ChildElements.FindAll("input"))
            {
                if (!string.IsNullOrEmpty(element.Name)
                    && !element.CachedAttributes.Disabled)
                {
                    switch (element.CachedAttributes.Type)
                    {
                        case HtmlInputElementType.Checkbox:
                        case HtmlInputElementType.Radio:
                            if (element.CachedAttributes.Checked)
                            {
                                postDataCollection.Add(GeneratePostDataForBooleanElement(element));
                            }
                            break;
                        case HtmlInputElementType.Submit:
                            if (submitInputElement == null || submitInputElement == element)
                            {
                                postDataCollection.Add(GeneratePostDataForTextElement(element));
                            }
                            break;
                        case HtmlInputElementType.File:
                        case HtmlInputElementType.Hidden:
                        case HtmlInputElementType.Password:
                        case HtmlInputElementType.Text:
                        case HtmlInputElementType.Search:
                        case HtmlInputElementType.Tel:
                        case HtmlInputElementType.Url:
                        case HtmlInputElementType.Email:
                        case HtmlInputElementType.Datetime:
                        case HtmlInputElementType.Date:
                        case HtmlInputElementType.Month:
                        case HtmlInputElementType.Week:
                        case HtmlInputElementType.Time:
                        case HtmlInputElementType.Number:
                            postDataCollection.Add(GeneratePostDataForTextElement(element));
                            break;
                    }
                }
            }

            foreach (HtmlTextAreaElement element in this.ChildElements.FindAll(new HtmlElementFindParams("textarea", 0), 0))
            {
                if (!string.IsNullOrEmpty(element.Name))
                {
                    postDataCollection.Add(new PostDataField()
                    {
                        Name = element.Name,
                        Value = HttpUtility.UrlEncode(element.CachedInnerText),
                        Type = PostDataFieldType.Text
                    });
                }
            }

            foreach (HtmlSelectElement element in this.ChildElements.FindAll(new HtmlElementFindParams("select", 0), 0))
            {
                if (!string.IsNullOrEmpty(element.Name))
                {
                    var options = element.ChildElements.FindAll("option");

                    // add all selected options to postdata
                    if (options != null)
                    {
                        foreach (HtmlElement o in options)
                        {
                            HtmlOptionElement option = o as HtmlOptionElement;

                            if (option.CachedAttributes.Selected)
                            {
                                // first try to pick value attribute
                                string value = option.CachedAttributes.Get<string>("value", null);

                                // if value is not specified use inner text
                                if (string.IsNullOrEmpty(value))
                                {
                                    value = option.CachedInnerText;
                                }

                                postDataCollection.Add(new PostDataField()
                                {
                                    Name = element.Name,
                                    Value = HttpUtility.UrlEncode(value),
                                    Type = PostDataFieldType.Text
                                });
                            }
                        }
                    }
                }
            }

            return postDataCollection;
        }

        /// <summary>
        /// Constructs the post data string for a boolean input
        /// </summary>
        private static PostDataField GeneratePostDataForBooleanElement(HtmlInputElement element)
        {
            return new PostDataField()
            {
                Name = element.Name,
                Value = String.IsNullOrEmpty(element.CachedAttributes.Value) ? "on" : HttpUtility.UrlEncode(element.CachedAttributes.Value),
                Type = PostDataFieldType.Text
            };
        }

        /// <summary>
        /// Constructs the post data string for a text input
        /// </summary>
        private static PostDataField GeneratePostDataForTextElement(HtmlInputElement element)
        {
            return new PostDataField()
            {
                Name = element.Name,
                Value = HttpUtility.UrlEncode(element.CachedAttributes.Value),
                Type = (element.CachedAttributes.Type == HtmlInputElementType.File)
                            ? PostDataFieldType.File : PostDataFieldType.Text
            };

        }

        /// <summary>
        /// Submits the form.
        /// </summary>
        public void Submit()
        {
            var command = new BrowserCommand(BrowserCommand.FunctionNames.FormSubmit);
            command.Target = this.BuildBrowserCommandTarget();
            command.Description = "FormSubmit";
            command.Handler.RequiresElementFound = true;
            this.ParentPage.ExecuteCommand(this, command);
        }
    }
}
