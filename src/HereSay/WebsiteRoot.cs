using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Web;
using HereSay.Pages;

namespace HereSay
{
    /// <summary>
    /// The "root" of one or more websites. Contains global settings for all websites that belong 
    /// to it.
    /// </summary>
    [N2.Web.UI.TabContainer(EditModeTabs.LookAndFeelName, EditModeTabs.LookAndFeelTitle, 0),
     N2.PageDefinition(
        Title = "Website Root",
        Description = "This is the container for all websites belonging to this installation. It "
                    + "is not visible to visitors of your website.",
        SortOrder = PageSorting.First,
        InstallerVisibility = N2.Installation.InstallerHint.PreferredRootPage | N2.Installation.InstallerHint.NeverStartPage,
        IconUrl = "~/N2/Resources/icons/page_gear.png"),
     N2.Integrity.RestrictParents(N2.Integrity.AllowedTypes.None),
     N2.Integrity.RestrictChildren(typeof(HomePage), typeof(LanguageHomeRedirectPage), typeof(N2.Edit.Trash.ITrashCan))]
    public class WebsiteRoot : N2.ContentItem
    {
        public const string InstallRoot = "~/_hs/";

        #region Themes
        private string[] _AvailableThemes = null;

        public const string DefaultThemeName = "Boring";
        public const string ThemesRoot = WebsiteRoot.InstallRoot + "Themes/";

        public string[] AvailableThemes
        {
            get
            {
                if (this._AvailableThemes == null)
                {
                    HttpContext context = HttpContext.Current;
                    if (context == null)
                        throw new InvalidOperationException("HttpContext is not available, but is required.");

                    DirectoryInfo themesDirectory = new DirectoryInfo(context.Server.MapPath(ThemesRoot));
                    if (!themesDirectory.Exists)
                        throw new InstallationException(ThemesRoot 
                            + " must exist and contain at least one theme for this website to display.");

                    DirectoryInfo[] themesDirectories = themesDirectory.GetDirectories();

                    if (themesDirectories.Length == 0)
                        throw new InstallationException("At least one theme must be present in " + ThemesRoot);
                    
                    this._AvailableThemes = themesDirectories
                        .Select(themeDirectory => themeDirectory.Name)
                        .Where(themeName => themeName != ".svn") // Subversion work-around
                        .ToArray();
                }

                return this._AvailableThemes;
            }
        }

        /// <summary>
        /// Gets and sets the default theme name for all websites part of the current installation.
        /// If no value has been specified, the "Boring" theme is used as a default if available; 
        /// otherwise the first theme found is used.
        /// </summary>
        [Details.EditableDropDownList("Theme", 5, "AvailableThemes", ContainerName = EditModeTabs.LookAndFeelName)]
        public string ThemeName
        {
            get 
            {
                string result = this.GetDetail<string>("ThemeName", null);
                if (result == null)
                {
                    if (this.AvailableThemes.Contains(DefaultThemeName))
                        result = DefaultThemeName;
                    else
                        result = this.AvailableThemes[0];
                }
                return result; 
            }
            set { this.SetDetail<string>("ThemeName", value); }
        }
        #endregion Themes

        public override string TemplateUrl
        {
            get
            {
                return "~/_hs/WebsiteRoot.aspx"; //TODO: Password protect
            }
        }

        public static WebsiteRoot Current
        {
            get { return Find.RootItem; }
        }
    }
}