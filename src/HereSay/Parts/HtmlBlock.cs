using System.Web.UI.WebControls;

namespace HereSay.Parts
{
    /// <summary>
    /// An HTML block on a page with a text editor.
    /// </summary>
    [N2.PartDefinition(
        "HTML (Code)",
        IconUrl = "~/N2/Resources/icons/html.png"),
     N2.Details.WithEditableTitle(Title = "Block Name", Required = false, HelpText = "This value is not shown on the page, it is just here to help you keep organized.")]
    public class HtmlBlock : N2.ContentItem, N2.Web.Parts.IAddablePart
    {
        /// <summary>
        /// Gets and sets the HTML to be placed on the page.
        /// </summary>
        [N2.Details.EditableTextBox("Html", 100, TextMode = TextBoxMode.MultiLine, Rows = 30, Columns = 50)]
        public virtual string Html
        {
            get { return (string)GetDetail("Html"); }
            set { SetDetail("Html", value, string.Empty); }
        }

        #region IAddablePart Members

        public System.Web.UI.Control AddTo(System.Web.UI.Control container)
        {
            Literal result = new Literal();
            result.Text = this.Html;
            container.Controls.Add(result);
            return result;
        }

        #endregion
    }
}
