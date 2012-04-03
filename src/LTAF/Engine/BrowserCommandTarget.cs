using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.Engine
{
    /// <summary>
    /// Class that holds information to locate the the target DOM element 
    /// upon which the command will be executed
    /// </summary>
    /// <change date="02/06/2006">Created</change>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public class BrowserCommandTarget
    {
        private string _windowCaption;
        private int _windowIndex;
        private object[] _frames;
        private string _id;
        private string _tagName;
        private int _index;
        private string _childTagName;
        private int _childTagIndex;
        private string _textBetweenTags;
        

        /// <summary>
        /// Public empty constructor for serialization
        /// </summary>
        public BrowserCommandTarget()
        {
        }

        /// <summary>
        /// The text between tags of the element to find
        /// </summary>
        public string TextBetweenTags
        {
            get
            {
                return _textBetweenTags;
            }
            set
            {
                _textBetweenTags = value;
            }
        }

        /// <summary>
        /// The caption of the window where the command is to be executed
        /// (eiher the title of the window or its url)
        /// </summary>
        public string WindowCaption
        {
            get
            {
                return _windowCaption;
            }
            set
            {
                _windowCaption = value;
            }
        }

        /// <summary>
        /// The zero based index of the window where the command is to be executed.
        /// The main browser is always 0, subsequent pop-up windows are incrementally indexed.
        /// </summary>
        public int WindowIndex
        {
            get
            {
                return _windowIndex;
            }
            set
            {
                _windowIndex = value;
            }
        }

        /// <summary>
        /// The hierarchy of frame names/index to reach the target frame inside the window
        /// where the command is to be executed
        /// </summary>
        public object[] FrameHierarchy
        {
            get
            {
                return _frames;
            }
            set
            {
                _frames = value;
            }
        }

        /// <summary>
        /// The ID or NAME attribute of the DOM element inside the target frame
        /// </summary>
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// The tag name of the DOM element inside the target frame
        /// </summary>
        public string TagName
        {
            get
            {
                return _tagName;
            }
            set
            {
                _tagName = value;
            }
        }

        /// <summary>
        /// The index when more than one objects are identified in the DOM
        /// </summary>
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        /// <summary>
        /// ChildTagName
        /// </summary>
        public string ChildTagName
        {
            get { return _childTagName; }
            set { _childTagName = value; }
        }

        /// <summary>
        /// ChildTagIndex
        /// </summary>
        public int ChildTagIndex
        {
            get { return _childTagIndex; }
            set { _childTagIndex = value; }
        }
    }
}
