using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using LTAF.Engine;

namespace LTAF
{
    /// <summary>
    /// Class that represents a collection of Html elements.
    /// </summary>
    public class HtmlElementCollection : ReadOnlyCollection<HtmlElement>
    {
        private HtmlPage _testPage;
        private HtmlElement _parentElement;
        private static int _timeoutWaitingToFind = 0;

        /// <summary>Event that is triggered when this collection is refreshed</summary>
        public event EventHandler<EventArgs> Refreshed;

        internal HtmlElementCollection(HtmlPage parentPage)
            : this(new List<HtmlElement>(), parentPage, null)
        {
        }

        internal HtmlElementCollection(IList<HtmlElement> elementCollection, HtmlPage parentPage, HtmlElement parentElement) :
            base(elementCollection)
        {
            _testPage = parentPage;
            _parentElement = parentElement;
        }

        /// <summary>
        /// Raises the Refreshed event.
        /// </summary>
        protected void OnRefresh(EventArgs e)
        {
            if (Refreshed != null)
            {
                Refreshed(this, e);
            }
        }


        #region Properties
        /// <summary>
        /// Allows set timeout for Find operations
        /// </summary>
        public static int FindElementTimeout
        {
            get
            {
                return _timeoutWaitingToFind;
            }
            set
            {
                _timeoutWaitingToFind = value;
            }
        }


        /// <summary>
        /// The web page that contains this collection of elements.
        /// </summary>
        public HtmlPage TestPage
        {
            get { return _testPage; }
        }

        /// <summary>
        /// The element that contains this collection of elements.
        /// </summary>
        public HtmlElement ParentElement
        {
            get { return _parentElement; }
        }
        #endregion

        #region Refresh
        /// <summary>
        /// Refresh this collection of html elements from the current Dom in the web page.
        /// </summary>
        public void Refresh()
        {
            Refresh(null);
        }

        /// <summary>
        /// Refresh this collection of html elements from the current Dom in the web page including the specified list of attributes
        /// </summary>
        /// <param name="attributesToLoad">Collection of attribute names to load for each element</param>
        internal void Refresh(ICollection<string> attributesToLoad)
        {
            if (TestPage == null)
            {
                return;
            }

            //send the command to get dom of this collection
            BrowserCommand command = new BrowserCommand();
            if (_parentElement == null)
            {
                command.Description = "GetPageDom";
                command.Handler.ClientFunctionName = BrowserCommand.FunctionNames.GetPageDom;
                command.Handler.RequiresElementFound = false;
            }
            else
            {
                command.Description = "GetElementDom";
                command.Handler.ClientFunctionName = BrowserCommand.FunctionNames.GetElementDom;
                command.Handler.RequiresElementFound = true;
                command.Target = this.ParentElement.BuildBrowserCommandTarget();
            }

            if (attributesToLoad != null && attributesToLoad.Count > 0)
            {
                StringBuilder attributesBuilder = new StringBuilder();
                foreach (string attr in attributesToLoad)
                {
                    attributesBuilder.Append(attr);
                    attributesBuilder.Append("-");
                }
                attributesBuilder.Length--;
                command.Handler.SetArguments(attributesBuilder.ToString().ToLowerInvariant());
            }


            BrowserInfo browserInfo = this.TestPage.ExecuteCommand(_parentElement, command);

            HtmlElement newRoot = HtmlElement.Create(browserInfo.Data, this.TestPage, false);
            this.Items.Clear();

            if (_parentElement == null)
            {
                this.Items.Add(newRoot);
            }
            else
            {
                foreach (HtmlElement childElement in newRoot.ChildElements)
                {
                    childElement.ParentElement = _parentElement;
                    this.Items.Add(childElement);
                }
            }

            OnRefresh(new EventArgs());
        }
        #endregion

        #region FindAllInternal
        private void FindAllInternal(HtmlElementFindParams findParams, ICollection<HtmlElement> collection, bool stopAfterFirstFind, bool canUseTagIndexToLocate)
        {
            if (stopAfterFirstFind && collection.Count > 0)
            {
                return;
            }

            foreach (HtmlElement element in this.Items)
            {
                if (findParams.DoesElementMatch(element))
                {
                    if (findParams.IndexInternal == 0)
                    {
                        element.CanUseTagIndexToLocate = canUseTagIndexToLocate;
                        collection.Add(element);
                        if (stopAfterFirstFind)
                        {
                            return;
                        }
                    }
                    else
                    {
                        findParams.IndexInternal--;
                    }

                }

                element.ChildElements.FindAllInternal(findParams, collection, stopAfterFirstFind, canUseTagIndexToLocate);
            }
        }
        #endregion

