using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI.WebControls;
using Rolcore.Net.Mail;
using Rolcore.Web;

namespace HereSay.Plugins.Forms
{
    [N2.PartDefinition(
        Title = "E-Mail",
        Name = "FormEmailAction",
        IconUrl = "~/N2/Resources/icons/email_go.png")]
    public class FormEmailAction : FormAction
    {
        private const string VariableFormat = "[@{0}]";

        #region UI Properties

        [N2.Details.Editable("<strong>Variable Replacement</strong>", typeof(Literal), "Text", 0)]
        public virtual string HelpText
        {
            get
            {
                return "<p>Posted form values can be used in the To, From, Subject, and Message fields of this action, "
                    + "to do so simply wrap the <em>field_id</em> with <strong>[@]</strong>. For example, you would like "
                    + "to send an autoresponse to the person that submitted the form, assuming there is a field within "
                    + "the form with an ID of <em>user_email</em> you would set the \"To\" field of this action to <strong>"
                    + "[@<em>user_email</em>]</strong>. When the form is submitted that string will be replaced with "
                    + "value entered by the user for the <em>user_email</em> field, so remember to validate it!</p><br />";
            }
        }
        
        [N2.Details.EditableTextBox("To", 10, Required = true, RequiredText = "*",
            HelpText = "This is where the email will be sent.")]
        public virtual string Recipient
        {
            get { return GetDetail<string>("Recipient", String.Empty); }
            set { SetDetail<string>("Recipient", value, String.Empty); }
        }

        [N2.Details.EditableTextBox("CC", 11, Required = false, HelpText = "This is where the email will be CCed.")]
        public virtual string RecipientCC
        {
            get { return GetDetail<string>("RecipientCC", String.Empty); }
            set { SetDetail<string>("RecipientCC", value, String.Empty); }
        }

        [N2.Details.EditableTextBox("BCC", 11, Required = false, HelpText = "This is where the email will be BCCed.")]
        public virtual string RecipientBCC
        {
            get { return GetDetail<string>("RecipientBCC", String.Empty); }
            set { SetDetail<string>("RecipientBCC", value, String.Empty); }
        }

        [N2.Details.EditableTextBox("From", 20, Required = true, RequiredText = "*",
            HelpText = "Will appear as the sender when the email is delivered to the recipient.")]
        public virtual string Sender
        {
            get { return GetDetail<string>("Sender", String.Empty); }
            set { SetDetail<string>("Sender", value, String.Empty); }
        }

        [N2.Details.EditableTextBox("Subject", 30, Required = true, RequiredText = "*",
            HelpText = "Will appear as the Subject of the email when delivered.")]
        public virtual string Subject
        {
            get { return GetDetail<string>("Subject", String.Empty); }
            set { SetDetail<string>("Subject", value, String.Empty); }
        }

        [N2.Details.EditableFreeTextArea("Message", 50, Required = true, RequiredText = "*",
            HelpText = "Will appear as the email Body.")]
        public virtual string Message
        {
            get { return GetDetail<string>("Message", String.Empty); }
            set { SetDetail<string>("Message", value, String.Empty); }
        }

        #endregion

        public override void DoWork()
        {
            NameValueCollection requestFields = FormUtils.NameValueCollectionFromRequest(HttpContext.Current.Request);

            string recipientEmail = this.Recipient.ReplaceVariables(VariableFormat, requestFields);
            string recipientCCEmail = this.RecipientCC.ReplaceVariables(VariableFormat, requestFields);
            string recipientBCCEmail = this.RecipientBCC.ReplaceVariables(VariableFormat, requestFields);
            string senderEmail = this.Sender.ReplaceVariables(VariableFormat, requestFields);
            string subject = this.Subject.ReplaceVariables(VariableFormat, requestFields);
            string body = this.Message.ReplaceVariables(VariableFormat, requestFields);

            EmailUtils.SendEmail(
                EmailUtils.CreateMessage(
                    recipientEmail, recipientCCEmail, recipientBCCEmail,
                    senderEmail,
                    subject,
                    body, true));
        }
    }
}
