using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Web.UI.WebControls;

namespace LTAF.CompositeControls
{
    public class TestGridView
    {
        private WaitFor _waitForAfterClicks = WaitFor.None;
        private HtmlTableElement _table;
        private PagerPosition _pagerPosition = PagerPosition.Bottom;
        private int? _activeEditRow;

        public TestGridView(HtmlElement tableElement)
            : this(tableElement, true)
        {

        }

        public TestGridView(HtmlElement tableElement, bool waitForPostbackAfterClicks)
        {
            if (waitForPostbackAfterClicks)
            {
                _waitForAfterClicks = WaitFor.Postback;
            }
            if (tableElement is HtmlTableElement)
            {
                _table = (HtmlTableElement)tableElement;
            }
            else
            {
                throw new WebTestException("This control can only wrap an element of type HtmlTableElement.");
            }
        }

        public ReadOnlyCollection<HtmlTableRowElement> Rows
        {
            get
            {
                return _table.Rows;
            }
        }

        public void Refresh()
        {
            _table.ChildElements.Refresh();
        }

        public PagerPosition PagerPosition
        {
            get
            {
                return _pagerPosition;
            }
            set
            {
                _pagerPosition = value;
            }
        }

        #region Sort
        public void Sort(int columnIndex)
        {
            _table.Rows[0].ChildElements[columnIndex].Click(_waitForAfterClicks);
        }

        public void Sort(string columnHeader)
        {
            _table.Rows[0].ChildElements.Find("a", columnHeader, 0).Click(_waitForAfterClicks);
        }
        #endregion

        #region Page
        public void Page(string pageLinkText)
        {
            HtmlElement link = GetPagerRow().Cells.Find("a", pageLinkText, 0);

            if (link != null)
            {
                link.Click(_waitForAfterClicks);
                this.Refresh();
            }
        }

        private HtmlTableRowElement GetPagerRow()
        {
            if (_pagerPosition == PagerPosition.Top || _pagerPosition == PagerPosition.TopAndBottom)
            {
                return _table.Rows[0];
            }
            else
            {
                return _table.Rows[_table.Rows.Count - 1];
            }
        }
        #endregion

        #region ClickEditRow
        public void ClickEditRow(int rowIndex)
        {
            HtmlTableRowElement edit = _table.Rows[rowIndex + 1];
            HtmlElement link = edit.Cells.Find("a", "Edit", 0);

            if (link != null)
            {
                link.Click(_waitForAfterClicks);
                this.Refresh();
                _activeEditRow = rowIndex;
            }
        }
        #endregion

        #region ClickCancel
        public void ClickCancel()
        {
            HtmlTableRowElement edit = _table.Rows[_activeEditRow.Value + 1];
            HtmlElement link = edit.Cells.Find("a", "Cancel", 0);

            if (link != null)
            {
                link.Click(_waitForAfterClicks);
                this.Refresh();
                _activeEditRow = null;
            }
        }
        #endregion
    }
}
