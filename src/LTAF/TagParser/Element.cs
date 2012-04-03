//------------------------------------------------------------------------------
// <copyright file="Element.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser
{
    // <ELEMENT>
    internal class Element : NodeContainer
    {
        private bool _isInlineClosed;
        private int _endPositionOpeningTag;
        private int _startPositionClosingTag;
        private bool _isClosed = true;

        internal Element() {
        }

        public int EndPositionOpeningTag {
            get {
                return _endPositionOpeningTag;
            }
        }

        public bool IsClosed {
            get {
                return _isClosed;
            }
        }

        public bool IsInlineClosed {
            get {
                return _isInlineClosed;
            }
        }

        public int StartPositionClosingTag {
            get {
                return _startPositionClosingTag;
            }
        }

        public override NodeType Type {
            get {
                return NodeType.Element;
            }
        }

        public string GetCloseTag() {
            if (this.IsInlineClosed || !this.IsClosed) {
                return "";
            }
            int length = this.EndPosition - this.StartPositionClosingTag;
            if (length <= 0) {
                return "";
            }
            return base.Document.Text.Substring(this.StartPositionClosingTag, length + 1);
        }

        public string GetOpenTag() {
            if (this.IsInlineClosed) {
                return this.GetText();
            }
            int length = this.EndPositionOpeningTag - this.StartPosition;
            if (length <= 0) {
                return "";
            }
            return base.Document.Text.Substring(this.StartPosition, length + 1);
        }

        internal void SetElementPosition(int startPosition, int endPositionOpeningTag, int startPositionClosingTag, int endPosition) {
            this.SetPosition(startPosition, endPosition);
            this._endPositionOpeningTag = endPositionOpeningTag;
            this._startPositionClosingTag = startPositionClosingTag;
        }

        internal void SetIsClosed(bool value) {
            this._isClosed = value;
        }

        internal void SetIsInlineClosed(bool value) {
            this._isInlineClosed = value;
        }

    }

}
