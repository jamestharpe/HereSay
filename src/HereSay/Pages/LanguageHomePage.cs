using System.Globalization;
using N2.Engine.Globalization;

namespace HereSay.Pages
{
    [N2.PageDefinition(
        Title = "Language-specific Home Page",
        Description = "For multi-language sites, create one Language-specific Home Page for each "
                    + "language that needs to be supported. All content in that language should "
                    + "then be managed beneith the Language-specifc Home Page page.",
        IconUrl = "~/N2/Resources/icons/page_world.png",
        SortOrder = PageSorting.RareUse,
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootPage 
                            | N2.Installation.InstallerHint.PreferredStartPage),
     N2.Integrity.RestrictParents(typeof(WebsiteRoot), typeof(HomePage), typeof(LanguageHomeRedirectPage))]
    public class LanguageHomePage : HomePage, ILanguage
    {
        #region ILanguage Members
        public string FlagUrl
        {
            get
            {
                if (string.IsNullOrEmpty(LanguageCode))
                    return string.Empty;
                else
                {
                    string[] parts = LanguageCode.Split('-');
                    return string.Format("~/N2/Resources/img/Globalization/flags/{0}.png", parts[parts.Length - 1]);
                }
            }
        }

        [N2.Details.EditableLanguagesDropDown("Language", 100, ContainerName = EditModeTabs.PageInformationName)]
        public virtual string LanguageCode
        {
            get { return GetDetail<string>("LanguageCode", null); }
            set 
            { 
                SetDetail("LanguageCode", value);
                if (!string.IsNullOrEmpty(value))
                {
                    string languageTitle = (new CultureInfo(LanguageCode)).DisplayName;
                    SetDetail<string>("LanguageTitle", languageTitle);
                }
                else
                {
                    SetDetail<string>("LanguageTitle", null);
                }
            }
        }

        public string LanguageTitle
        {
            get { return GetDetail<string>("LanguageCode", string.Empty); }
        }
        #endregion
    }
}
