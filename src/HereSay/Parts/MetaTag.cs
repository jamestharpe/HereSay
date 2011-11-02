using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HereSay.Parts
{
    [N2.PartDefinition(
        Title = "Meta Tag",
        Name = "MetaTag",
        IconUrl = "~/N2/Resources/icons/tag_blue.png"),
     N2.Integrity.RestrictParents(typeof(HereSay.Pages.WebPage)),
     N2.Integrity.RestrictChildren(N2.Integrity.AllowedTypes.None),
     N2.Details.WithEditableTitle("Title", 0, Required = true, RequiredMessage = "Title is required")]
    public class MetaTag : N2.ContentItem
    {
        [N2.Details.EditableTextBox("Content", 10, Required = true, RequiredMessage = "Content is required")]
        public string Content
        {
            get { return GetDetail<string>("Content", String.Empty); }
            set { SetDetail<string>("Content", value, String.Empty); }
        }

        [N2.Details.EditableTextBox("http-equiv", 30)]
        public string HttpEquiv
        {
            get { return GetDetail<string>("HttpEquiv", String.Empty); }
            set { SetDetail<string>("HttpEquiv", value, String.Empty); }
        }

        [N2.Details.EditableTextBox("Name", 20)]
        public string TagName
        {
            // Do not use "WithEditableName" because name is not a required attribute for meta tags.
            get { return GetDetail<string>("TagName", String.Empty); }
            set { SetDetail<string>("TagName", value, String.Empty); }
        }

        [N2.Details.EditableTextBox("Scheme", 40)]
        public string Scheme
        {
            get { return GetDetail<string>("Scheme", String.Empty); }
            set { SetDetail<string>("Scheme", value, String.Empty); }
        }
    }
}
