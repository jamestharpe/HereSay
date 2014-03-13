using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using N2.Edit.Workflow;
using Rolcore;
using Rolcore.Web;
using N2.Engine.Globalization;
using HereSay.Globalization;
using N2.Collections;
using N2;
using HereSay.Engine;

namespace HereSay
{
    /// <summary>
    /// Provides extension methods for <see cref="N2.Contentitem"/>.
    /// </summary>
    public static class ContentItemExtensions
    {
        internal static string GetTemplateUrl(this ContentItem item, string templatesRoot, string templateName, string templateFileExtension, string defaultTemplateName)
        {
            string result = templatesRoot + templateName;

            string templatePath = HttpContext.Current.Server.MapPath(result);
            if (!File.Exists(templatePath))
            {
                Debug.WriteLine(string.Format("HereSay: Could not find {0} using default.", result));
                result = templatesRoot + defaultTemplateName + templateFileExtension;
            }

            return result;
        }

        internal static string[] GetAvailableTemplates(this ContentItem contentItem, string templatesRoot, string templateSearchPatternFormat, string defaultTemplateName)
        {
            //
            // Pre-conditions 

            if (contentItem == null)
                throw new ArgumentNullException("contentItem", "contentItem is null.");

            HttpContext context = HttpContext.Current;
            if (context == null)
                throw new InvalidOperationException("HttpContext is not available, but is required.");

            DirectoryInfo pageTemplatesDirectory = new DirectoryInfo(context.Server.MapPath(templatesRoot));
            if (!pageTemplatesDirectory.Exists)
                throw new InstallationException(templatesRoot
                    + " does not contain any templates.");

            string typeName = contentItem.GetType().Name;
            string searchPattern = string.Format(templateSearchPatternFormat, typeName);
            FileInfo[] templateFiles = pageTemplatesDirectory.GetFiles(searchPattern);

            if (templateFiles.Length == 0)
            {
                searchPattern = string.Format(templateSearchPatternFormat, defaultTemplateName);
                templateFiles = pageTemplatesDirectory.GetFiles(searchPattern);
                if (templateFiles.Length == 0)
                    throw new InstallationException("No templates available for " + typeName);
            }

            string[] result = templateFiles
                .Select(themeFile => themeFile.Name)
                .ToArray();

            // Move the default template to the top of the list. The ensures the default template is used
            // whenever there is no template set on a given page.
            string defaultTemplate = searchPattern.Substring(1); // the searchPattern variable is what we want minus the prepended asterick (*)
            if (result.Contains(defaultTemplate))
            {
                int index = Array.IndexOf(result, defaultTemplate);
                result[index] = result[0];
                result[0] = defaultTemplate;
            }

            return result;
        }

        /// <summary>
        /// Gets the site source for the specified <see cref="ContentItem"/>.
        /// <seealso cref="N2.Web.ISitesSource"/>
        /// </summary>
        /// <param name="contentItem">Specifies the content item.</param>
        /// <returns></returns>
        public static N2.Web.ISitesSource GetSitesSource(this ContentItem contentItem)
        {
            if (contentItem == null)
                return Find.StartPage as N2.Web.ISitesSource;

            var currentItem = contentItem;
            do
            {
                var result = currentItem as N2.Web.ISitesSource;
                if (result != null)
                    return result;

                currentItem = currentItem.Parent;
            } while (currentItem != null);

            return Find.StartPage as N2.Web.ISitesSource;
        }

        /// <summary>
        /// Returns the available translations for the specified item.
        /// </summary>
        /// <param name="contentItem">Specifies the content item for which to find a translation.</param>
        /// <returns>Available translations</returns>
        public static IEnumerable<ContentTranslation> GetTranslations(this ContentItem contentItem)
        {
            //
            // Pre-conditions 

            if (contentItem == null)
                throw new ArgumentNullException("contentItem", "contentItem is null.");

            var languageGateway = N2.Context.Current.GetLanguageGateway();
            using (var languageFilter = new AllFilter(new AccessFilter(), new PublishedFilter()))
            {
                var translationItems = languageGateway.FindTranslations(Context.CurrentPage);
                foreach (var translation in languageFilter.Pipe(translationItems))
                {
                    var language = languageGateway.GetLanguage(translation);

                    //
                    // Hide translations when filtered access to their language

                    var languageItem = language as ContentItem;
                    if (languageItem == null || languageFilter.Match(languageItem))
                        yield return new ContentTranslation(translation, language);
                }
            }
        }

