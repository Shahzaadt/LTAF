using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Post data fields type
    /// </summary>
    public enum PostDataFieldType
    {
        /// <summary>Text</summary>
        Text,
        /// <summary>File</summary>
        File
    }

    /// <summary>
    /// Represents postdata (input) field, that is passed with request
    /// </summary>
    public class PostDataField
    {
        /// <summary>Input field name </summary>
        public string Name { get; set; }

        /// <summary>Input field value </summary>
        public string Value { get; set; }

        /// <summary>Input field type </summary>
        public PostDataFieldType Type { get; set; }
    }

    /// <summary>
    /// Collection of postdata fields
    /// </summary>
    public class PostDataCollection : List<PostDataField>
    {
        /// <summary>
        /// Combines all fields to standard postdata string for request: name=value&amp;name1=value1
        /// </summary>
        /// <returns>Postdata string </returns>
        public string GetPostDataString()
        {
            return GetPostDataString(null);
        }

        /// <summary>
        /// Combines all fields that satisfy filter to standard postdata 
        /// string for request: name=value&amp;name1=value1
        /// </summary>
        /// <param name="filterBy">Specifies what types of fields to return</param>
        /// <returns>Postdata string</returns>
        public string GetPostDataString(PostDataFieldType? filterBy)
        {
            List<PostDataField> filteredFields = GetFieldsForType(filterBy);

            StringBuilder post = new StringBuilder();

            if (filteredFields != null)
            {
                foreach (PostDataField field in filteredFields)
                {
                    post.Append(String.Format("{0}={1}&", field.Name, field.Value));
                }

                if (post.Length > 0)
                {
                    post.Length--;
                }
            }

            return post.ToString();
        }

        /// <summary>
        /// Specifies if Postdata collection has files or not
        /// </summary>
        public bool HasFilesToUpload
        {
            get
            {
                List<PostDataField> filteredFields = GetFieldsForType(PostDataFieldType.File);

                return (filteredFields.Count != 0);
            }
        }

        private List<PostDataField> GetFieldsForType(PostDataFieldType? filterBy)
        {
            return (filterBy == null)
                        ? this
                        : this.FindAll(f => f.Type == filterBy);
        }


    }
}
