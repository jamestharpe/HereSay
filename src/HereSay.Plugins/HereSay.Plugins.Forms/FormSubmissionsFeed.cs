using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using HereSay.Details;
using HereSay.Pages;
using N2;
using N2.Collections;
using N2.Edit.Workflow;
using N2.Details;
using System;

namespace HereSay.Plugins.Forms
{
    [N2.PageDefinition(
        "Form Submission Feed",
        Name = "FormSubmissionsFeed",
        SortOrder = PageSorting.RareUse,
        Description = "Establishes a Feed of a specified type (RSS or ATOM) based on form submissions.",
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootOrStartPage,
        TemplateUrl = CustomTextContent.HttpHandlerTemplateUrl,
        IconUrl = "~/N2/Resources/icons/rss_feed.gif")]
    public class FormSubmissionsFeed : FeedPageBase
    {
        private IEnumerable<IFormSubmissionsProvider> _AvailableSubmissionProviders;
        private List<FormSubmission> _FeedSource;

        protected void AddSubmissionToFeedSource(FormSubmission submission, ref int submissionsAdded)
        {
            _FeedSource.Add(submission);
            submissionsAdded++;
            foreach (FormSubmission childSubmission in submission.ChildSubmissions)
            {
                if (submissionsAdded >= this.MaxNumberOfItemsToDisplayOrDefault)
                    break;
                AddSubmissionToFeedSource(childSubmission, ref submissionsAdded);
            }
        }

        public IEnumerable<IFormSubmissionsProvider> AvailableSubmissionProviders
        {
            get
            {
                if (_AvailableSubmissionProviders == null)
                {
                    _AvailableSubmissionProviders =
                        new ItemList<ContentItem>(
                            Find.Items.All.Select(),
                            new AccessFilter(),
                            new PublishedFilter(),
                            new TypeFilter(typeof(IFormSubmissionsProvider)))
                        .Select(item => item)
                        .Cast<IFormSubmissionsProvider>();
                }

                return _AvailableSubmissionProviders;
            }
        }

        [EditableDropDownList("Submissions Source", 100, "AvailableSubmissionProviders", "{$Name}", "{$Name}")]
        public string SubmissionsSource
        {
            get { return this.GetDetail<string>("SubmissionsSource", null); }
            set { this.SetDetail<string>("SubmissionsSource", value); }
        }

        [N2.Details.EditableTextBox("Item Title Format", 10, Required = true, RequiredText = "*",
            HelpText = "The format to use for the article title of each entry in the feed. Use "
                     + "[@<em>...</em>] to use a submitted value. For example "
                     + "[@<em>comment_title</em>] would be replaced with the value of the "
                     + "\"comment_title\" field in the submission.")]
        public string ItemTitleFormat
        {
            get { return this.GetDetail<string>("ItemTitleFormat", string.Empty); }
            set { this.SetDetail<string>("ItemTitleFormat", value); }
        }

        [N2.Details.EditableTextBox("Item Content Format", 11, Required = true, RequiredText = "*",
            HelpText = "The format to use for the content of each entry in the feed. Use "
                     + "[@<em>...</em>] to use a submitted value. For example "
                     + "[@<em>comment_message</em>] would be replaced with the value of the "
                     + "\"comment_message\" field in the submission.")]
        public string ItemContentFormat
        {
            get { return this.GetDetail<string>("ItemContentFormat", string.Empty); }
            set { this.SetDetail<string>("ItemContentFormat", value); }
        }

        public IEnumerable<FormSubmission> FeedSource
        {
            get
            {
                if (_FeedSource == null)
                {
                    IFormSubmissionsProvider submissionsSource = (IFormSubmissionsProvider)Find.Items
                        .Where
                            .Name.Eq(this.SubmissionsSource)
                            .And.State.Eq(ContentState.Published)
                        .Select<ContentItem>()
                        .FirstOrDefault();
                    _FeedSource = new List<FormSubmission>();
                    if (submissionsSource != null)
                    {
                        int submissionsAdded = 0;
                        foreach (FormSubmission submission in submissionsSource.Submissions)
                        {
                            if (submissionsAdded >= this.MaxNumberOfItemsToDisplayOrDefault)
                                break;
                            AddSubmissionToFeedSource(submission, ref submissionsAdded);
                        }
                    }
                }
                return _FeedSource;
            }
        }

        public override SyndicationFeed Feed
        {
            get 
            {
                List<SyndicationItem> feedItems = new List<SyndicationItem>();
                foreach (FormSubmission submission in this.FeedSource)
                {
                    SyndicationItem item = submission.ToSyndicationItem(
                        this.ItemTitleFormat,
                        this.ItemContentFormat,
                        FormUtils.VariableFormat);
                    feedItems.Add(item);
                }
                SyndicationFeed result = new SyndicationFeed(
                    this.Title,
                    this.Text,
                    new Uri(this.GetSafeParent().GetSafeUrl()),
                    feedItems);

                return result;
            }
        }
    }
}
