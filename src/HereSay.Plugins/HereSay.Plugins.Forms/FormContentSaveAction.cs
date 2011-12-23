using System.Collections.Specialized;
using System.Web;
using System;
using N2.Edit.Workflow;
using N2;
using N2.Integrity;
using System.Collections.Generic;
using N2.Collections;
using N2.Details;

namespace HereSay.Plugins.Forms
{
    [PartDefinition(
        Title = "Save as Content",
        Name = "FormContentSaveAction",
        IconUrl = "~/N2/Resources/icons/database_save.png"),
     RestrictChildren(typeof(FormSubmission)),
     WithEditableName("Name", 20)]
    public class FormContentSaveAction : FormAction, IFormSubmissionsProvider
    {
        public override void DoWork()
        {
            NameValueCollection submissionValues = FormUtils.NameValueCollectionFromRequest(HttpContext.Current.Request);
            DateTime now = DateTime.Now;
            FormSubmission submission = new FormSubmission()
            {
                Name = string.Format("{0}_{1:yyyyMMddhhmmssfff}{2}", this.Parent.Name, now, Rolcore.ThreadSafeRandom.Next()),
                Title = string.Format("{0} {1}", this.Parent.Title, now),
                ZoneName = HereSayZones.Data,
                Published = null,
                State = ContentState.Draft
            };
            submission.AddTo(this);
            Context.Persister.Save(submission);
            for (int i = 0; i < submissionValues.Count; i++)
            {
                string key = submissionValues.AllKeys[i];
                string value = submissionValues[i];
                FormSubmissionField field = new FormSubmissionField()
                {
                    Name = key,
                    Title = value,
                    ZoneName = HereSayZones.Data,
                    Published = null,
                    State = ContentState.Draft
                };
                field.AddTo(submission);
                Context.Persister.Save(field);
            }

            //TODO: Allow automatic publishing, e.g. for comments.
        }

        [N2.Details.EditableChildren("Submissions", HereSayZones.Data, 100)]
        public virtual IList<FormSubmission> Submissions
        {
            get
            {
                return new ItemList<FormSubmission>(
                    this.Children,
                    new AccessFilter(),
                    new TypeFilter(typeof(FormSubmission)));
            }
        }
    }
}
