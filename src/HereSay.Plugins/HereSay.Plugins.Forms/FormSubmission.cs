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
     N2.Web.UI.TabContainer(
        EditModeTabs.Content,
        EditModeTabs.Content,
        EditModeTabs.ContentSortOrder),
     N2.Integrity.RestrictParents(typeof(Form)),
     N2.Integrity.RestrictChildren(typeof(FormSubmission), typeof(FormSubmissionField))]
    public class FormSubmission : Part
    {

        [N2.Details.EditableChildren("Items", "Items", 100, ContainerName = EditModeTabs.Content)]
        public virtual IList<FormSubmissionField> Items
        {
            get {
                return new ItemList<FormSubmissionField>(
                    this.Children, 
                    new AccessFilter(),
                    new TypeFilter(typeof(FormSubmissionField)));
            }
        }
    }
}
