using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using HereSay.Engine;
using HereSay.Pages;
using Rolcore.Web;
using  N2.Engine.Globalization;
using N2;
using System.Collections.Generic;

namespace HereSay
{
    public static class HttpContextExtensionMethods
    {
        /// <summary>
        /// For an <see cref="N2"/> page, returns the <see cref="CmsContentPage.CanonicalUrl"/> value. Otherwise
        /// gets the <see cref="UnrewrittenUrl"/> value of the current request.
        /// </summary>
        public static string GetCanonicalUrl(this HttpContext httpContext)
        {
            N2.ContentItem n2Item = N2.Context.CurrentPage;
            if (n2Item != null)
            {
                if (n2Item is WebPage)
                    return ((WebPage)n2Item).CanonicalUrl;
                // Legacy support...
                return (n2Item["CanonicalUrl"] as string) ?? httpContext.GetSiteBaseUrl() + n2Item.GetSafeUrl();
            }
            return httpContext.GetUnrewrittenUrl();
        }

        public static string GetFullyQualifiedCanonicalUrl(this HttpContext httpContext)
        {
            string siteBaseUri = httpContext.GetSiteBaseUrl().ToString();
            string canonicalUrl = httpContext.GetCanonicalUrl();
            if (!canonicalUrl.Contains(siteBaseUri) && !canonicalUrl.Contains(Uri.SchemeDelimiter))
            {
                if (canonicalUrl.Length > 1) // TODO: Why is this 1?
                    canonicalUrl = canonicalUrl.Substring(1);
                else if (canonicalUrl == "/")
                    canonicalUrl = string.Empty;
                return siteBaseUri + canonicalUrl;
            }
            return httpContext.GetCanonicalUrl();
        }

        public static string GetPreferredLanguageHomePageUrl(this HttpContext httpContext, string defaultResult)
        {
            string languageMatch = string.Empty;
            string partialMatch = string.Empty;
            HttpRequest request = httpContext.Request;
            if (request != null)
            {
                string[] userLanguages = request.UserLanguages;
                if ((userLanguages != null) && (userLanguages.Length > 0))
                {
                    IEnumerable<ContentItem> languages = N2.Context.Current
                        .GetLanguageGateway().GetAvailableLanguages()
                        .Cast<ContentItem>();

                    foreach (string userLanguage in userLanguages)
                    {
                        if (string.IsNullOrWhiteSpace(userLanguage))
                            continue;

                        //
                        // Find exact match

                        ContentItem pageInLanguage = languages
                            .Where(page =>
                                ((ILanguage)page).LanguageCode.Equals(userLanguage, StringComparison.InvariantCultureIgnoreCase))
                            .FirstOrDefault();

                        if (pageInLanguage != null)
                            return pageInLanguage.GetSafeUrl();

                        //
                        // Try partial match

                        string[] userLanguageParts = userLanguage.Split('-'); // en-US -> en

                        pageInLanguage = languages
                            .Where(page =>
                                ((ILanguage)page).LanguageCode.StartsWith(userLanguageParts[0], StringComparison.InvariantCultureIgnoreCase))
                            .FirstOrDefault();

                        if (pageInLanguage != null)
                            return pageInLanguage.GetSafeUrl();
                    }
                }
            }

            return defaultResult;
        }
    }
}
