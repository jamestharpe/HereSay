using System;
using System.ServiceModel.Syndication;
using System.Xml;
using HereSay.Pages;
using Rolcore.Web.Syndication;
using Rolcore;
using N2;
using N2.Web.UI;
using N2.Details;
using System.Text;

namespace HereSay.Parts
{
    [PartDefinition(
        Name = "SyndicatedContent", // compatability issue with legacy items
        Title = "Syndicated Content",
        IconUrl = "~/N2/Resources/icons/rss.png"),
     TabContainer(
         EditModeTabs.Content,
         EditModeTabs.Content,
         EditModeTabs.ContentSortOrder),
     N2.Integrity.RestrictParents(typeof(WebPage)),
     N2.Integrity.RestrictChildren(N2.Integrity.AllowedTypes.None),
     WithEditableTitle(
         Title = "Display Name",
         ContainerName = EditModeTabs.Content,
         HelpText = "This value is not shown on the page, it is just here to help you keep organized.",
         Required = true, RequiredMessage = "Display Name is required.")]
    public class SyndicatedContent : ThemedWebPart
    {
        private SyndicationFeed _Feed;

        /// <summary>
        /// Gets and sets the Url for the Feed to be used within the display. 
        /// Can be an absolute or relative Url.
        /// </summary>
        [EditableUrl("Feed Url", 5,
            ContainerName = EditModeTabs.Content, 
            AvailableModes = N2.Web.UI.WebControls.UrlSelectorMode.Items,
            OpeningMode = N2.Web.UI. WebControls.UrlSelectorMode.Items,
            RelativeTo = N2.Web.UI.WebControls.UrlRelativityMode.Absolute,
            RelativeWhen = RelativityMode.ImportingOrExporting,
            Required = true, RequiredMessage = "Feed Url is required.", 
            HelpText = "This MUST be a URL to an ATOM or RSS feed.")]
        public virtual string FeedUrl
        {
            get { return GetDetail<string>("FeedUrl", String.Empty); }
            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Feed URL must have a value.");

                //
                // Calc the fully qualified URI.

                // I dont think this shouldn't be here for 2 reasons
                // 1. it will store the fully qualified URL in the database, 
                //    which isn't something we want to do.
                // 2. It wont allow feed
                //Uri valueUri = value.ToUri(System.Web.HttpContext.Current.GetSiteBaseUrl());

                SetDetail<string>("FeedUrl", value);
            }
        }

        /// <summary>
        /// Gets and sets the maximum number of articles to display. Use zero to represent 
        /// infinity.
        /// </summary>
        [EditableTextBox("Max Items to Display (0=infinity)", 15, 
            ContainerName = EditModeTabs.Content,
            ValidationExpression = "^([-]|[0-9])[0-9]*$", 
            ValidationMessage = "Must be an integer.",
            Validate = true)]
        public virtual int DisplayableItemCount
        {
            get { return GetDetail<int>("DisplayableItemCount", -1); }
            set { SetDetail<int>("DisplayableItemCount", value, -1); }
        }
        
        public virtual SyndicationFeed Feed
        {
            get
            {
                //
                // Only do work if necessary

                string feedUrl = this.FeedUrl;

                if (_Feed == null && !String.IsNullOrWhiteSpace(feedUrl))
                {
                    FeedPageBase feedPage;
                    try
                    {
                        feedPage = Find.ByUrl<FeedPageBase>(feedUrl);
                    }
                    catch (System.InvalidCastException)
                    {
                        return new SyndicationFeed(
                            "Invalid feed URL", 
                            string.Empty, 
                            this.FeedUrl.ToUri(),
                            new SyndicationItem[]{ 
                                new SyndicationItem("Error loading feed", string.Empty, feedUrl.ToUri()) { 
                                    Summary = new TextSyndicationContent(string.Format("The Feed URL specified does not point to a valid syndication feed ({0}).", feedUrl)) 
                                } 
                            });
                    }
                    if (feedPage != null)
                        _Feed = (DisplayableItemCount > 0)
                            ? feedPage.Feed.Reduce(DisplayableItemCount)
                            : feedPage.Feed;
                    else
                    {
                        //
                        // Read the feed

                        try
                        {
                            int displayableItemCount = this.DisplayableItemCount;
                            using (XmlReader feedReader = XmlReader.Create(feedUrl)) //TODO: Allow caching
                            {
                                // The XmlReader will not work w/out a modification to the web.config 
                                // file's /configuration/system.net/defaultProxy/proxy settings. See 
                                // the following url for details:
                                // http://west-wind.com/weblog/posts/3871.aspx

                                _Feed = (displayableItemCount > 0)
                                    ? SyndicationFeed.Load(feedReader).Reduce(displayableItemCount)
                                    : SyndicationFeed.Load(feedReader);

                                feedReader.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            return new SyndicationFeed(
                                "Error loading feed",
                                ex.Message,
                                this.FeedUrl.ToUri(),
                                new SyndicationItem[]{ 
                                    new SyndicationItem(
                                        "Error loading feed", 
                                        ex.StackTrace, 
                                        feedUrl.ToUri()) { 
                                        Summary = new TextSyndicationContent(ex.Message) 
                                    } 
                                });
                        }
                    }
                }

                return _Feed;                
            }
        }

        public string GetSummary(SyndicationItem item)
        {
            return (item.Summary != null)
                ? item.Summary.Text
                : string.Empty;
        }

        public string GetContent(SyndicationItem item)
        {
            StringBuilder content = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(content);
            item.Content.WriteTo(writer, "span", null);
            writer.Flush();
            return content.ToString();
        }
    }
}