        public static ILanguage GetLanguage(this ContentItem contentItem, bool fromCache = true)
        {
            //
            // Pre-conditions

            if (contentItem == null)
                throw new ArgumentNullException("contentItem", "contentItem is null.");

            //
            // Check request-level cache

            ILanguage result = null;
            if (fromCache)
            {
                result = HttpContext.Current.Items["CurrentLanguage"] as ILanguage;
            }

            if (result == null)
            {
                //
                // Not in cache, get from context

                ILanguageGateway languageGateway = N2.Context.Current.GetLanguageGateway();
                if (languageGateway != null)
                {
                    result = languageGateway.GetLanguage(contentItem);

                    //
                    // Cache

                    HttpContext.Current.Items["CurrentLanguage"] = result;
                }
            }


            return result;
        }

        /// <summary>
        /// Returns a value specifying a consistent URL for the content item, even if it is not 
        /// published.
        /// </summary>
        /// <param name="contentItem">Specifies a content item.</param>
        /// <param name="cacheResult">Specifies if the result should be cached in the ASP.NET Cache</param>
        /// <returns>A <see cref="string"/> that represents a URL to the content item.</returns>
        public static string GetSafeUrl(this ContentItem contentItem, bool cacheResult = true)
        {
            //
            // Pre-conditions

            if (contentItem == null)
                return string.Empty;
            if (!contentItem.IsPage || contentItem.State == ContentState.None)
                return contentItem.Url;

            //
            // Determine base-url/authority from sites source and requested URL.

            HttpContext httpContext = HttpContext.Current;
            HttpRequest request = httpContext.Request;
            N2.Web.ISitesSource sitesSource = contentItem.GetSitesSource();
            IEnumerable<N2.Web.Site> sites = (sitesSource != null) ? sitesSource.GetSites() : new List<N2.Web.Site>();
            N2.Web.Site site = sites // Try to match domain, otherwise default to first-available authority
                .Where(s =>
                    s.Authority.Equals(request.Url.Authority, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault()
                ?? sites.FirstOrDefault();

            Uri siteBaseUrl = (site != null) // "Safe" URLs are fully qualified
                ? new Uri(string.Format("{0}{1}{2}", request.Url.Scheme, Uri.SchemeDelimiter, site.Authority))
                : httpContext.GetSiteBaseUrl();

            //
            // Check cache

            System.Web.Caching.Cache cache = httpContext.Cache;
            cacheResult = cacheResult && cache != null;
            string cacheKey = null;

            if (cacheResult)
            {
                cacheKey = string.Format(
                    "ContentItemExtensions.SafeUrl_{0}{1}{2}{3}",
                    contentItem.ID,
                    contentItem.Updated.Ticks,
                    contentItem.State, // forces re-calc when a page is published
                    siteBaseUrl);
                string cachedResult = cache[cacheKey] as string;
                if (!string.IsNullOrWhiteSpace(cachedResult))
                    return cachedResult;
            }

            //
            // Not in cache, calc result

            string result;
            ContentItem safeParent = contentItem.GetSafeParent();
            if ((safeParent == null) || (safeParent == Find.RootItem))
                result = siteBaseUrl.ToString(); // contentItem.Url.ToUri(siteBaseUrl).ToString();
            else
            {
                StringBuilder resultBuilder = new StringBuilder(safeParent.GetSafeUrl().EnsureTrailing('/') + contentItem.Name);

                if (contentItem.Children.Any(item => item.IsPage))
                    //if (contentItem.GetPublishedChildren<N2.ContentItem>(true).Any(item => item.IsPage))
                    resultBuilder.Append('/');

                result = resultBuilder.ToString().ToUri(siteBaseUrl).ToString();
            }

            //
            // Cache result

            if (cacheResult)
                cache[cacheKey] = result;

            return result;
        }

        /// <summary>
        /// Returns the parent of the given item, even if the item is not published.
        /// </summary>
        /// <param name="contentItem">Specifies the item.</param>
        /// <returns>The parent of the item, or the parent of the currently published version of 
        /// the item.</returns>
        public static ContentItem GetSafeParent(this ContentItem contentItem)
        {
            return contentItem.Parent ?? ((contentItem.VersionOf == null) ? null : contentItem.VersionOf.Parent);
        }

        public static IEnumerable<TContentItem> GetPublishedSiblings<TContentItem>(this ContentItem contentItem, bool includeItemsAssignableFromTContentItem)
        {
            return Find.PublishedChildren<TContentItem>(
                contentItem.GetSafeParent(),
                includeItemsAssignableFromTContentItem);
        }

        public static IEnumerable<TContentItem> GetPublishedChildren<TContentItem>(this ContentItem contentItem, bool includeItemsAssignableFromTContentItem)
            where TContentItem : ContentItem
        {
            return Find.PublishedChildren<TContentItem>(
                contentItem,
                includeItemsAssignableFromTContentItem);
        }

        public static IEnumerable<TContentItem> GetPublishedParents<TContentItem>(this ContentItem contentItem, bool includeItemsAssignableFromTContentItem)
            where TContentItem : ContentItem
        {
            return Find.PublishedParents<TContentItem>(
                contentItem,
                includeItemsAssignableFromTContentItem);
        }

    }
}
