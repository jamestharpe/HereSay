using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Web.Parts;
using HereSay.Pages;
using HereSay;
using HereSay.UI;
using Rolcore.Web.UI;
using Rolcore;
using N2.Collections;

namespace HereSay.Plugins.Forms
{
    /// <summary>
    /// An HTML block on a page with a text editor.
    /// </summary>
    [N2.PartDefinition(
        Name = "Form",
        Title = "Form",
        IconUrl = "~/N2/Resources/icons/application_form.png"),
     N2.Web.UI.TabContainer(
        EditModeTabs.Content, 
        EditModeTabs.Content, 
        EditModeTabs.ContentSortOrder),
     N2.Web.UI.TabContainer(
        EditModeTabs.FormValidationName,
        EditModeTabs.FormValidationTitle,
        EditModeTabs.FormValidationSortOrder),
     N2.Web.UI.TabContainer(
        EditModeTabs.FormActionName,
        EditModeTabs.FormActionTitle,
        EditModeTabs.FormActionSortOrder),
     N2.Integrity.RestrictParents(typeof(WebPage)),
     N2.Integrity.RestrictChildren(typeof(FormAction), typeof(FormValidator)),
     N2.Details.WithEditableTitle(
         Title = "Form Name", 
         ContainerName = EditModeTabs.Content, Required = true,
         HelpText = "This value is not shown on the page, it is just here to help you keep organized.")]
    public class Form : N2.ContentItem, N2.Web.Parts.IAddablePart
        //
        // Does not inherit from AddablePart because access to container is needed to access the 
        // ASP.NET page, which is required to render server-side forms.
    {
        public const string FormIdFieldName = "__hsFormId";

        #region UI Properties

        [N2.Details.EditableFreeTextArea("Content", 200, ContainerName = EditModeTabs.Content)] //TODO: Validate <form>..</form> exists in content
        public virtual string FormHtml
        {
            get { return GetDetail<string>("FormHtml", "<form>&nbsp;</form>"); }
            set { SetDetail<string>("FormHtml", value); }
        }

        [N2.Details.EditableChildren("Validators", "ValidatorList", 100, ContainerName = EditModeTabs.FormValidationName)]
        public virtual IList<FormValidator> Validators
        {
            get
            {
                return new ItemList<FormValidator>(
                    this.Children,
                    new AccessFilter(),
                    new TypeFilter(typeof(FormValidator)));
            }
        }

        [N2.Details.EditableChildren("Actions", "ActionList", 20, ContainerName = EditModeTabs.FormActionName)]
        public virtual IList<FormAction> Actions
        {
            get
            {
                return new ItemList<FormAction>(
                  this.Children,
                  new AccessFilter(),
                  new TypeFilter(typeof(FormAction)));
            }
        }

        #endregion

        private bool ProcessFormSubmission(Control container)
        {
            bool result = true;
            // let's go ahead a process the form since we know we're in the correct "context"
            const string formValidationIdFormat = "ValidationId{0}";
            HtmlGenericControl validationMessageslistBox = new HtmlGenericControl("ul")
            {
                ID = String.Format(formValidationIdFormat, this.ID)
            };
            validationMessageslistBox.Attributes["class"] = "FormValidationErrors";

            int listItemCounter = 0;
            const string listItemIdFormat = "ItemId{0}";

            // run through any validators that have been created
            foreach(FormValidator validator in this.Validators)
            {
                result = result && validator.IsValid;
                if (!validator.IsValid)
                    validationMessageslistBox.Controls.Add(
                        new HtmlGenericControl("li")
                        {
                            ID = String.Format(listItemIdFormat, listItemCounter++),
                            InnerText = validator.Message
                        }
                    );
            };

            if (validationMessageslistBox.Controls.Count > 0)
                container.Controls.Add(validationMessageslistBox); //there are "error" messeges in the list so kick those back to the user
            else
                try
                {
                    foreach(FormAction action in this.Actions)
                        action.DoWork();
                }
                catch (Exception rootEx)
                {
                    //
                    // Capture any exceptions thrown by the "actions" performed and kick them back 
                    // to the user.

                    rootEx.ForEachInnerException(exception =>
                    {
                        validationMessageslistBox.Controls.Add(
                            new HtmlGenericControl("li")
                            {
                                InnerText = exception.Message,
                                ID = String.Format(listItemIdFormat, listItemCounter++)
                            }
                        );
                    });
                    container.Controls.Add(validationMessageslistBox);
                }
            return result;
        }

        public Control AddTo(Control container)
        {

            //
            // Check for submision

            string postedFormId = WebUtils.GetFormControlValueByFormControlId(
                FormIdFieldName,
                (HttpContext.Current.Request.RequestType == "POST")
                    ? HttpContext.Current.Request.Form
                    : HttpContext.Current.Request.QueryString);

            const string formIdFieldValueFormat = "hs_form{0}";

            string expectedFormId = String.Format(formIdFieldValueFormat, this.ID);

            //
            // Process submission, if detected

            bool formIsValid = (postedFormId == expectedFormId)
                ? ProcessFormSubmission(container)
                : true;

            //
            // Forms require a page to be available. Note, a redirect may have occured in 
            // ProcessFormSubmission, in which case we'll never make it here.

            Page currentPage = container.Page ?? (HttpContext.Current.Handler as Page);
            Control result;

            //
            // Build the form for display.

            if (currentPage == null)
            {
                result = new LiteralControl("[form]"); // This happens when a form gets syndicated
                container.Controls.Add(result);
                return result;
            }

            StringBuilder formHtmlBuilder = new StringBuilder(this.FormHtml);
            
            //
            // Hidden field to use as the form identifier

            HtmlInputHidden formIdHiddenFIeld = new HtmlInputHidden {
                ID = FormIdFieldName,
                Name = FormIdFieldName, 
                Value = expectedFormId };

            if (currentPage.ControlsOfType<HtmlForm>(true).Count() > 0)
            {
                //TODO: Consider http://htmlagilitypack.codeplex.com/ to mimic ASP.NET w/ out the
                //      requirement of only a single server-side form.

                formHtmlBuilder
                    .Replace("</form>", formIdHiddenFIeld.RenderControlToString() + "<!-- WARNING: Not runat=server --></form>");
                result = new LiteralControl(formHtmlBuilder.ToString());
                container.Controls.Add(result);
                return result;
            }

            //
            // We're injecting all instances of the form (and related) elements with runat=server 
            // so that we may run them through the ASP.NET parser and work with actual controls 
            // here on the server.

            formHtmlBuilder // TODO: What if they used <FORM> instead of <form>?
                .Replace("<form", "<form runat=\"server\"")
                .Replace("<input", "<input runat=\"server\"")
                .Replace("<select", "<select runat=\"server\"")
                .Replace("<textarea", "<textarea runat=\"server\"")

            //
            // The tiny_mce WYSIWYG editor is set on XHTML compliance, and thus automatically sets 
            // the "checked" and "selected" attributes on the input and option tags. They need to 
            // be set to "true" to work w/in the ASP.NET parser.

                .Replace("checked=\"checked\"", "checked=\"true\"")
                .Replace("selected=\"selected\"", "selected=\"true\"");

            result = currentPage.ParseControl(formHtmlBuilder.ToString());

            //
            // Hijack the form action to allow FormActions to handle submissions.

            HtmlForm htmlFormCtrl = result.ControlsOfType<HtmlForm>(recurse: true).Single();
            htmlFormCtrl.Action = this.GetSafeParent().GetSafeUrl();
            htmlFormCtrl.Controls.Add(formIdHiddenFIeld);

            // 
            // Only add children if the form is valid. Since this code calls the AddTo method 
            // recursively, any validation that has occured will get wiped out.

            if (formIsValid)
            {
                //
                // Have to add any child controls which are also IAddable. N2 doesn't do this by 
                // default.

                IEnumerable<IAddablePart> addableChildren = this.Children
                    .Where(child => child is IAddablePart)
                    .Cast<IAddablePart>();

                foreach (N2.Web.Parts.IAddablePart addableChild in addableChildren)
                    addableChild.AddTo(result);
            }

            result.ID = expectedFormId;
            container.Controls.Add(result);
            return result;
        }
    }
}
