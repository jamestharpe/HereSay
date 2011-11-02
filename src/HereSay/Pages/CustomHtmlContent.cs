using System.Web.UI.WebControls;
using Rolcore.Web.UI;
using Rolcore.Web;

namespace HereSay.Pages
{
    [N2.PageDefinition(
        "Custom HTML", 
        Name = "CustomHtmlContent", 
        Description = "Custom HTML content", 
        SortOrder = PageSorting.InfrequentUse,
        TemplateUrl = CustomTextContent.HttpHandlerTemplateUrl, 
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootPage,
        IconUrl = "~/N2/Resources/icons/page_code.png")]
    public class CustomHtmlContent : CustomTextContent
    {
        protected override string ResponseContentType
        {
            get { return ContentTypes.TextHtml; }
        }

        [N2.Details.EditableTextBox("HTML", 200, ContainerName = EditModeTabs.Content, TextMode = TextBoxMode.MultiLine, Rows=30)]
        public override string Text
        {
            get
            {
                string result = base.Text;
                if (string.IsNullOrEmpty(result))
                    result = "<html>\n<head>\n\t<title></title>\n</head>\n<body>\n\n</body>\n</html>";

                return result;
            }
            set
            {
                base.Text = value;
                //TODO: Minify into ResponseContentText
            }
        }
    }
}
