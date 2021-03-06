﻿using Abot.Poco;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abot.Core
{
    /// <summary>
    /// Parser that uses CsQuery https://github.com/jamietre/CsQuery to parse page links
    /// </summary>
    public class CSQueryHyperlinkParser : HyperLinkParser
    {
        public CSQueryHyperlinkParser()
            :base()
        {
        }

        [Obsolete("Use the constructor that accepts a configuration object instead")]
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isRespectMetaRobotsNoFollowEnabled">Whether parser should ignore pages with meta no robots</param>
        /// <param name="isRespectAnchorRelNoFollowEnabled">Whether parser should ignore links with rel no follow</param>
        /// <param name="cleanURLFunc">Function to clean the url</param>
        /// <param name="isRespectUrlNamedAnchorOrHashbangEnabled">Whether parser should consider named anchor and/or hashbang '#' character as part of the url</param>
        public CSQueryHyperlinkParser(bool isRespectMetaRobotsNoFollowEnabled,
                                  bool isRespectAnchorRelNoFollowEnabled,
                                  Func<string, string> cleanURLFunc = null,
                                  bool isRespectUrlNamedAnchorOrHashbangEnabled = false)
            :this(new CrawlConfiguration
            {
                IsRespectMetaRobotsNoFollowEnabled = isRespectMetaRobotsNoFollowEnabled,
                IsRespectUrlNamedAnchorOrHashbangEnabled = isRespectUrlNamedAnchorOrHashbangEnabled,
                IsRespectAnchorRelNoFollowEnabled = isRespectAnchorRelNoFollowEnabled
            }, cleanURLFunc)
        {

        }

        public CSQueryHyperlinkParser(CrawlConfiguration config, Func<string, string> cleanURLFunc)
            : base(config, cleanURLFunc)
        {

        }

        protected override string ParserType
        {
            get { return "CsQuery"; }
        }

        protected override IEnumerable<string> GetHrefValues(CrawledPage crawledPage)
        {
            if (HasRobotsNoFollow(crawledPage))
                return null;

            IEnumerable<string> hrefValues = crawledPage.CsQueryDocument.QuerySelectorAll("a, area")
            .Where(e => !HasRelNoFollow(e))
            .Select(y => y.GetAttribute("href"))
            .Where(a => !string.IsNullOrWhiteSpace(a));

            IEnumerable<string> canonicalHref = crawledPage.CsQueryDocument.QuerySelectorAll("link")
                .Where(e => HasRelCanonicalPointingToDifferentUrl(e, crawledPage.Uri.ToString()))
                .Select(e => e.Attributes["href"].Value);

            return hrefValues.Concat(canonicalHref);
        }

        protected override string GetBaseHrefValue(CrawledPage crawledPage)
        {
            string baseTagValue = crawledPage.CsQueryDocument.QuerySelector("base")?.Attributes["href"].Value ?? "";
            return baseTagValue.Trim();
        }

        protected override string GetMetaRobotsValue(CrawledPage crawledPage)
        {
            return crawledPage.CsQueryDocument.QuerySelectorAll("meta[name]").FirstOrDefault(d => d.Attributes["name"]?.Value.ToLower() == "robots")?.Attributes["content"].Value;
        }

        protected virtual bool HasRelCanonicalPointingToDifferentUrl(IElement e, string orginalUrl)
        {
            return e.HasAttribute("rel") && !string.IsNullOrWhiteSpace(e.Attributes["rel"].Value) &&
                    string.Equals(e.Attributes["rel"].Value, "canonical", StringComparison.OrdinalIgnoreCase) &&
                    e.HasAttribute("href") && !string.IsNullOrWhiteSpace(e.Attributes["href"].Value) &&
                    !string.Equals(e.Attributes["href"].Value, orginalUrl, StringComparison.OrdinalIgnoreCase);
        }

        protected virtual bool HasRelNoFollow(IElement e)
        {
            return _config.IsRespectAnchorRelNoFollowEnabled && (e.HasAttribute("rel") && e.GetAttribute("rel").ToLower().Trim() == "nofollow");
        }
    }
}