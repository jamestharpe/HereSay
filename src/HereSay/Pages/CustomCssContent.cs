using System.Web.UI.WebControls;
using Rolcore.Web;

namespace HereSay.Pages
{
    [N2.PageDefinition(
        Title = "Custom CSS", 
        Description = "A custom CSS file. Output is automatically \"minified\" to optimize "
                    + "download speed.",
        SortOrder = PageSorting.InfrequentUse,
        TemplateUrl = CustomTextContent.HttpHandlerTemplateUrl,
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootOrStartPage,
        IconUrl = "~/N2/Resources/icons/css.png")]
    public class CustomCssContent : CustomTextContent
    {
        protected override string ResponseContentType
        {
            get { return ContentTypes.TextCss; }
        }

        protected override string ResponseContentText
        {
            get { return this.GetDetail<string>("ResponseContentText", this.Text); }
        }

        public override string Title
        {
            get { return this.Name; }
        }

        [N2.Details.EditableTextBox("CSS", 200, ContainerName = EditModeTabs.Content, TextMode = TextBoxMode.MultiLine, Rows = 30)]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                if(!string.IsNullOrWhiteSpace(this.Text))
                {
                    string minifiedText = Yahoo.Yui.Compressor.CssCompressor.Compress(this.Text);
                    this.SetDetail<string>("ResponseContentText", minifiedText);
                }
            }
        }
    }
}
