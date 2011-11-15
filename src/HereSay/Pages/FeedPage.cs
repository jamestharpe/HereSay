using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using Rolcore.Web;
using N2.Details;

namespace HereSay.Pages
{
    [N2.PageDefinition(
        "Syndication Feed",
        Name = "SyndicationFeed",
        SortOrder = PageSorting.RareUse,
        Description = "Establishes a Syndication Feed of a specified type (RSS or ATOM) based on sibling pages.", 
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootOrStartPage,
        TemplateUrl = CustomTextContent.HttpHandlerTemplateUrl,
        IconUrl = "~/N2/Resources/icons/rss_feed.gif"),
     WithEditableTitle("Feed Title", 10, ContainerName = EditModeTabs.PageInformationName, Required = true, RequiredMessage = "Feed Title is required."),
     WithEditableName("Path Segment", 20, ContainerName = EditModeTabs.PageInformationName, Required = true, RequiredMessage = "Path Segment is required.")]
    public class FeedPage : FeedPageBase
    {
        private SyndicationFeed _Feed;

        [EditableCheckBox("Syndicate section", 10, ContainerName=EditModeTabs.PageInformationName)]
        public bool SyndicateSection
        {
            get { return GetDetail<bool>("SyndicateSection", true); }
            set { SetDetail<bool>("SyndicateSection", value, true); }
        }

        public IEnumerable<WebPage> FeedSource
        {
            get
            {
                IEnumerable<WebPage> result = this.GetPublishedSiblings<WebPage>(true)
                    .Where(webPage =>
                        !webPage.GetExcludeFromFeed()
                     && webPage.Published.Value >= this.StartDateFilter.GetValueOrDefault(DateTime.MinValue)
                     && webPage.Published.Value <= this.EndDateFilter.GetValueOrDefault(DateTime.MaxValue))
                    .OrderByDescending(webPage => 
                        webPage.GetDetail<DateTime>(EditModeFields.Syndication.ContentPublishDateName, webPage.Published.Value));

                // TODO: Is there a way (if SyndicateSection is on) to "flatten" the list so that 
                //       we don't have to process EVERY page because it MIGHT have children that 
                //       need to be included in the FeedSource?

                if (this.SyndicateSection)
                {
                    foreach (WebPage page in result)
                    {
                        if (!page.Children.Where(item => item.IsPage).Any())
                            continue; // no children to syndicate

                        FeedPage feedFromPage = new FeedPage()
                        {
                            MaxNumberOfItemsToDisplay = this.MaxNumberOfItemsToDisplay,
                            Name = Guid.NewGuid().ToString(), // temp name
                            Published = DateTime.Now,
                            SyndicationFormat = this.SyndicationFormat,
                            Parent = page
                        };
                        result = result.Union(feedFromPage.FeedSource);
                    }

                    result = result.OrderByDescending(webPage => 
                        webPage.GetDetail<DateTime>(EditModeFields.Syndication.ContentPublishDateName, webPage.Published.Value));
                }

                if (this.MaxNumberOfItemsToDisplay > 0)
                    result = result.Take(this.MaxNumberOfItemsToDisplay);

                return result;
            }
        }

        public override SyndicationFeed Feed
        {
            get
            {
                if (_Feed == null) //TODO: Should we use the ASP.NET cache?
                {
                    List<WebPage> webPages = this.FeedSource.ToList();

                    List<SyndicationItem> feedItems = new List<SyndicationItem>(webPages.Count);

                    webPages.ForEach(pageToSyndicate =>
                    {
                        Debug.Assert(pageToSyndicate.Published.HasValue);
                        SyndicationItem pageItem = pageToSyndicate.ToSyndicationItem();
                        feedItems.Add(pageItem);
                    });

                    DateTimeOffset mostRecentlyModified = feedItems.Max(item => item.LastUpdatedTime);

                    _Feed = new SyndicationFeed(
                        this.Title.RemoveHtml(), 
                        this.Text,
                        new Uri(this.GetSafeParent().GetSafeUrl()),
                        this.SyndicationFeedId,
                        mostRecentlyModified,
                        feedItems);
                    _Feed.BaseUri = HttpContext.Current.GetSiteBaseUrl();
                }

                return _Feed;
            }
        }
    }
}