        #region FindAll
        private IList<HtmlElement> FindAll(HtmlElementFindParams findParams, int timeoutInSeconds, bool stopAfterFirstFind)
        {
            List<HtmlElement> elements = new List<HtmlElement>();
            bool canUseTagIndexToLocate = (this.ParentElement == null ? true : false);
            DateTime timeout = DateTime.Now.AddSeconds(timeoutInSeconds);
            int attempts = 0;

            /*
             Design:
             *  1) First try to locate element in in-memory collection.
             *  2) If that fails, refresh the collection from the browser and try again (regardless of timeout)
             *  3) If that fails, keep refreshing and searching until timeout is met
             */

            do
            {
                if (this._testPage != null && _timeoutWaitingToFind > 0 &&
                    (attempts > 0 || findParams.Attributes.Count > 0))
                {
                    // after first attempt (or if we want to find by Attributes)
                    //refresh the collection from client before trying again.
                    Thread.Sleep(100);
                    this.Refresh(findParams.Attributes.ConvertAll<string>(
                        delegate(HtmlAttributeFindParams attrMatch) { return attrMatch.Name; })
                        );
                }

                findParams.IndexInternal = findParams.Index;
                this.FindAllInternal(findParams, elements, stopAfterFirstFind, canUseTagIndexToLocate);
                attempts++;

            } while (elements.Count == 0 && (attempts < 2 || DateTime.Now < timeout) && timeoutInSeconds != 0);

            return elements;
        }
        #endregion

        #region Find Public Methods
        /// <summary>
        /// Recursively finds the first element whose id or name ends with the given string.
        /// </summary>
        /// <param name="idEndsWith">The string that appears at the end of the element's id or name to find (case insensitive).</param>
        /// <returns>The first HtmlElement whose id or name ends with the string provided</returns>
        /// <exception cref="ElementNotFoundException">Thrown if element not found</exception>
        public HtmlElement Find(string idEndsWith)
        {
            return this.Find(new HtmlElementFindParams(idEndsWith));
        }

        /// <summary>
        /// Recursively finds the first element whose id or name matches the given string.
        /// </summary>
        /// <param name="id">The string to use when matching the element's id or name to find (case insensitive).</param>
        /// <param name="idMatchMethod">How to treat the string when matching it with the HtmlElement's id or name</param>
        /// <returns>The first HtmlElement whose id or name matches the given string</returns>
        /// <exception cref="ElementNotFoundException">Thrown if element not found</exception>
        public HtmlElement Find(string id, MatchMethod idMatchMethod)
        {
            return this.Find(new HtmlElementFindParams(id, idMatchMethod));
        }

        /// <summary>
        /// Recursively find the element with the determined tagName
        /// </summary>
        /// <param name="tagName">Tag name of the element to find (case insensitive).</param>
        /// <param name="index">The zero-based index of the element to find.</param>
        /// <returns>The HtmlElement in this collection that has the given tag name at the given position from the start of the Dom.</returns>
        /// <exception cref="ElementNotFoundException">Thrown if element not found</exception>
        public HtmlElement Find(string tagName, int index)
        {
            return this.Find(new HtmlElementFindParams(tagName, index));
        }

        /// <summary>
        /// Recursively find the element with the determined tagName and innerText
        /// </summary>
        /// <param name="tagName">Tag name of the element to find (case insensitive).</param>
        /// <param name="innerText">The innerText of the element to find (case insensitive).</param>
        /// <param name="index">The zero-based index of the element to find.</param>
        /// <returns>The HtmlElement in this collection that has the given tag name and inner text at the given position from the start of the Dom.</returns>
        /// <exception cref="ElementNotFoundException">Thrown if element not found</exception>
        public HtmlElement Find(string tagName, string innerText, int index)
        {
            return this.Find(new HtmlElementFindParams(tagName, innerText, index));
        }

        /// <summary>
        /// Recursively find the first element that satisfies the given parameters
        /// </summary>
        /// <param name="findParams">HtmlElementFindParams object that encapsulates the find parameters</param>
        /// <returns>The first HtmlElement in this collection that satisfies the given find parameters</returns>
        /// <exception cref="ElementNotFoundException">Thrown if element not found</exception>
        public HtmlElement Find(HtmlElementFindParams findParams)
        {
            return Find(findParams, _timeoutWaitingToFind);
        }

        /// <summary>
        /// Recursively find the first element that satisfies the given parameters
        /// </summary>
        /// <param name="findParams">anonymous object that encapsulates the find parameters</param>
        /// <returns>The first HtmlElement in this collection that satisfies the given find parameters</returns>
        /// <exception cref="ElementNotFoundException">Thrown if element not found</exception>
        public HtmlElement Find(object findParams)
        {
            return this.Find(new HtmlElementFindParams(findParams));
        }

