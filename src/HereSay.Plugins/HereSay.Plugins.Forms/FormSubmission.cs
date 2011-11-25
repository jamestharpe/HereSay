using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HereSay.Parts;
using N2.Collections;

namespace HereSay.Plugins.Forms
{
    [N2.PartDefinition(
        Title = "Form Submission",
        Name = "FormSubmission",
        IconUrl = "~/N2/Resources/icons/report.png"),
     N2.Integrity.RestrictParents(typeof(Form)),
     N2.Integrity.RestrictChildren(typeof(FormSubmission), typeof(FormSubmissionField))]
    public class FormSubmission : Part
    {

        [N2.Details.EditableChildren("Fields", HereSayZones.Data, 100)]
        public virtual IList<FormSubmissionField> Fields
        {
            get {
                return new ItemList<FormSubmissionField>(
                    this.Children, 
                    new AccessFilter(),
                    new TypeFilter(typeof(FormSubmissionField)));
            }
        }

        public IList<FormSubmission> ChildSubmissions
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
