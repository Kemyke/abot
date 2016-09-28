using Abot.Poco;
using NLog;
using System;
using System.CodeDom;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Abot.Core
{
    public interface IPageRequester : IDisposable
    {
        /// <summary>
        /// Make an http web request to the url and download its content
        /// </summary>
        CrawledPage MakeRequest(Uri uri);

        /// <summary>
        /// Make an http web request to the url and download its content based on the param func decision
        /// </summary>
        CrawledPage MakeRequest(Uri uri, Func<CrawledPage, CrawlDecision> shouldDownloadContent);

        ///// <summary>
        ///// Asynchronously make an http web request to the url and download its content based on the param func decision
        ///// </summary>
        //Task<CrawledPage> MakeRequestAsync(Uri uri, Func<CrawledPage, CrawlDecision> shouldDownloadContent);
    }

    public class PageRequester : IPageRequester
    {
        static ILogger _logger = LogManager.GetLogger("AbotLogger");

        protected CrawlConfiguration _config;
        protected IWebContentExtractor _extractor;
        protected CookieContainer _cookieContainer = new CookieContainer();

        public PageRequester(CrawlConfiguration config)
            : this(config, null)
        {

        }

        public PageRequester(CrawlConfiguration config, IWebContentExtractor contentExtractor)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            _config = config;

            _extractor = contentExtractor ?? new WebContentExtractor();
        }

        /// <summary>
        /// Make an http web request to the url and download its content
        /// </summary>
        public virtual CrawledPage MakeRequest(Uri uri)
        {
            return MakeRequest(uri, (x) => new CrawlDecision { Allow = true });
        }

        /// <summary>
        /// Make an http web request to the url and download its content based on the param func decision
        /// </summary>
        public virtual CrawledPage MakeRequest(Uri uri, Func<CrawledPage, CrawlDecision> shouldDownloadContent)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            CrawledPage crawledPage = new CrawledPage(uri);

            HttpRequestMessage request = null;
            HttpResponseMessage response = null;

            WinHttpHandler handler = new WinHttpHandler();
            handler.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            handler.MaxConnectionsPerServer = _config.HttpServicePointConnectionLimit;

            using (var client = new HttpClient(handler))
            {
                try
                {
                    request = BuildRequestObject(client, handler, uri);
                    crawledPage.RequestStarted = DateTime.Now;
                    response = client.SendAsync(request).GetAwaiter().GetResult();
                    ProcessResponseObject(handler, response);
                }
                catch (WebException e)
                {
                    crawledPage.WebException = e;

                    //if (e.Response != null)
                    //    response = e.Response;

                    _logger.Debug("Error occurred requesting url [{0}]", uri.AbsoluteUri);
                    _logger.Debug(e);
                }
                catch (Exception e)
                {
                    _logger.Debug("Error occurred requesting url [{0}]", uri.AbsoluteUri);
                    _logger.Debug(e);
                }
                finally
                {
                    try
                    {
                        crawledPage.HttpWebRequest = request;
                        crawledPage.RequestCompleted = DateTime.Now;
                        if (response != null)
                        {
                            crawledPage.HttpWebResponse = new HttpWebResponseWrapper(response);
                            CrawlDecision shouldDownloadContentDecision = shouldDownloadContent(crawledPage);
                            if (shouldDownloadContentDecision.Allow)
                            {
                                crawledPage.DownloadContentStarted = DateTime.Now;
                                crawledPage.Content = _extractor.GetContent(response);
                                crawledPage.DownloadContentCompleted = DateTime.Now;
                            }
                            else
                            {
                                _logger.Debug("Links on page [{0}] not crawled, [{1}]", crawledPage.Uri.AbsoluteUri, shouldDownloadContentDecision.Reason);
                            }

                            response.Dispose();//Should already be closed by _extractor but just being safe
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Debug("Error occurred finalizing requesting url [{0}]", uri.AbsoluteUri);
                        _logger.Debug(e);
                    }
                }
            }



            return crawledPage;
        }

        ///// <summary>
        ///// Asynchronously make an http web request to the url and download its content based on the param func decision
        ///// </summary>
        //public Task<CrawledPage> MakeRequestAsync(Uri uri, Func<CrawledPage, CrawlDecision> shouldDownloadContent)
        //{
        //    if (uri == null)
        //        throw new ArgumentNullException("uri");

        //    CrawledPage crawledPage = new CrawledPage(uri);
        //    crawledPage.RequestStarted = DateTime.Now;

        //    HttpWebRequest request = BuildRequestObject(uri);
        //    HttpWebResponse response = null;

        //    crawledPage.HttpWebRequest = request;
        //    crawledPage.RequestStarted = DateTime.Now;

        //    Task<WebResponse> task = Task.Factory.FromAsync(
        //        request.BeginGetResponse,
        //        asyncResult => request.EndGetResponse(asyncResult),
        //        null);

        //    return task.ContinueWith((Task<WebResponse> t) =>
        //    {
        //        crawledPage.RequestCompleted = DateTime.Now;

        //        if (t.IsFaulted)
        //        {
        //            //handle error
        //            Exception firstException = t.Exception.InnerExceptions.First();
        //            crawledPage.WebException = firstException as WebException;

        //            if (crawledPage.WebException != null && crawledPage.WebException.Response != null)
        //                response = (HttpWebResponse)crawledPage.WebException.Response;

        //            _logger.DebugFormat("Error occurred requesting url [{0}]", uri.AbsoluteUri);
        //            _logger.Debug(crawledPage.WebException);
        //        }
        //        else
        //        {
        //            ProcessResponseObject(response);
        //            response = (HttpWebResponse)t.Result;
        //        }

        //        if (response != null)
        //        {
        //            crawledPage.HttpWebResponse = response;
        //            CrawlDecision shouldDownloadContentDecision = shouldDownloadContent(crawledPage);
        //            if (shouldDownloadContentDecision.Allow)
        //            {
        //                crawledPage.DownloadContentStarted = DateTime.Now;
        //                crawledPage.Content = _extractor.GetContent(response);
        //                crawledPage.DownloadContentCompleted = DateTime.Now;
        //            }
        //            else
        //            {
        //                _logger.DebugFormat("Links on page [{0}] not crawled, [{1}]", crawledPage.Uri.AbsoluteUri,
        //                    shouldDownloadContentDecision.Reason);
        //            }

        //            response.Close(); //Should already be closed by _extractor but just being safe
        //        }

        //        return crawledPage;
        //    });
        //}

        protected virtual HttpRequestMessage BuildRequestObject(HttpClient c, WinHttpHandler handler, Uri uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            handler.AutomaticRedirection = _config.IsHttpRequestAutoRedirectsEnabled;
            request.Headers.UserAgent.ParseAdd(_config.UserAgentString);
            request.Headers.Accept.ParseAdd("*/*");

            if (_config.HttpRequestMaxAutoRedirects > 0)
                handler.MaxAutomaticRedirections = _config.HttpRequestMaxAutoRedirects;

            if (_config.IsHttpRequestAutomaticDecompressionEnabled)
                request.Headers.AcceptEncoding.ParseAdd("gzip, deflate");

            if (_config.HttpRequestTimeoutInSeconds > 0)
                c.Timeout = TimeSpan.FromSeconds(_config.HttpRequestTimeoutInSeconds);

            if (_config.IsSendingCookiesEnabled)
                handler.CookieContainer = _cookieContainer;

            //Supposedly this does not work... https://github.com/sjdirect/abot/issues/122
            //if (_config.IsAlwaysLogin)
            //{
            //    request.Credentials = new NetworkCredential(_config.LoginUser, _config.LoginPassword);
            //    request.UseDefaultCredentials = false;
            //}
            if (_config.IsAlwaysLogin)
            {
                string credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(_config.LoginUser + ":" + _config.LoginPassword));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            }

            return request;
        }

        protected virtual void ProcessResponseObject(WinHttpHandler handler, HttpResponseMessage response)
        {
            if (response != null && _config.IsSendingCookiesEnabled)
            {
                CookieCollection cookies = handler.CookieContainer.GetCookies(response.RequestMessage.RequestUri);
                _cookieContainer.Add(response.RequestMessage.RequestUri, cookies);
            }
        }

        public void Dispose()
        {
            if (_extractor != null)
            {
                _extractor.Dispose();
            }
            _cookieContainer = null;
            _config = null;
        }
    }
}