using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using Rolcore.Web;
using System.Xml;
using N2.Details;
using System.Web.UI.WebControls;
using N2.Web.UI;

namespace HereSay.Pages
{
    /// <summary>
    /// Base class for creating pages that syndicate content via an XML feed such as RSS or ATOM.
    /// </summary>
    [WithEditableTitle("Feed Title", 10, ContainerName = EditModeTabs.PageInformationName, Required = true, RequiredMessage = "Feed Title is required."),
     WithEditableName("Path Segment", 20, ContainerName = EditModeTabs.PageInformationName, Required = true, RequiredMessage = "Path Segment is required."),
     TabContainer(
        EditModeTabs.PageInformationName,
        EditModeTabs.PageInformationTitle,
        EditModeTabs.PageInformationSortOrder)]
    public abstract class FeedPageBase : CustomTextContent
    {
        protected override string ResponseContentType
        {
            get { return ContentTypes.TextXml; }
        }

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

        protected int MaxNumberOfItemsToDisplayOrDefault
        {
            get
            {
                return (this.MaxNumberOfItemsToDisplay < 1)
                    ? int.MaxValue
                    : this.MaxNumberOfItemsToDisplay;
            }
        }

        #region Properties limiting which items get displayed
        /// <summary>
        /// Gets and sets the maximum number of articles to display. Use a negative number (e.g. 
        /// -1) to represent infinity.
        /// </summary>
        [EditableTextBox("Max # Items to Display (0=infinity)", 75, Validate = true, ValidationExpression = "^([-]|[0-9])[0-9]*$", ValidationMessage = "Must be an integer.", ContainerName = EditModeTabs.Content)]
        public virtual int MaxNumberOfItemsToDisplay
        {
            get { return GetDetail<int>("MaxNumberOfItemsToDisplay", 0); }
            set { SetDetail<int>("MaxNumberOfItemsToDisplay", value); }
        }

        /// <summary>
        /// The <see cref="DateTime?" /> in which this list will begin displaying 
        /// <see cref="CmsSyndicationFeedPage" /> items. Any items published before this date will 
        /// not be included in the list, if the date is left blank (or null) it will be as 
        /// if there is no lower bound specified and all items will be used.
        /// </summary>
        [EditableDate("Show Only Posts Published After", 80, Name = "StartDateFilter", ShowTime = false, ContainerName = EditModeTabs.Content)]
        public virtual DateTime? StartDateFilter
        {
            get { return GetDetail<DateTime?>("StartDateFilter", null); }
            set { SetDetail<DateTime?>("StartDateFilter", value, null); }
        }

        /// <summary>
        /// The <see cref="DateTime" /> in which this list will stop displaying <see cref="CmsSyndicationFeedPage" /> items. 
        /// Any items published after this date will not be included in the list, if the date is left blank (or null) it will 
        /// be as if there is no upper bound specified and all items will be used.
        /// </summary>
        [EditableDate("Only Show Posts Published Before", 85, Name = "EndDateFilter", ShowTime = false, ContainerName = EditModeTabs.Content)]
        public virtual DateTime? EndDateFilter
        {
            get { return GetDetail<DateTime?>("EndDateFilter", null); }
            set { SetDetail<DateTime?>("EndDateFilter", value, null); }
        }
        #endregion Properties limiting which items get displayed

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

        public abstract SyndicationFeed Feed { get; }

        /// <summary>
        /// Gets and sets the Syndication Format of the feed (rss/atom/etc).
        /// </summary>
        [EditableEnum("Syndication Format", 90, typeof(SyndicationFormat), ContainerName = EditModeTabs.PageInformationName)]
        public virtual SyndicationFormat SyndicationFormat
        {
            get { return GetDetail<SyndicationFormat>("SyndicationFormat", SyndicationFormat.ATOM10); }
            set { SetDetail<SyndicationFormat>("SyndicationFormat", value, SyndicationFormat.ATOM10); }
        }

        /// <summary>
        /// Gets and sets the Description content of the feed.
        /// </summary>
        [EditableTextBox("Description", 30, ContainerName = EditModeTabs.PageInformationName, TextMode = TextBoxMode.MultiLine)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Gets the description content of the feed. Use the <see cref="Text"/> property for 
        /// read/write access (this property is here to make finding the description easier).
        /// </summary>
        public string Description { get { return this.Text; } }
    }
}
