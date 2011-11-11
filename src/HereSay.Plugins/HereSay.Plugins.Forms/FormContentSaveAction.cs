using System.Collections.Specialized;
using System.Web;
using System;

namespace HereSay.Plugins.Forms
{
    [N2.PartDefinition(
        Title = "Save as Content",
        Name = "FormContentSaveAction",
        IconUrl = "~/N2/Resources/icons/database_save.png")]
    public class FormContentSaveAction : FormAction
    {
        public override void DoWork()
        {
            NameValueCollection submissionValues = FormUtils.NameValueCollectionFromRequest(HttpContext.Current.Request);
            DateTime now = DateTime.Now;
            FormSubmission submission = new FormSubmission()
            {
                Name = string.Format("{0}_{1:yyyyMMddhhmmssfff}", this.Parent.Name, now),
                Title = string.Format("{0} {1}", this.Parent.Title, now),
                State = N2.Edit.Workflow.ContentState.Draft
            };
            submission.AddTo(this.Parent);
            N2.Context.Persister.Save(submission);
            for (int i = 0; i < submissionValues.Count; i++)
            {
                string key = submissionValues.AllKeys[i];
                string value = submissionValues[i];
                FormSubmissionField field = new FormSubmissionField() { Name = key, Title = value, ZoneName = "Items" };
                field.AddTo(submission);
                N2.Context.Persister.Save(field);
            }

            //TODO: Allow automatic publishing, e.g. for comments.
        }
    }
}
