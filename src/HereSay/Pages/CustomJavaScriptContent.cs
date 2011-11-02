using System.Web.UI.WebControls;
using Rolcore.Web;

namespace HereSay.Pages
{
    [N2.PageDefinition(
        "Custom JavaScript", 
        Name="CustomJavaScriptContent", 
        Description="Custom JavaScript content",
        SortOrder = PageSorting.InfrequentUse,
        TemplateUrl = CustomTextContent.HttpHandlerTemplateUrl,
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootOrStartPage,
        IconUrl = "~/N2/Resources/icons/script.png")]
    public class CustomJavaScriptContent : CustomTextContent
    {
        protected override string ResponseContentType
        {
            get { return ContentTypes.TextJavaScript; }
        }

        protected override string ResponseContentText
        {
            get
            {
                return this.GetDetail<string>("ResponseContentText", this.Text);
            }
        }

        [N2.Details.EditableTextBox("Script", 200, ContainerName = EditModeTabs.Content, TextMode = TextBoxMode.MultiLine, Rows = 30)]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                if(!string.IsNullOrWhiteSpace(this.Text))
                {
                    string minifiedText = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(this.Text);
                    this.SetDetail<string>("ResponseContentText", minifiedText);
                }
            }
        }
    }
}
