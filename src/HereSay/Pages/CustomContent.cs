using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HereSay.Pages
{
    [N2.PageDefinition(
        "Custom Text Content", 
        Name="CustomContent", 
        Description="Custom text-based content", SortOrder = PageSorting.InfrequentUse,
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootOrStartPage,
        TemplateUrl = CustomTextContent.HttpHandlerTemplateUrl,
        IconUrl = "~/N2/Resources/icons/page_white_text.png")]
    public class CustomContent : CustomTextContent
    {
        [N2.Details.EditableTextBox("Content Type", 200, ContainerName = EditModeTabs.Content)]
        public string ContentType
        {
            get { return base.ResponseContentType; }
            set { base.ResponseContentType = value; }
        }
    }
}
