using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using N2.Definitions;
using N2.Plugin;
using HereSay.Definitions;
using Rolcore.Web;
using System.Diagnostics.Contracts;
using N2.Engine;
using Rolcore;
using N2.Web;

namespace HereSay
{
    /// <summary>
    /// A HereSay <see cref="AutoStarter"/> that automatically redirects requests to a consistent 
    /// URL. This helps to prevent "duplicate" content in Google and other search engines.
    /// </summary>
    [AutoInitialize]
    public class SafeUrlResponseModifier : AutoStarter
    {
        private const string EnabledPropertyName = "SafeUrlResponseModifier.Enabled";
        private const bool EnabledDefaultValue = true;

        /// <summary>
        /// Forces the correct "safe" URL.
        /// </summary>
        private static void ApplicationInstance_AcquireRequestState(object sender, EventArgs e)
        {
            N2.ContentItem page = GetCurrentPage();

            if ((page != null) && (page.IsPage))
            {
                //
                // Check that this feature is enabled

                N2.ContentItem home = page.GetPublishedParents<Pages.HomePage>(true).FirstOrDefault();

                bool enabled = (home != null)
                    ? home.GetDetail<bool>(EnabledPropertyName, EnabledDefaultValue)
                    : true;

                if (!enabled)
                    return;

                //
                // Feature is enabled, redirect to safe URL if necessary

                HttpContext httpContext = HttpContext.Current;
                Uri siteBaseUrl = httpContext.GetSiteBaseUrl();
                string
                    rawUrl = httpContext.Request.RawUrl.ToUri(siteBaseUrl).ToString(), //TODO: Strip out page parameter
                    safeUrl = page.GetSafeUrl();

                if (!rawUrl.Equals(safeUrl, StringComparison.InvariantCulture))
                {
                    HttpResponse response = ((HttpApplication)sender).Response;
                    int queryStringIndex = rawUrl.IndexOf('?');
                    if (queryStringIndex < 0)
                    {   // Not equal to SafeUrl - redirect
                        Debug.WriteLine(string.Format("Redirecting {0} to {1}", rawUrl, safeUrl));
                        response.RedirectPermanent(safeUrl);
                    }
                    else if (queryStringIndex != safeUrl.Length)
                    {
                        // There was a query string - this could have caused the difference
                        string 
                            queryString = rawUrl.Substring(queryStringIndex),
                            destination = safeUrl + queryString;
                        Debug.WriteLine(string.Format("Redirecting {0} to {1}", rawUrl, destination));
                        response.RedirectPermanent(destination);
                    }
                    else
                        Debug.WriteLine(string.Format("NOT REDIRECTING {0} to {1}", rawUrl, safeUrl));
                }
            }
        }

        public override void Start()
        {
            // foreach (ItemDefinition definition in this.Definitions)

            IEnumerable<ItemDefinition> homePageDefinitions = this.Definitions
               .Where(definition =>
                   IsPage(definition.ItemType)
                && (typeof(Pages.HomePage).IsAssignableFrom(definition.ItemType)));


            Parallel.ForEach<ItemDefinition>(homePageDefinitions, definition =>
            {
                Type itemType = definition.ItemType;
                if (IsPage(itemType))
                {

                    definition.EnsureTab(
                        EditModeTabs.Advanced,
                        EditModeTabs.Advanced,
                        EditModeTabs.AdvancedSortOrder);

                    int sortOrder = definition.GetMaxEditableSortOrder();

                    definition.AddEditableCheckBox(
                        "Force Safe URLs",
                        ++sortOrder,
                        EditModeTabs.Advanced,
                        EnabledPropertyName,
                        EnabledDefaultValue);
                }
            });

            EventBroker.Instance.AcquireRequestState += ApplicationInstance_AcquireRequestState;

            Debug.WriteLine("SafeUrlResponseModifier Started");
        }

        public override void Stop()
        {
            Debug.WriteLine("SafeUrlResponseModifier Stopped");
        }
    }
}
