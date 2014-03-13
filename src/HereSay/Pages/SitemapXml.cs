using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using HereSay.Decorators;
using HereSay.Persistence.Finder;
using N2;
using Rolcore.Web;
using Rolcore.Web.Protocols;


namespace HereSay.Pages
{
    [N2.PageDefinition(
        "Sitemap XML",
        Name = "SitemapXml",
        SortOrder = PageSorting.VeryRareUse,
        Description = "An XML file that lists URLs with additional information about each URL.",
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootOrStartPage,
        TemplateUrl = CustomTextContent.HttpHandlerTemplateUrl,
        IconUrl = "~/N2/Resources/icons/sitemap.png"),
     N2.Integrity.RestrictParents(typeof(HomePage), typeof(LanguageHomePage), typeof(LanguageHomeRedirectPage)),
     N2.Web.UI.TabContainer(
         EditModeTabs.PageInformationName,
         EditModeTabs.PageInformationTitle,
         EditModeTabs.PageInformationSortOrder),
     N2.Details.WithEditableName("Path Segment", 20, ContainerName = EditModeTabs.PageInformationName, Required = true, RequiredMessage = "Path Segment is required.")]
    public class SitemapXml : CustomTextContent
    {
        private const string XmlKey = "Xml";

        protected string GenerateXml(ContentItem startItem, Sitemap result = null)
        {
            IEnumerable<N2.ContentItem> pages = Find.Items
                .Where
                    .Parent.Eq(startItem)
                    .And.IsPublished()
                    .And.Type.NotEq(typeof(SitemapXml))                 // Minimize selection as
                    .And.Type.NotEq(typeof(CustomCssContent))           // much as possible
                    .And.Type.NotEq(typeof(CustomJavaScriptContent))
                    .And.Type.NotEq(typeof(RedirectPage))
                    .And.Type.NotEq(typeof(FeedPage))
                //.And.State.Eq(ContentState.Published)
                .Select()
                    .Where(item =>
                        item.IsPage
                        && SitemapDecorator.AutoDefineOnType(item.GetType())
                        && !item.GetDetail<bool>(EditModeFields.Syndication.ExcludeFromSiteMapName, false))
                .Union(new N2.ContentItem[] { startItem });

            result = result ?? new Sitemap();
            foreach (N2.ContentItem page in pages)
            {
                string url = (page is WebPage)
                    ? ((WebPage)page).CanonicalUrl //TODO: Fully qualify
                    : page.GetSafeUrl();

                if (!result.Where(item => item.Loc == url).Any())
                {
                    SitemapUrl pageUrl = new SitemapUrl()
                    {
                        LastModifiedDate = page.Updated,
                        Loc = url
                    };
                    result.Add(pageUrl);
                }
                if (page != startItem)
                    GenerateXml(page, result);
            }
            return result.ToString();
        }

        protected internal void Regenerate(N2.ContentItem startingPoint)
        {
            if (startingPoint == null)
                throw new ArgumentNullException("startingPoint", "startingPoint is null.");

            this.SetDetail<string>(XmlKey, this.GenerateXml(startingPoint));
            N2.Context.Persister.Save(this);
        }

        protected override string ResponseContentText
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()), "Result was null or whitespace.");

                string result = this.GetDetail<string>(XmlKey, null);
                if (string.IsNullOrWhiteSpace(result))
                {
                    Regenerate(Find.StartPage);
                    result = this.GetDetail<string>(XmlKey, null);
                }

                return result;
            }
        }

        protected override string ResponseContentType
        {
            get { return ContentTypes.TextXml; }
        }
    }
}
