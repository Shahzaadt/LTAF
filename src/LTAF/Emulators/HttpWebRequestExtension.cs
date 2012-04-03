using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using LTAF.Engine;

namespace LTAF.Emulators
{
    /// <summary>
    /// Contains xtension methods that helps BrowserEmulator to deal with Request data streams and cookies
    /// </summary>
    internal static class HttpWebRequestExtension
    {
        // boundary constants for multipart request format
        private static string _boundary = "----------------------------8ccae08efb39ed2";
        private static byte[] _boundaryBytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + _boundary + "\r\n");

        /// <summary>
        /// Adds cookie collection to request and sets correct header
        /// </summary>
        public static void AddCookies(this HttpWebRequest request, CookieContainer cookies)
        {
            request.CookieContainer = cookies;

            // Add cookies to request
            string cookieHeader = ConstructCookieHeaderString(cookies.GetCookies(request.RequestUri));

            if (!string.IsNullOrEmpty(cookieHeader))
            {
                request.Headers.Add("Cookie", cookieHeader);
            }
        }

        /// <summary>
        /// Prepares standard post data string (pairs separated with 'And' symbol) and outputs 
        /// postdata collection to request stream.
        /// </summary>
        public static string AddStandardRequestData(this HttpWebRequest request, string postDataString)
        {
            if (!string.IsNullOrEmpty(postDataString))
            {
                byte[] postDataBytes = Encoding.UTF8.GetBytes(postDataString);
                request.ContentLength = postDataBytes.Length;
                
                Stream stream = request.GetRequestStream();
                stream.Write(postDataBytes, 0, postDataBytes.Length);
                stream.Close();
            }

            return postDataString;
        }

        /// <summary>
        /// Prepares multipart request data (postdata and files to upload) and
        /// outputs postdata collection to request stream.
        /// </summary>
        public static string AddMultipartRequestData(this HttpWebRequest request, PostDataCollection postData,
                                                IFileSystem fileSystem)
        {          
            // get ususal postdata and files
            List<PostDataField> textPostData = postData.FindAll(f => f.Type != PostDataFieldType.File);
            List<PostDataField> filePostData = postData.FindAll(f => f.Type == PostDataFieldType.File);

            NameValueCollection filesToUpload = ParseFilePostData(filePostData);

            // set corresponding content type
            request.ContentType = "multipart/form-data; boundary=" + _boundary;

            // stream where we going to combine postdata and files to upload
            Stream memoryStream = new MemoryStream();

            byte[] result = null;

            try
            {
                // add postdata
                foreach (PostDataField field in textPostData)
                {
                    WriteFieldToMemoryStream(memoryStream, field.Name, field.Value);
                }

                // write boundary after every form element's value
                memoryStream.Write(_boundaryBytes, 0, _boundaryBytes.Length);

                // add files
                foreach (string key in filesToUpload)
                {
                    WriteFileToMemoryStream(memoryStream, key, filesToUpload[key], fileSystem);
                }

                // now read everything from memory stream to buffer
                memoryStream.Position = 0;
                result = new byte[memoryStream.Length];
                memoryStream.Read(result, 0, result.Length);

                // set correct content length
                request.ContentLength = result.Length;

                // write buffer to request stream
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(result, 0, result.Length);
                requestStream.Close();
            }
            finally
            {
                memoryStream.Close();
            }

            return Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// Parses input file postdata values and returns name/value collection with all 
        /// files to be uploaded
        /// </summary>
        static private NameValueCollection ParseFilePostData(List<PostDataField> filePostData)
        {
            NameValueCollection files = new NameValueCollection();

            foreach(PostDataField field in filePostData)
            {
                // Note: There could be several files associated with the same form elemnt
                // we want to store all off them and add auto index to form element names,
                // that have multiple files
                string[] fieldFiles = HttpUtility.UrlDecode(field.Value).Split(';');
                int count = 0;
                foreach (string file in fieldFiles)
                {
                    files.Add((count == 0) ? field.Name : field.Name + count.ToString(), file);

                    count++;
                }
            }

            return files;
        }

        /// <summary>
        /// Adds specified field and its value to a stream in multipart request format
        /// </summary>
        private static void WriteFieldToMemoryStream(Stream stream, string key, string value)
        {
            // prepare template for every form element value
            string formdataTemplate = "\r\n--" + _boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            string formElement = string.Format(formdataTemplate, key, value);
            byte[] formElementBytes = System.Text.Encoding.UTF8.GetBytes(formElement);
            stream.Write(formElementBytes, 0, formElementBytes.Length);
        }

        /// <summary>
        /// Adds specified file name and its contents to a stream in multipart request format
        /// </summary>
        private static void WriteFileToMemoryStream(Stream stream, string fieldName, string fileName, IFileSystem fileSystem)
        {
            Stream fileStream = null;

            try
            {
                // prepare header for every uploaded file
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";

                // add file name to postdata for current form data block
                string header = string.Format(headerTemplate, fieldName, fileName);
                byte[] headerBytes = System.Text.Encoding.UTF8.GetBytes(header);

                stream.Write(headerBytes, 0, headerBytes.Length);

                // Read file and write it to memory stream
                // Note: we don't verify if file exist or not. It's for testing purpose and we expect 
                // that files are specified, if not it is a test bug.
                fileStream = fileSystem.OpenFileStream(fileName, FileMode.Open, FileAccess.Read);

                byte[] buffer = new byte[1024];
                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }

                // write boundary
                stream.Write(_boundaryBytes, 0, _boundaryBytes.Length);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        /// <summary>
        /// Prints all given cookies to correctly formatted header string
        /// </summary>
        private static string ConstructCookieHeaderString(CookieCollection cookies)
        {
            if (cookies == null)
            {
                return "";
            }

            // construct string with all cookies
            StringBuilder header = new StringBuilder();

            foreach (Cookie cookie in cookies)
            {
                if (!cookie.Expired)
                {
                    header.Append(cookie.Name + "=" + cookie.Value);

                    if (cookie.Expires != DateTime.MinValue)
                    {
                        header.Append("; expires=" + cookie.Expires.ToString());
                    }

                    if (!string.IsNullOrEmpty(cookie.Path))
                    {
                        header.Append("; path=" + cookie.Path);
                    }

                    if (!string.IsNullOrEmpty(cookie.Domain))
                    {
                        header.Append("; domain=" + cookie.Domain);
                    }

                    if (cookie.HttpOnly)
                    {
                        header.Append("; httponly");
                    }

                    if (cookie.Secure)
                    {
                        header.Append("; secure");
                    }

                    header.Append(", ");
                }
            }

            return header.ToString().Trim().Trim(',');
        }
    }
}
