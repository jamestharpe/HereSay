using System.Web.UI.WebControls;

namespace HereSay
{
    [N2.Web.UI.TabContainer(EditModeTabs.Content, EditModeTabs.Content, EditModeTabs.ContentSortOrder),
     N2.Details.WithEditableName("Name", 20, ContainerName = EditModeTabs.Content),]
    public class CustomTextItem : N2.ContentItem
    {
        /// <summary>
        /// Gets and sets the text content of the item.
        /// </summary>
        [N2.Details.EditableTextBox("Text", 200, ContainerName = EditModeTabs.Content, TextMode = TextBoxMode.MultiLine)]
        public virtual string Text
        {
            get { return GetDetail<string>("Text", string.Empty); }
            set { SetDetail<string>("Text", value, string.Empty); }
        }

        public override string Title
        {
            get { return string.IsNullOrWhiteSpace(base.Title) ? this.Name : base.Title; }
        }

    }
}