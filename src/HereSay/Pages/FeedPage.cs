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
     N2.Web.UI.TabContainer(
         EditModeTabs.PageInformationName,
         EditModeTabs.PageInformationTitle,
         EditModeTabs.PageInformationSortOrder),
     N2.Details.WithEditableTitle("Feed Title", 10, ContainerName = EditModeTabs.PageInformationName, Required = true, RequiredMessage = "Feed Title is required."),
     N2.Details.WithEditableName("Path Segment", 20, ContainerName = EditModeTabs.PageInformationName, Required = true, RequiredMessage = "Path Segment is required.")]
    public class FeedPage : CustomTextContent
    {
        private SyndicationFeed _Feed;

        #region UI Properties

        [N2.Details.EditableCheckBox("Syndicate section", 10, ContainerName=EditModeTabs.PageInformationName)]
        public bool SyndicateSection
        {
            get { return GetDetail<bool>("SyndicateSection", true); }
            set { SetDetail<bool>("SyndicateSection", value, true); }
        }

        /// <summary>
        /// Gets a <see cref="Guid"/> representing a globally unique identifier value for the 
        /// current instance. This is necessary for RSS and Atom formats.
        /// </summary>
        public virtual string SyndicationFeedId
        {
            get
            {
                const string propertyKey = "SyndicationFeedId";

                string result = GetDetail<string>(propertyKey, null);
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = Guid.NewGuid().ToString();
                    this[propertyKey] = result;
                    N2.Context.Persister.Save(this);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets and sets the Description content of the feed.
        /// </summary>
        [N2.Details.EditableTextBox("Description", 30, ContainerName = EditModeTabs.PageInformationName, TextMode = TextBoxMode.MultiLine)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Gets and sets the maximum number of articles to display. Use a negative number (e.g. 
        /// -1) to represent infinity.
        /// </summary>
        [N2.Details.EditableTextBox("Max # Items to Display (-1=infinity)", 75, Validate = true, ValidationExpression = "^([-]|[0-9])[0-9]*$", ValidationMessage = "Must be an integer.", ContainerName = EditModeTabs.Content)]
        public virtual int MaxNumberOfItemsToDisplay
        {
            get { return (int)GetDetail("MaxNumberOfItemsToDisplay", -1); }
            set { SetDetail("MaxNumberOfItemsToDisplay", value, -1); }
        }

        /// <summary>
        /// The <see cref="DateTime?" /> in which this list will begin displaying 
        /// <see cref="CmsSyndicationFeedPage" /> items. Any items published before this date will 
        /// not be included in the list, if the date is left blank (or null) it will be as 
        /// if there is no lower bound specified and all items will be used.
        /// </summary>
        [N2.Details.EditableDate("Show Only Posts Published After", 80, Name = "StartDateFilter", ShowTime = false, ContainerName = EditModeTabs.Content)]
        public virtual DateTime? StartDateFilter
        {
            get { return (DateTime?)GetDetail("StartDateFilter"); }
            set { SetDetail<DateTime?>("StartDateFilter", value, null); }
        }

        /// <summary>
        /// The <see cref="DateTime" /> in which this list will stop displaying <see cref="CmsSyndicationFeedPage" /> items. 
        /// Any items published after this date will not be included in the list, if the date is left blank (or null) it will 
        /// be as if there is no upper bound specified and all items will be used.
        /// </summary>
        [N2.Details.EditableDate("Show Only Posts Published Before", 85, Name = "EndDateFilter", ShowTime = false, ContainerName = EditModeTabs.Content)]
        public virtual DateTime? EndDateFilter
        {
            get { return (DateTime?)GetDetail("EndDateFilter"); }
            set { SetDetail<DateTime?>("EndDateFilter", value, null); }
        }

        /// <summary>
        /// Gets and sets the Syndication Format of the feed (rss/atom/etc).
        /// </summary>
        [N2.Details.EditableEnum("Syndication Format", 90, typeof(SyndicationFormat), ContainerName = EditModeTabs.PageInformationName)]
        public virtual SyndicationFormat SyndicationFormat
        {
            get { return GetDetail<SyndicationFormat>("SyndicationFormat", SyndicationFormat.ATOM10); }
            set { SetDetail<SyndicationFormat>("SyndicationFormat", value, SyndicationFormat.ATOM10); }
        }

        #endregion

        #region Overridden Properties

        protected override string ResponseContentText
        {
            get
            {
                StringBuilder results = new StringBuilder();
                XmlWriterSettings xmlSettings = new XmlWriterSettings()
                {
                    Indent = false,
                    CloseOutput = true,
                    OmitXmlDeclaration = true
                };

                using (XmlWriter feedXml = XmlWriter.Create(results, xmlSettings))
                {
                    switch (this.SyndicationFormat)
                    {
                        case SyndicationFormat.RSS20:
                            this.Feed.SaveAsRss20(feedXml);
                            break;
                        case SyndicationFormat.ATOM10:
                        default:
                            this.Feed.SaveAsAtom10(feedXml);
                            break;
                    }
                }

                return results.ToString();
            }
        }

        protected override string ResponseContentType
        {
            get { return ContentTypes.TextXml; }
        }

        #endregion

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

        public SyndicationFeed Feed
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
