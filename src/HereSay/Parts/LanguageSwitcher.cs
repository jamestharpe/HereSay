using System.Collections.Generic;
using System.Web;
using N2;
using N2.Collections;
using N2.Engine.Globalization;
using HereSay.Globalization;
using HereSay.Pages;

namespace HereSay.Parts
{
    [N2.PartDefinition(
         Name = "LanguageSwitcher", // compatability issue with legacy items
         Title = "Language Switcher",
         Description = "Switch languages for current page if available",
         IconUrl = "~/N2/Resources/icons/rss.png"),
     N2.Web.UI.TabContainer(
         EditModeTabs.Content,
         EditModeTabs.Content,
         EditModeTabs.ContentSortOrder),
     N2.Integrity.RestrictParents(typeof(WebPage)),
     N2.Integrity.RestrictChildren(N2.Integrity.AllowedTypes.None),
     N2.Details.WithEditableTitle(
         Title = "Display Name",
         ContainerName = EditModeTabs.Content,
         HelpText = "This value is not shown on the page, it is just here to help you keep organized.",
         Required = true, RequiredMessage = "Display Name is required.")]
    public class LanguageSwitcher : ThemedWebPart
    {
        protected List<ContentTranslation> _Translations;
        
        protected static ILanguageGateway LanguageGateway
        {
            get
            {
                ILanguageGateway result = HttpContext.Current.Items["LanguageGateway"] as ILanguageGateway;
                if (result == null)
                {
                    result = N2.Context.Current.Resolve<ILanguageGateway>();
                    HttpContext.Current.Items["LanguageGateway"] = result;
                }
                return result;
            }
        }

        public ILanguage CurrentLanguage
        {
            get { return this.Page.GetLanguage(); }
        }

        public IEnumerable<ContentTranslation> Translations
        {
            get { return this.Page.GetTranslations(); }
        }
    }
}