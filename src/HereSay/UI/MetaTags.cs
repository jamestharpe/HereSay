using System.Web;
using System.Web.UI;
using HereSay.Pages;
using HereSay.Parts;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Engine.Globalization;

namespace HereSay.UI
{
    [ToolboxData("<{0}:MetaTags runat=\"server\"></{0}:MetaTags>")]
    public class MetaTags : Control
    {
        /// <summary>
        /// Forces ViewState to be disabled.
        /// </summary>
        public override bool EnableViewState
        {
            get { return false; }
        }

        protected override void Render(HtmlTextWriter output)
        {
            WebPage page = N2.Context.CurrentPage as WebPage;
            if (page == null)
                return;

            ILanguage pageLanguage = page.GetLanguage();
            MetaTag languageMetaTag = (pageLanguage != null)
                ? new MetaTag() { HttpEquiv = "Content-Language", Content = pageLanguage.LanguageCode }
                : null;

            IEnumerable<MetaTag> tags = (new MetaTag[]{
                                            new MetaTag() { TagName = "keywords", Content = page.MetaKeywords },
                                            new MetaTag() { TagName = "description", Content = page.MetaDescription } })
                                        .Union(page.MetaTagItems);

            ContentPlaceHolder container = new ContentPlaceHolder();
            foreach (MetaTag tag in tags)
                tag.AddTo(container);

            if (languageMetaTag != null)
                languageMetaTag.AddTo(container);

            container.RenderControl(output);
        }
    }
}
