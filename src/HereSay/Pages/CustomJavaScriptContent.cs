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
        /// <summary>
        /// Gets a value specifying that the response content type should be text/javascript.
        /// </summary>
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

        /// <summary>
        /// Gets and sets the text (script) of the JavaScript. A compressed version of the content 
        /// is stored in ResponseContentText to reduce bandwidth utilization.
        /// </summary>
        [N2.Details.EditableTextBox("Script", 200, ContainerName = EditModeTabs.Content, TextMode = TextBoxMode.MultiLine, Rows = 30)]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                if(!string.IsNullOrWhiteSpace(this.Text))
                {
                    var compressor = new Yahoo.Yui.Compressor.JavaScriptCompressor();
                    string minifiedText = compressor.Compress(this.Text);
                    this.SetDetail<string>("ResponseContentText", minifiedText);
                }
            }
        }
    }
}
