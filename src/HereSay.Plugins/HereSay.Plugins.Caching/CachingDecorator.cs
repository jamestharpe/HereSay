using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using N2.Definitions;
using N2.Plugin;
using HereSay.Definitions;
using N2.Web;
using HereSay;
using N2.Engine;

namespace HereSay.Plugins.Caching
{
    /// <summary>
    /// Decorates pages within HereSay with properties to manage caching.
    /// </summary>
    [Service, AutoInitialize]
    public class CachingDecorator : AutoStarter
    {
        private readonly IDefinitionManager definitions;

        public CachingDecorator() { }
        public CachingDecorator(IDefinitionManager definitions)
        {
            this.definitions = definitions;
        }

        /// <summary>
        /// Modifies the given response's <see cref="System.Web.HttpResponse.Cache"/> to use the 
        /// desired cache settings.
        /// </summary>
        private static void ApplicationInstance_AcquireRequestState(object sender, EventArgs e)
        {
            N2.ContentItem page = Find.CurrentPage;

            if (page == null)
                return;

            //
            // Set cacheability

            int cacheability = (int)(page.GetDetail<Cacheability>(CacheabilityName, Cacheability.Default));

            cacheability = (cacheability == (int)Cacheability.Default)
                ? (int)HttpCacheability.Private
                : cacheability;

            HttpCacheability httpCacheability = (HttpCacheability)cacheability;
            HttpResponse response = ((HttpApplication)sender).Response;

            response.Cache.SetCacheability(httpCacheability);

            //
            // Set cache Duration

            string cacheDurationValue = page.GetDetail<string>(CacheDurationName, (-1).ToString());

            if (string.IsNullOrWhiteSpace(cacheDurationValue))
                cacheDurationValue = (-1).ToString();

            int cacheDuration = int.Parse(cacheDurationValue);
            if (cacheDuration >= 0)
                response.Cache.SetExpires(DateTime.Now.AddMinutes(cacheDuration));

            response.Cache.VaryByParams["*"] = true; //TODO: Make this configurable. See: http://msdn.microsoft.com/en-us/library/y96218s9(VS.71).aspx
        }

        #region Constants
        public const string
            CacheabilityHelpTitle = "Cacheability Values",
            CacheabilityHelpText =
                "<ul><li><strong>NoCache</strong> sets the Cache-Control: no-cache header.<br/>"
                + "<li><strong>Private</strong> (default) sets Cache-Control: private to specify that the response is cacheable only on the client and not by shared (proxy server) caches.</li>"
                + "<li><strong>ServerAndNoCache</strong> Applies the settings of both Server and NoCache to indicate that the content is cached at the server but all others are explicitly denied the ability to cache the response.</li>"
                + "<li><strong>Public</strong> sets Cache-Control: public to specify that the response is cacheable by clients and shared (proxy) caches.</li>"
                + "<li><strong>ServerAndPrivate</strong> indicates that the response is cached at the server and at the client but nowhere else. Proxy servers are not allowed to cache the response.</li></ul>",
            CacheabilityTitle = "Cacheability",
            CacheabilityName = "Cacheability";

        public const string
            CacheDurationTitle = "Cache Duration (minutes)",
            CacheDurationHelpText =
                "Specifies the number of minutes content is cached. A negative value will cause the server default to be used. "
                + "1 hour = 60, 1 day = 1440, 1 week = 10080, 30 days = 43200, 1 month = 43829 (average), 1 year = 525948.",
            CacheDurationHelpTitle = "Cache Duration",
            CacheDurationName = "CacheDuration";
        #endregion Constants

        #region Required AutoStarter overrides
        /// <summary>
        /// This method is called automatically by N2 when the website starts. Registers properties
        /// with page types available within the current installation that enable CMS users to 
        /// manage output caching.
        /// </summary>
        public override void Start()
        {
            // foreach (ItemDefinition definition in definitions.GetDefinitions())
            Parallel.ForEach<ItemDefinition>(this.definitions.GetDefinitions(), definition =>
            {
                Type itemType = definition.ItemType;
                if (IsPage(itemType))
                {

                    definition.EnsureTab(
                        EditModeTabs.Advanced,
                        EditModeTabs.Advanced,
                        EditModeTabs.AdvancedSortOrder);

                    int sortOrder = definition.GetMaxEditableSortOrder();

                    definition.AddEditableEnum(
                        CacheabilityTitle,
                        ++sortOrder,
                        typeof(Cacheability),
                        EditModeTabs.Advanced,
                        CacheabilityName,
                        CacheabilityHelpTitle,
                        CacheabilityHelpText);

                    definition.AddEditableTextBox(
                        CacheDurationTitle,
                        ++sortOrder,
                        6,
                        EditModeTabs.Advanced,
                        CacheDurationName,
                        CacheDurationHelpTitle,
                        CacheDurationHelpText);
                }
            });

            EventBroker.Instance.AcquireRequestState += ApplicationInstance_AcquireRequestState;

            Debug.WriteLine("CachingDecorator Started");
        }

        public override void Stop()
        {
            Debug.WriteLine("CachingDecorator Stopped");
        }
        #endregion Required AutoStarter overrides
    }
}
