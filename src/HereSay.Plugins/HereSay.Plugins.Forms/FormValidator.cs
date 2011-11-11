using System;

namespace HereSay.Plugins.Forms
{
    [N2.Integrity.RestrictParents(typeof(Form)),
     N2.Details.WithEditableTitle("Field Id", 10, Required = true, RequiredText = "*")]
    public abstract class FormValidator : N2.ContentItem
    {
        public abstract bool IsValid { get; }

        [N2.Details.EditableTextBox("Message", 100, Required = true, RequiredText = "*")]
        public virtual string Message
        {
            get { return GetDetail<string>("Message", String.Empty); }
            set { SetDetail<string>("Message", value, String.Empty); }
        }
    }
}
