using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HereSay.Pages
{
    [N2.PageDefinition(
        Title = "Language-detector Home Page",
        Description = "For multi-language sites, create a Language-detector Home Page as the start "
                    + "page of your website to automatically detect your visitors language and re-"
                    + "direct them to the appropriate Langugage-specific Home Page.",
        IconUrl = "~/N2/Resources/icons/world_go.png",
        TemplateUrl = "~/RedirectPage.ashx",
        SortOrder = PageSorting.RareUse,
        InstallerVisibility = N2.Installation.InstallerHint.NeverRootPage
                            | N2.Installation.InstallerHint.PreferredStartPage),
     N2.Integrity.RestrictParents(typeof(WebsiteRoot))]
    public class LanguageHomeRedirectPage : RedirectPage
    {
        protected override string GetRedirectUrlPath()
        {
            return this.PreferredLanguageHomePageUrl;
        }

        [N2.Details.EditableUrl("Default destination", 40, Required = true, ContainerName = EditModeTabs.Content)]
        public override string RedirectUrl
        {
            get { return base.RedirectUrl; }
            set { base.RedirectUrl = value; }
        }

        /// <summary>
        /// Gets the URL of the preferred home page based off of the visitors browser language 
        /// preferences.
        /// </summary>
        public string PreferredLanguageHomePageUrl
        {
            get
            {
                return HttpContext.Current.GetPreferredLanguageHomePageUrl(this.RedirectUrl);
            }
        }
    }
}