        /// <summary>
        /// Recursively find the first element that satisfies the given parameters and keep trying for the specified time
        /// </summary>
        /// <param name="findParams">HtmlElementFindParams object that encapsulates the find parameters</param>
        /// <param name="timeoutInSeconds">Seconds to keep searching the collection if element not found</param>
        /// <returns>The first HtmlElement in this collection that satisfies the given find parameters</returns>
        /// <exception cref="ElementNotFoundException">Thrown if element not found</exception>
        public HtmlElement Find(HtmlElementFindParams findParams, int timeoutInSeconds)
        {
            IList<HtmlElement> list = FindAll(findParams, timeoutInSeconds, true);

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                throw BuildElementNotFoundException(findParams, timeoutInSeconds);
            }
        }
        #endregion

        #region FindAll Public Methods
        /// <summary>
        /// Recursively finds all the elements whose id or name matches the given string.
        /// </summary>
        /// <param name="id">The string to use when matching the element's id or name to find (case insensitive).</param>
        /// <param name="idMatchMethod">How to treat the string when matching it with the HtmlElement's id or name</param>
        /// <returns>A collection of  HtmlElements whose ids or names match the given string</returns>
        public ReadOnlyCollection<HtmlElement> FindAll(string id, MatchMethod idMatchMethod)
        {
            return this.FindAll(new HtmlElementFindParams(id, idMatchMethod));
        }

        /// <summary>
        /// Recursively finds all the elements with the given tagName
        /// </summary>
        /// <param name="tagName">Tag name of the element to find (case insensitive).</param>
        /// <returns>A collection of HtmlElements that have the given tag name.</returns>
        public ReadOnlyCollection<HtmlElement> FindAll(string tagName)
        {
            return this.FindAll(new HtmlElementFindParams(tagName, 0));
        }

        /// <summary>
        /// Recursively finds all the elements with the determined tagName and innerText
        /// </summary>
        /// <param name="tagName">Tag name of the element to find (case insensitive).</param>
        /// <param name="innerText">The innerText of the element to find (case insensitive).</param>
        /// <returns>A collection of HtmlElements that have the given tag name and inner text.</returns>
        public ReadOnlyCollection<HtmlElement> FindAll(string tagName, string innerText)
        {
            HtmlElementFindParams findParams = new HtmlElementFindParams();
            findParams.TagName = tagName;
            findParams.InnerText = innerText;
            return this.FindAll(findParams);
        }

        /// <summary>
        /// Recursively finds all the elements that satisfy the given parameters
        /// </summary>
        /// <param name="findParams">HtmlElementFindParams object that encapsulates the find parameters</param>
        /// <returns>A collection of HtmlElements that satisfy the given find parameters</returns>
        public ReadOnlyCollection<HtmlElement> FindAll(HtmlElementFindParams findParams)
        {
            return FindAll(findParams, _timeoutWaitingToFind);
        }

        /// <summary>
        /// Recursively finds all the elements that satisfy the given parameters
        /// </summary>
        /// <param name="findParams">anonymous object that encapsulates the find parameters</param>
        /// <returns>A collection of HtmlElements that satisfy the given find parameters</returns>
        public ReadOnlyCollection<HtmlElement> FindAll(object findParams)
        {
            return this.FindAll(new HtmlElementFindParams(findParams));
        }

        /// <summary>
        /// Recursively finds all the elements that satisfy the given parameters and keep trying for the specified time
        /// </summary>
        /// <param name="findParams">HtmlElementFindParams object that encapsulates the find parameters</param>
        /// <param name="timeoutInSeconds">Seconds to keep searching the collection if element not found</param>
        /// <returns>A collection of HtmlElements that satisfy the given find parameters</returns>
        public ReadOnlyCollection<HtmlElement> FindAll(HtmlElementFindParams findParams, int timeoutInSeconds)
        {
            IList<HtmlElement> list = FindAll(findParams, timeoutInSeconds, false);
            return new ReadOnlyCollection<HtmlElement>(list);
        }
        #endregion

        /// <summary>
        /// Returns the first occurence of an element that matches the findParams and that occurs after
        /// the specified element.
        /// </summary>
        /// <param name="precedingElement">The element that must occur before the element that we are looking for</param>
        /// <param name="idEndsWith">The id that the element we are looking for ends with.</param>
        /// <returns>The element that is found.  An exception is thrown if an element is not found</returns>
        public HtmlElement FindAfter(HtmlElement precedingElement, string idEndsWith)
        {
            return FindAfter(precedingElement, new HtmlElementFindParams(idEndsWith));
        }

