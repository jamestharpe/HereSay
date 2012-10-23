using System.Web.UI.WebControls;

namespace HereSay.Parts
{
    /// <summary>
    /// An HTML block on a page with a text editor.
    /// </summary>
    [N2.PartDefinition(
        "HTML (Code)",
        IconUrl = "~/N2/Resources/icons/html.png"),
     N2.Details.WithEditableTitle(Title = "Block Name", Required = false, HelpText = "This value is not shown on the page, it is just here to help you keep organized."),
     N2.Integrity.RestrictChildren(N2.Integrity.AllowedTypes.None)]
    public class HtmlBlock : AddablePart
    {
        protected override System.Web.UI.Control CreateViewControl()
        {
            return new Literal { Text = this.Html };
        }

        /// <summary>
        /// Gets and sets the HTML to be placed on the page.
        /// </summary>
        [N2.Details.EditableTextBox("Html", 100, TextMode = TextBoxMode.MultiLine, Rows = 20, Columns = 50)]
        public virtual string Html
        {
            get { return (string)GetDetail("Html"); }
            set { SetDetail("Html", value, string.Empty); }
        }
    }
}
