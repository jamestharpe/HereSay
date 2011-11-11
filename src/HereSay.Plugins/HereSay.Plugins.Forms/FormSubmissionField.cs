using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HereSay.Parts;
using N2.Integrity;
using N2;
using N2.Details;

namespace HereSay.Plugins.Forms
{
    [PartDefinition(
        Title = "Form Submission Field",
        Name = "FormSubmissionField",
        IconUrl = "~/N2/Resources/icons/textfield.png"),
     RestrictParents(typeof(FormSubmission)),
     RestrictChildren(AllowedTypes.None),
     WithEditableName("Name", 100),
     WithEditableTitle("Value", 200)]
    public class FormSubmissionField : Part
    {
    }
}
