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

        public static ILanguage CurrentLanguage
        {
            get
            {
                ILanguage result = HttpContext.Current.Items["CurrentLanguage"] as ILanguage;
                if (result == null)
                {
                    result = LanguageGateway.GetLanguage(N2.Context.CurrentPage);
                    HttpContext.Current.Items["CurrentLanguage"] = result;
                }
                return result;
            }
        }

        public List<ContentTranslation> Translations
        {
            get
            {
                if ((_Translations == null) && (LanguageGateway.Enabled))
                {
                    _Translations = new List<ContentTranslation>();

                    using (ItemFilter languageFilter = new AllFilter(new AccessFilter(), new PublishedFilter()))
                    {
                        IEnumerable<ContentItem> translationItems = LanguageGateway.FindTranslations(N2.Context.CurrentPage);
                        foreach (ContentItem translation in languageFilter.Pipe(translationItems))
                        {
                            ILanguage language = LanguageGateway.GetLanguage(translation);
                            // Hide translations when filtered access to their language
                            ContentItem languageItem = language as ContentItem;
                            if (languageItem == null || languageFilter.Match(languageItem))
                                _Translations.Add(new ContentTranslation(translation, language));
                        }
                    }
                }
                return _Translations;
            }
        }
    }
}