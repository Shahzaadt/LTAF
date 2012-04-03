using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace LTAF
{
    /// <summary>
    /// Class that represents the 'table' html element.
    /// </summary>
    public class HtmlTableElement: HtmlElement
    {
        private ReadOnlyCollection<HtmlTableRowElement> _rows = null;

        internal HtmlTableElement(
            IDictionary<string, string> attributes,
            HtmlElement parentElement,
            HtmlPage parentPage): base("table", attributes, parentElement, parentPage)
        {
            
        }

        internal override void SetChildElements(IList<HtmlElement> childElements)
        {
            base.SetChildElements(childElements);
            AttachEvents();
        }

        private void AttachEvents()
        {
            // This event will set _rows to null when table is refreshed
            // Also, it will add an event to tbody so that when tbody is refreshed _rows will be set to null as well
            this.ChildElements.Refreshed += new EventHandler<EventArgs>(RefreshTableChildElements);            
            AttachEventToTBodyToInvalidateRowsCollection();
        }

        private void InvalidateRowsCollectionOnRefresh(object sender, EventArgs e)
        {
            _rows = null;
        }

        private void RefreshTableChildElements(object sender, EventArgs e)
        {
            _rows = null;
            AttachEventToTBodyToInvalidateRowsCollection();
        }

        private void AttachEventToTBodyToInvalidateRowsCollection()
        {
            if (TBody != null)
            {
                this.TBody.ChildElements.Refreshed += new EventHandler<EventArgs>(InvalidateRowsCollectionOnRefresh);
            }
        }

        /// <summary>
        /// The TBody element of this table, null if none exists.
        /// </summary>
        public HtmlElement TBody
        {
            get
            {
                foreach (HtmlElement e in this.ChildElements)
                {
                    if (e.TagName.Equals("tbody", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return e;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// A readonly collection of HtmlTableRowElement's that exists under this table
        /// </summary>
        public ReadOnlyCollection<HtmlTableRowElement> Rows
        {
            get
            {
                if (_rows == null)
                {
                    List<HtmlTableRowElement> rows = new List<HtmlTableRowElement>();
                    if (this.ChildElements.Count == 0)
                    {
                        _rows = new ReadOnlyCollection<HtmlTableRowElement>(rows);
                    }
                    else
                    {
                        foreach (HtmlElement element in this.TBody.ChildElements)
                        {
                            HtmlTableRowElement row = element as HtmlTableRowElement;
                            if (row != null)
                            {
                                rows.Add(row);
                            }
                        }
                        _rows = new ReadOnlyCollection<HtmlTableRowElement>(rows);
                    }
                }

                return _rows;
            }
            
        }
    }
}