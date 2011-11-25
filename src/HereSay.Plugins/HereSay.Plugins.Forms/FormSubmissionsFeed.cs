using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HereSay.Pages;
using System.ServiceModel.Syndication;
using N2.Collections;
using N2;
using HereSay.Details;
using N2.Edit.Workflow;

namespace HereSay.Plugins.Forms
{
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

        [EditableDropDownList("Submissions Source", 100, "AvailableSubmissionProviders", "Name", "Name", Required=true)]
        public string SubmissionsSource
        {
            get { return this.GetDetail<string>("SubmissionsSource", null); }
            set { this.SetDetail<string>("SubmissionsSource", value); }
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
                    feedItems.Add(submission.ToSyndicationItem());
                }
            }
        }
    }
}
