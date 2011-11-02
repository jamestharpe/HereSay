
namespace HereSay.Parts
{
    /// <summary>
    /// An HTML block on a page with a text editor.
    /// </summary>
    [N2.PartDefinition(
        "HTML (WYSIWYG)",
        IconUrl = "~/N2/Resources/icons/html_go.png"),
     N2.Details.WithEditableTitle(Title = "Block Name", Required = false, HelpText = "This value is not shown on the page, it is just here to help you keep organized.")]
    public class HtmlContentBlock : HtmlBlock
    {
        [N2.Details.EditableFreeTextArea("Content", 200)]
        public override string Html
        {
            get { return base.Html; }
            set { base.Html = value; }
        }
    }
}
