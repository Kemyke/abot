using Abot.Poco;
using System;

namespace Abot.Core
{
    public class AbotConfigurationSectionHandler 
    {

        public CrawlBehaviorElement CrawlBehavior
        {
            get; set;
        }

        public PolitenessElement Politeness
        {
            get; set;
        }

        public AuthorizationElement Authorization
        {
            get; set;
        }

        public CrawlConfiguration Convert()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CrawlBehaviorElement, CrawlConfiguration>();
                cfg.CreateMap<PolitenessElement, CrawlConfiguration>();
                cfg.CreateMap<AuthorizationElement, CrawlConfiguration>();
            });


            CrawlConfiguration config = new CrawlConfiguration();
            AutoMapper.Mapper.Map<CrawlBehaviorElement, CrawlConfiguration>(CrawlBehavior, config);
            AutoMapper.Mapper.Map<PolitenessElement, CrawlConfiguration>(Politeness, config);
            AutoMapper.Mapper.Map<AuthorizationElement, CrawlConfiguration>(Authorization, config);

            return config;
        }
    }

    public class AuthorizationElement
    {
        /// <summary>
        /// Defines whatewer each request shold be autorized via login 
        /// </summary>
        public bool IsAlwaysLogin
        {
            get; set;
        }

        /// <summary>
        /// The user name to be used for autorization 
        /// </summary>
        public string LoginUser
        {
            get; set;
        }
        /// <summary>
        /// The password to be used for autorization 
        /// </summary>
        public string LoginPassword
        {
            get; set;
        }
    }
    public class PolitenessElement 
    {
        public bool IsRespectRobotsDotTextEnabled
        {
            get; set;
        }

        public bool IsRespectMetaRobotsNoFollowEnabled
        {
            get; set;
        }

        public bool IsRespectHttpXRobotsTagHeaderNoFollowEnabled
        {
            get; set;
        }

        public bool IsRespectAnchorRelNoFollowEnabled
        {
            get; set;
        }

        public bool IsIgnoreRobotsDotTextIfRootDisallowedEnabled
        {
            get; set;
        }

        public string RobotsDotTextUserAgentString
        {
            get; set;
        }

        public int MaxRobotsDotTextCrawlDelayInSeconds
        {
            get; set;
        }

        public int MinCrawlDelayPerDomainMilliSeconds
        {
            get; set;
        }
    }

    public class CrawlBehaviorElement
    {
        
        public int MaxConcurrentThreads
        {
            get; set;
        }
        
        public int MaxPagesToCrawl
        {
            get; set;
        }
        
        public int MaxPagesToCrawlPerDomain
        {
            get; set;
        }

        public int MaxPageSizeInBytes
        {
            get; set;
        }

        public string UserAgentString
        {
            get; set;
        }

        public int CrawlTimeoutSeconds
        {
            get; set;
        }

        public string DownloadableContentTypes
        {
            get; set;
        }

        public bool IsUriRecrawlingEnabled
        {
            get; set;
        }

        public bool IsExternalPageCrawlingEnabled
        {
            get; set;
        }

        public bool IsExternalPageLinksCrawlingEnabled
        {
            get; set;
        }

        public bool IsSslCertificateValidationEnabled
        {
            get; set;
        }

        public int HttpServicePointConnectionLimit
        {
            get; set;
        }

        public int HttpRequestTimeoutInSeconds
        {
            get; set;
        }
        
        public int HttpRequestMaxAutoRedirects
        {
            get; set;
        }
        
        public bool IsHttpRequestAutoRedirectsEnabled
        {
            get; set;
        }
        
        public bool IsHttpRequestAutomaticDecompressionEnabled
        {
            get; set;
        }
        
        public bool IsSendingCookiesEnabled
        {
            get; set;
        }

        public bool IsRespectUrlNamedAnchorOrHashbangEnabled
        {
            get; set;
        }

        public int MinAvailableMemoryRequiredInMb
        {
            get; set;
        }

        public int MaxMemoryUsageInMb
        {
            get; set;
        }

        public int MaxMemoryUsageCacheTimeInSeconds
        {
            get; set;
        }
        
        public int MaxCrawlDepth
        {
            get; set;
        }
        
        public int MaxLinksPerPage
        {
            get; set;
        }
        
        public bool IsForcedLinkParsingEnabled
        {
            get; set;
        }
        
        public int MaxRetryCount
        {
            get; set;
        }
        
        public int MinRetryDelayInMilliseconds
        {
            get; set;
        }
    }

    public class ExtensionValueElement
    {
        public string Key
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }
    }
}
