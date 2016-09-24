
using Abot.Poco;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Abot.Core
{
    public interface IWebContentExtractor : IDisposable
    {
        PageContent GetContent(HttpResponseMessage response);
    }

    public class WebContentExtractor : IWebContentExtractor
    {
        static ILogger _logger = LogManager.GetLogger("AbotLogger");

        public virtual PageContent GetContent(HttpResponseMessage response)
        {
            using (MemoryStream memoryStream = GetRawData(response))
            {
                String charset = GetCharsetFromHeaders(response);

                if (charset == null) {
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Do not wrap in closing statement to prevent closing of this stream.
                    StreamReader srr = new StreamReader(memoryStream, Encoding.ASCII);
                    String body = srr.ReadToEnd();
                    charset = GetCharsetFromBody(body);
                }
                memoryStream.Seek(0, SeekOrigin.Begin);

                charset = CleanCharset(charset);
                Encoding e = GetEncoding(charset);
                string content = "";
                using (StreamReader sr = new StreamReader(memoryStream, e))
                {
                    content = sr.ReadToEnd();
                }

                PageContent pageContent = new PageContent();
                pageContent.Bytes = memoryStream.ToArray();
                pageContent.Charset = charset;
                pageContent.Encoding = e;
                pageContent.Text = content;

                return pageContent;
            }
        }

        protected virtual string GetCharsetFromHeaders(HttpResponseMessage webResponse)
        {
            string charset = null;
            String ctype = webResponse.Headers.GetValues("content-type").FirstOrDefault();
            if (ctype != null)
            {
                int ind = ctype.IndexOf("charset=");
                if (ind != -1)
                    charset = ctype.Substring(ind + 8);
            }
            return charset;
        }

        protected virtual string GetCharsetFromBody(string body)
        {
            String charset = null;
            
            if (body != null)
            {
                //find expression from : http://stackoverflow.com/questions/3458217/how-to-use-regular-expression-to-match-the-charset-string-in-html
                Match match = Regex.Match(body, @"<meta(?!\s*(?:name|value)\s*=)(?:[^>]*?content\s*=[\s""']*)?([^>]*?)[\s""';]*charset\s*=[\s""']*([^\s""'/>]*)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    charset = string.IsNullOrWhiteSpace(match.Groups[2].Value) ? null : match.Groups[2].Value;
                }
            }

            return charset;
        }
        
        protected virtual Encoding GetEncoding(string charset)
        {
            Encoding e = Encoding.UTF8;
            if (charset != null)
            {
                try
                {
                    e = Encoding.GetEncoding(charset);
                }
                catch{}
            }

            return e;
        }

        protected virtual string CleanCharset(string charset)
        {
            //TODO temporary hack, this needs to be a configurable value
            if (charset == "cp1251") //Russian, Bulgarian, Serbian cyrillic
                charset = "windows-1251";

            return charset;
        }

        private MemoryStream GetRawData(HttpResponseMessage webResponse)
        {
            MemoryStream rawData = new MemoryStream();

            try
            {
                using (Stream rs = webResponse.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                {
                    byte[] buffer = new byte[1024];
                    int read = rs.Read(buffer, 0, buffer.Length);
                    while (read > 0)
                    {
                        rawData.Write(buffer, 0, read);
                        read = rs.Read(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Warn("Error occurred while downloading content of url {0}", webResponse.RequestMessage.RequestUri.AbsoluteUri);
                _logger.Warn(e);
            }

            return rawData;
        }

        public virtual void Dispose()
        {
            // Nothing to do
        }
    }
}
