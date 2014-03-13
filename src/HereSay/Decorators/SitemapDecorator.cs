using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HereSay.Definitions;
using HereSay.Pages;
using HereSay.Persistence.Finder;
using N2.Definitions;
using N2.Engine;
using N2.Plugin;
using N2.Persistence;

namespace HereSay.Decorators
{

    [Service, AutoInitialize]
    public class SitemapDecorator : AutoStarter
    {
        private readonly IDefinitionManager definitions;
        private readonly IPersister persister;
        
        public SitemapDecorator() { }
        public SitemapDecorator(IDefinitionManager definitions, IPersister persister)
        {
            this.definitions = definitions;
            this.persister = persister;
        }

        public static bool AutoDefineOnType(Type type)
        {
            return !typeof(SitemapXml).IsAssignableFrom(type)
                && !typeof(CustomCssContent).IsAssignableFrom(type)
                && !typeof(CustomJavaScriptContent).IsAssignableFrom(type)
                && !typeof(RedirectPage).IsAssignableFrom(type)
                && !typeof(FeedPageBase).IsAssignableFrom(type)
                && !typeof(WebsiteRoot).IsAssignableFrom(type);
        }

        public override void Start()
        {
            // add the Syndication tab as a new "WebPage" definition
            IEnumerable<ItemDefinition> definitions = this.definitions.GetDefinitions()
                .Where(definition =>
                       IsPage(definition.ItemType)
                    && AutoDefineOnType(definition.ItemType));

            Parallel.ForEach<ItemDefinition>(definitions, definition =>
            {
                definition.EnsureTab(
                    EditModeTabs.PageInformationName,
                    EditModeTabs.PageInformationTitle,
                    EditModeTabs.PageInformationSortOrder);

                int sortOrder = definition.GetMaxEditableSortOrder();

                definition.AddEditableCheckBox(
                    EditModeFields.Syndication.ExcludeFromSiteMapTitle,
                    ++sortOrder,
                    EditModeTabs.PageInformationName,
                    EditModeFields.Syndication.ExcludeFromSiteMapName,
                    false);
            });

            persister.ItemCopied += Persister_SitemapChangeNeeded;
            persister.ItemMoved += Persister_SitemapChangeNeeded;
            persister.ItemDeleted += Persister_SitemapChangeNeeded;
            persister.ItemSaved += Persister_SitemapChangeNeeded;

            Debug.WriteLine("HereSay: SitemapDecorator Started");
        }

        static void Persister_SitemapChangeNeeded(object sender, N2.ItemEventArgs e)
        {
            DateTime published = e.AffectedItem.Published.GetValueOrDefault(DateTime.MaxValue);
            bool ignoreChange =
                published > DateTime.Now ||
                !e.AffectedItem.IsPage ||
                !AutoDefineOnType(e.AffectedItem.GetType());

            if (ignoreChange)
                return;

            IEnumerable<N2.ContentItem> sitemaps = Find.Items
                .Where
                // Add back when legacy compatibility is no longer an issue: .Type.Eq(typeof(SitemapXml))
                    .Type.NotEq(typeof(WebPage))
                //TODO: Remove all these Type.NotEq lines when we add back the .Type.Eq(typeof(SitemapXml)) line
                    .And.Type.NotEq(typeof(CustomContent))
                    .And.Type.NotEq(typeof(CustomCssContent))
                    .And.Type.NotEq(typeof(CustomHtmlContent))
                    .And.Type.NotEq(typeof(FeedPage))
                    .And.Type.NotEq(typeof(HomePage))
                    .And.Type.NotEq(typeof(LanguageHomePage))
                    .And.Type.NotEq(typeof(LanguageHomeRedirectPage))
                    .And.Type.NotEq(typeof(RedirectPage))
                    .And.IsPublished()
                .Select<N2.ContentItem>();
            //.Where(item => item.GetType().IsAssignableFrom(typeof(SitemapXml)))
            //.Cast<SitemapXml>();

            foreach (N2.ContentItem sitemap in sitemaps)
            {
                if (sitemap is SitemapXml)
                    ((SitemapXml)sitemap).Regenerate(Find.StartPage);
            }
        }

        public override void Stop()
        {
            Debug.WriteLine("HereSay: SitemapDecorator Stopped");
        }
    }
}
