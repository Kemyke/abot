using Abot.Poco;
using Microsoft.Extensions.Configuration;
using System;

namespace Abot.Core
{
    public class AbotConfigurationSectionHandler 
    {
        private IConfigurationRoot config;
        public AbotConfigurationSectionHandler(IConfigurationRoot cr)
        {
            config = cr;
        }

        public CrawlBehaviorElement CrawlBehavior
        {
            get { return new CrawlBehaviorElement(config); }
        }

        public PolitenessElement Politeness
        {
            get { return new PolitenessElement(config); }
        }

        public AuthorizationElement Authorization
        {
            get { return new AuthorizationElement(config); }
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
        private IConfigurationSection config;
        public AuthorizationElement(IConfigurationRoot cr)
        {
            config = cr.GetSection("Authorization");
        }

        /// <summary>
        /// Defines whatewer each request shold be autorized via login 
        /// </summary>
        public bool IsAlwaysLogin
        {
            get { return bool.Parse(config["IsAlwaysLogin"]); }
        }

        /// <summary>
        /// The user name to be used for autorization 
        /// </summary>
        public string LoginUser
        {
            get { return config["LoginUser"]; }
        }
        /// <summary>
        /// The password to be used for autorization 
        /// </summary>
        public string LoginPassword
        {
            get { return config["LoginPassword"]; }
        }
    }
    public class PolitenessElement 
    {
        private IConfigurationSection config;
        public PolitenessElement(IConfigurationRoot cr)
        {
            config = cr.GetSection("Politeness");
        }

        public bool IsRespectRobotsDotTextEnabled
        {
            get { return bool.Parse(config["IsRespectRobotsDotTextEnabled"]); }
        }

        public bool IsRespectMetaRobotsNoFollowEnabled
        {
            get { return bool.Parse(config["IsRespectMetaRobotsNoFollowEnabled"]); }
        }

        public bool IsRespectHttpXRobotsTagHeaderNoFollowEnabled
        {
            get { return bool.Parse(config["IsRespectHttpXRobotsTagHeaderNoFollowEnabled"]); }
        }

        public bool IsRespectAnchorRelNoFollowEnabled
        {
            get { return bool.Parse(config["IsRespectAnchorRelNoFollowEnabled"]); }
        }

        public bool IsIgnoreRobotsDotTextIfRootDisallowedEnabled
        {
            get { return bool.Parse(config["IsIgnoreRobotsDotTextIfRootDisallowedEnabled"]); }
        }

        public string RobotsDotTextUserAgentString
        {
            get { return config["RobotsDotTextUserAgentString"]; }
        }

        public int MaxRobotsDotTextCrawlDelayInSeconds
        {
            get { return int.Parse(config["MaxRobotsDotTextCrawlDelayInSeconds"]); }
        }

        public int MinCrawlDelayPerDomainMilliSeconds
        {
            get { return int.Parse(config["MinCrawlDelayPerDomainMilliSeconds"]); }
        }
    }

    public class CrawlBehaviorElement
    {
        private IConfigurationSection config;
        public CrawlBehaviorElement(IConfigurationRoot cr)
        {
            config = cr.GetSection("CrawlBehavior");
        }


        public int MaxConcurrentThreads
        {
            get { return int.Parse(config["MaxConcurrentThreads"]); }
        }
        
        public int MaxPagesToCrawl
        {
            get { return int.Parse(config["MaxPagesToCrawl"]); }
        }
        
        public int MaxPagesToCrawlPerDomain
        {
            get { return int.Parse(config["MaxPagesToCrawlPerDomain"]); }
        }

        public int MaxPageSizeInBytes
        {
            get { return int.Parse(config["MaxPageSizeInBytes"]); }
        }

        public string UserAgentString
        {
            get { return config["UserAgentString"]; }
        }

        public int CrawlTimeoutSeconds
        {
            get { return int.Parse(config["CrawlTimeoutSeconds"]); }
        }

        public string DownloadableContentTypes
        {
            get { return config["DownloadableContentTypes"]; }
        }

        public bool IsUriRecrawlingEnabled
        {
            get { return bool.Parse(config["IsUriRecrawlingEnabled"]); }
        }

        public bool IsExternalPageCrawlingEnabled
        {
            get { return bool.Parse(config["IsExternalPageCrawlingEnabled"]); }
        }

        public bool IsExternalPageLinksCrawlingEnabled
        {
            get { return bool.Parse(config["IsExternalPageLinksCrawlingEnabled"]); }
        }

        public bool IsSslCertificateValidationEnabled
        {
            get { return bool.Parse(config["IsSslCertificateValidationEnabled"]); }
        }

        public int HttpServicePointConnectionLimit
        {
            get { return int.Parse(config["HttpServicePointConnectionLimit"]); }
        }

        public int HttpRequestTimeoutInSeconds
        {
            get { return int.Parse(config["HttpRequestTimeoutInSeconds"]); }
        }
        
        public int HttpRequestMaxAutoRedirects
        {
            get { return int.Parse(config["HttpRequestMaxAutoRedirects"]); }
        }
        
        public bool IsHttpRequestAutoRedirectsEnabled
        {
            get { return bool.Parse(config["IsHttpRequestAutoRedirectsEnabled"]); }
        }
        
        public bool IsHttpRequestAutomaticDecompressionEnabled
        {
            get { return bool.Parse(config["IsHttpRequestAutomaticDecompressionEnabled"]); }
        }
        
        public bool IsSendingCookiesEnabled
        {
            get { return bool.Parse(config["IsSendingCookiesEnabled"]); }
        }

        public bool IsRespectUrlNamedAnchorOrHashbangEnabled
        {
            get { return bool.Parse(config["IsRespectUrlNamedAnchorOrHashbangEnabled"]); }
        }

        public int MinAvailableMemoryRequiredInMb
        {
            get { return int.Parse(config["MinAvailableMemoryRequiredInMb"]); }
        }

        public int MaxMemoryUsageInMb
        {
            get { return int.Parse(config["MaxMemoryUsageInMb"]); }
        }

        public int MaxMemoryUsageCacheTimeInSeconds
        {
            get { return int.Parse(config["MaxMemoryUsageCacheTimeInSeconds"]); }
        }
        
        public int MaxCrawlDepth
        {
            get { return int.Parse(config["MaxCrawlDepth"]); }
        }
        
        public int MaxLinksPerPage
        {
            get { return int.Parse(config["MaxLinksPerPage"]); }
        }
        
        public bool IsForcedLinkParsingEnabled
        {
            get { return bool.Parse(config["IsForcedLinkParsingEnabled"]); }
        }
        
        public int MaxRetryCount
        {
            get { return int.Parse(config["MaxRetryCount"]); }
        }
        
        public int MinRetryDelayInMilliseconds
        {
            get { return int.Parse(config["MinRetryDelayInMilliseconds"]); }
        }
    }

    public class ExtensionValueElement
    {
        private IConfigurationSection config;
        public ExtensionValueElement(IConfigurationRoot cr)
        {
            config = cr.GetSection("ExtensionValue");
        }

        public string Key
        {
            get { return config["Key"]; }
        }

        public string Value
        {
            get { return config["Value"]; }
        }
    }
}
