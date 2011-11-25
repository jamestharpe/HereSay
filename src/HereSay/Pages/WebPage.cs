using System.Collections.Generic;
using HereSay.Parts;
using System.Linq;
using N2.Collections;
using HereSay.Details;

namespace HereSay.Pages
{
    [N2.PageDefinition(
        Title = "Web Page",
        SortOrder = PageSorting.ExtremelyFrequentUse,
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootOrStartPage,
        IconUrl = "~/N2/Resources/icons/page.png"),
     N2.Web.UI.TabContainer(
         EditModeTabs.PageInformationName, 
         EditModeTabs.PageInformationTitle, 
         EditModeTabs.PageInformationSortOrder),
     N2.Web.UI.TabContainer(
         EditModeTabs.Navigation,
         EditModeTabs.Navigation,
         EditModeTabs.NavigationSortOrder), 
     N2.Web.UI.TabContainer(
         EditModeTabs.LookAndFeelName,
         EditModeTabs.LookAndFeelTitle,
         EditModeTabs.LookAndFeelSortOrder),
     N2.Web.UI.TabContainer(
         EditModeTabs.Advanced,
         EditModeTabs.Advanced,
         EditModeTabs.AdvancedSortOrder), 
     N2.Details.WithEditableName("Path Segment", 20, ContainerName = EditModeTabs.Navigation),
     N2.Details.WithEditableTitle("Label", 15, ContainerName = EditModeTabs.Navigation)]
    public class WebPage : N2.ContentItem
    {
        #region Themes and Templates
        
        private string  _TemplateUrl = null, _ThemeName = null, _ThemeRoot = null;
        private string[] _AvailableTemplates = null;

        protected string ThemePagesRoot
        {
            get { return this.ThemeRoot + "Pages/"; }
        }

        protected internal string ThemeRoot
        {
            get
            {
                if (this._ThemeRoot == null)
                    _ThemeRoot = string.Format("{0}{1}/", WebsiteRoot.ThemesRoot, this.ThemeName);

                return this._ThemeRoot;
            }
        }

        public const string 
            PageTemplateFileExtension = ".aspx",
            PageTemplateDefaultName = "WebPage",
            PageTemplateSearchPatternFormat = "*{0}" + PageTemplateFileExtension;

        [EditableDropDownList("Template", 5, "AvailableTemplates", ContainerName = EditModeTabs.LookAndFeelName)]
        public virtual string TemplateName
        {
            get { return this.GetDetail<string>("TemplateName", this.AvailableTemplates[0]); }
            set { this.SetDetail<string>("TemplateName", value, this.AvailableTemplates[0]); }
        }

        public override string TemplateUrl
        {
            get
            {
                if (this._TemplateUrl == null)
                    this._TemplateUrl = this.GetTemplateUrl(
                           this.ThemePagesRoot,
                           this.TemplateName,
                           PageTemplateFileExtension,
                           PageTemplateDefaultName);

                return _TemplateUrl;
            }
        }

        /// <summary>
        /// Gets and sets the theme name for the current web page. If no value has been set the 
        /// value from the parent is returned.
        /// </summary>
        [EditableDropDownList("Theme", 5, "AvailableThemes", ContainerName = EditModeTabs.LookAndFeelName)]
        public virtual string ThemeName
        {
            get
            {
                if (_ThemeName == null)
                {
                    // _ThemeName is stored to prevent having to crawl up the parent chain each 
                    // time when the default is in use.
                    _ThemeName = this.GetDetail<string>("Theme", null);
                    if (_ThemeName == null)
                    {
                        WebPage parent = this.GetSafeParent() as WebPage;
                        _ThemeName = (parent != null) ? parent.ThemeName : WebsiteRoot.Current.ThemeName;
                    }
                }
                return _ThemeName;
            }
            set
            {
                WebPage parent = this.GetSafeParent() as WebPage;
                string defaultValue = (parent != null) ? parent.ThemeName : WebsiteRoot.Current.ThemeName;

                if (value == defaultValue)
                    this.SetDetail<string>("Theme", null); // Force changes in parent to cascade
                else
                    this.SetDetail<string>("Theme", value);

                _ThemeName = value;
                _ThemeRoot = null; // Force re-eval
            }
        }