        /// <summary>
        /// Returns the first occurence of an element that matches the findParams and that occurs after
        /// the specified element.
        /// </summary>
        /// <param name="precedingElement">The element that must come before the what we are looking for</param>
        /// <param name="findParams">The find params for the element we are looking for</param>
        /// <returns>The element that is found.  An exception is thrown if an element is not found</returns>
        public HtmlElement FindAfter(HtmlElement precedingElement, HtmlElementFindParams findParams)
        {
            if (IsElementUnderThisCollection(precedingElement))
            {
                return FindAfterRecursive(precedingElement, findParams);
            }
            else
            {
                throw new ArgumentException("The 'precedingElement' argument was not itself a member of the collection.", "precedingElement");
            }
        }

        private bool IsElementUnderThisCollection(HtmlElement element)
        {
            HtmlElement ancestor = element;
            while (ancestor != null)
            {
                if (this.IndexOf(ancestor) >= 0)
                {
                    return true;
                }
                ancestor = ancestor.ParentElement;
            }
            return false;
        }

        private HtmlElement FindAfterRecursive(HtmlElement precedingElement, HtmlElementFindParams findParams)
        {
            var parentOfElement = precedingElement.ParentElement;

            // Stop if we are at the root element
            if (parentOfElement == null)
            {
                throw BuildElementNotFoundException(findParams, 0);
            }

            // Stop if we are looking outside of this collection's parent (only possible if the collection belongs
            // to an element, not to the page).
            if (this.ParentElement != null && this.ParentElement.ParentElement == parentOfElement)
            {
                throw BuildElementNotFoundException(findParams, 0);
            }

            int indexOfThisElement = parentOfElement.ChildElements.IndexOf(precedingElement);

            for (int i = indexOfThisElement + 1; i < parentOfElement.ChildElements.Count; i++)
            {
                if (findParams.DoesElementMatch(parentOfElement.ChildElements[i]))
                {
                    return parentOfElement.ChildElements[i];
                }
                else
                {
                    var matchedElements = parentOfElement.ChildElements[i].ChildElements.FindAll(findParams, 0);
                    if (matchedElements.Count > 0)
                    {
                        return matchedElements[0];
                    }
                }
            }

            return this.FindAfterRecursive(parentOfElement, findParams);
        }


        #region Exists Public Methods
        /// <summary>
        /// Returns whether an element whose id or name ends with the given string exists within this collection (recursive)
        /// </summary>
        /// <param name="idEndsWith">The string that appears at the end of the element's id or name to find (case insensitive).</param>
        /// <returns>True if an element is found, otherwise false</returns>
        public bool Exists(string idEndsWith)
        {
            return Exists(new HtmlElementFindParams(idEndsWith));
        }

        /// <summary>
        /// Returns whether an element whose id or name matches the given string exists within this collection (recursive)
        /// </summary>
        /// <param name="id">The string to use when matching the element's id or name to find (case insensitive).</param>
        /// <param name="idMatchMethod">How to treat the string when matching it with the HtmlElement's id or name</param>
        /// <returns>True if an element is found, otherwise false</returns>
        public bool Exists(string id, MatchMethod idMatchMethod)
        {
            return Exists(new HtmlElementFindParams(id, idMatchMethod));
        }

        /// <summary>
        /// Returns whether an element that satisfies the given parameters exists within this collection (recursive)
        /// </summary>
        /// <param name="findParams">HtmlElementFindParams object that encapsulates the find parameters</param>
        /// <returns>True if an element is found, otherwise false</returns>
        public bool Exists(HtmlElementFindParams findParams)
        {
            return Exists(findParams, _timeoutWaitingToFind);
        }

        /// <summary>
        /// Returns whether an element that satisfies the given parameters exists within this collection (recursive)
        /// </summary>
        /// <param name="findParams">HtmlElementFindParams object that encapsulates the find parameters</param>
        /// <param name="timeoutInSeconds">Seconds to keep searching the collection if element not found</param>
        /// <returns>True if an element is found, otherwise false</returns>
        public bool Exists(HtmlElementFindParams findParams, int timeoutInSeconds)
        {
            IList<HtmlElement> list = FindAll(findParams, timeoutInSeconds, true);

            if (list.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region BuildElementNotFoundException
        private ElementNotFoundException BuildElementNotFoundException(HtmlElementFindParams findParams, int timeout)
        {
            string message = @"Control not found in HtmlElementCollection after {0} seconds. HtmlElemntFindParams:
<br />TagName:'{1}'.
<br />Id:'{2}'.
<br />InnerText:'{3}'.
<br />Index:'{4}'. 
If control is taking too long to appear in browser's DOM, use 'Find(HtmlElementFindParams,int)' method overload to wait until control appears.";

            return new ElementNotFoundException(String.Format(message,
                                       timeout,
                                       findParams.TagName,
                                       findParams.IdAttributeFindParams.Value,
                                       findParams.InnerText,
                                       findParams.Index));
        }
        #endregion
    }
}