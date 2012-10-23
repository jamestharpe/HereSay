using N2.Edit.Workflow;
using HereSay.Pages;
using N2;
using System.Diagnostics;

namespace HereSay.Parts
{
    [N2.Web.UI.TabContainer(
         EditModeTabs.LookAndFeelName,
         EditModeTabs.LookAndFeelTitle,
         EditModeTabs.LookAndFeelSortOrder)]
    public abstract class ThemedWebPart : Part
    {
        private string 
            _TemplateUrl, 
            _ThemeRoot;

        private string[] _AvailableTemplates;

        protected virtual string ThemePartsRoot
        {
            get { return this.ThemeRoot + "Parts/"; }
        }

        protected virtual string ThemeRoot
        {
            get
            {
                if (this._ThemeRoot == null)
                {
                    var page = Find.FirstPublishedParent<WebPage>(this, true);
                    if (page != null)
                    {
                        _ThemeRoot = page.ThemeRoot;
                        Debug.WriteLine(string.Format("Theme Root for part '{0}': {1}", this.Name, _ThemeRoot));
                    }
                }

                return this._ThemeRoot;
            }
        }

        public const string
            PartTemplateFileExtension = ".ascx",
            PartTemplateDefaultName = "WebPart",
            PartTemplateSearchPatternFormat = "*{0}" + PartTemplateFileExtension;

        [Details.EditableDropDownList("Template", 5, "AvailableTemplates", ContainerName = EditModeTabs.LookAndFeelName)]
        public virtual string TemplateName
        {
            get { return this.GetDetail<string>("TemplateName", this.AvailableTemplates[0]); }
            set { this.SetDetail<string>("TemplateName", value, this.AvailableTemplates[0]); }
        }

        public override string TemplateUrl
        {
            get
            {
                if (this.State == ContentState.Deleted)
                    return base.TemplateUrl;

                if (this._TemplateUrl == null)
                    this._TemplateUrl = this.GetTemplateUrl(
                        this.ThemePartsRoot,
                        this.TemplateName,
                        PartTemplateFileExtension,
                        PartTemplateDefaultName);


                return _TemplateUrl;
            }
        }

        public string[] AvailableTemplates
        {
            get
            {
                if (this._AvailableTemplates == null)
                    this._AvailableTemplates = this.GetAvailableTemplates(
                        this.ThemePartsRoot,
                        PartTemplateSearchPatternFormat,
                        PartTemplateDefaultName);

                return this._AvailableTemplates;
            }
        }
    }
}