        public string[] AvailableThemes
        {
            get { return WebsiteRoot.Current.AvailableThemes; }
        }

        public string[] AvailableTemplates
        {
            get
            {
                if (this._AvailableTemplates == null)
                    this._AvailableTemplates = this.GetAvailableTemplates(
                        this.ThemePagesRoot,
                        PageTemplateSearchPatternFormat,
                        PageTemplateDefaultName);

                return this._AvailableTemplates;
            }
        }
        #endregion Themes and Templates

        #region Page Information
        private string _SafeUrl = null;
        public string SafeUrl
        {
            get
            {
                if (this._SafeUrl == null) this._SafeUrl = this.GetSafeUrl();
                return this._SafeUrl;
            }
        }

        /// <summary>
        /// Gets and sets the canonical URL for the page.
        /// <seealso cref="http://googlewebmastercentral.blogspot.com/2009/02/specify-your-canonical.html"/>
        /// <seealso cref="http://www.google.com/support/webmasters/bin/answer.py?hl=en&answer=139394"/>
        /// </summary>
        [N2.Details.EditableTextBox("Canonical URL", 50, ContainerName = EditModeTabs.PageInformationName)]
        public virtual string CanonicalUrl
        {
            get
            {
                string result = GetDetail<string>("CanonicalUrl", null);
                return string.IsNullOrEmpty(result) 
                    ? this.SafeUrl 
                    : result;
            }
            set
            {
                // For some reason when saving a new item this.SafeUrl and this.Url return the 
                // parent item's URL instead of the current items URL. To ensure the correct 
                // default is used, we have to check for this. Not sure how to handle the parent
                // actually being the desired canonical value.
                // TODO: Revisit after N2 2.0 upgrade
                string safeParentUrl = null;
                WebPage parent = this.GetSafeParent() as WebPage;
                if (parent != null)
                    safeParentUrl = parent.SafeUrl;
                if ((value != this.CanonicalUrl) && (value != this.SafeUrl) && (value != safeParentUrl))
                    SetDetail<string>("CanonicalUrl", value);
                else if (value == this.SafeUrl || value == safeParentUrl) // Case 2984
                    SetDetail<string>("CanonicalUrl", null, null); // Force default

            }
        }

        /// <summary>
        /// Gets and sets the value to  be used when a title tag is rendered.
        /// </summary>
        [N2.Details.EditableTextBox("Page Title", 10, ContainerName = EditModeTabs.PageInformationName)]
        public virtual string PageTitle
        {
            get { return (string)(GetDetail("PageTitle") ?? this.Title); }
            set { SetDetail("PageTitle", value, string.Empty); }
        }

        /// <summary>
        /// Gets and sets the value to  be used when a keywords meta tag is rendered.
        /// </summary>
        [N2.Details.EditableTextBox("Keywords", 20, ContainerName = EditModeTabs.PageInformationName, TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine)]
        public virtual string MetaKeywords
        {
            get { return (string)(GetDetail("MetaKeywords") ?? string.Empty); }
            set { SetDetail("MetaKeywords", value, string.Empty); }
        }

        /// <summary>
        /// Gets and sets the value to be used when a description meta tag is rendered.
        /// </summary>
        [N2.Details.EditableTextBox("Description", 25, ContainerName = EditModeTabs.PageInformationName, TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine)]
        public virtual string MetaDescription
        {
            get { return (string)(GetDetail("MetaDescription") ?? string.Empty); }
            set { SetDetail("MetaDescription", value, string.Empty); }
        }

        [N2.Details.EditableChildren("Meta Tag Items", "MetaTagItems", 100, ContainerName = EditModeTabs.Advanced, ZoneName=HereSayZones.MetaTags)]
        public virtual IList<MetaTag> MetaTagItems
        {
            get
            {
                return new ItemList<MetaTag>(
                    this.Children,
                    new AccessFilter(),
                    new TypeFilter(typeof(MetaTag)));
            }
        }

        #endregion Page Information
    }
}
