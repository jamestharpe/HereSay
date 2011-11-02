using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using HereSay.Pages;
using HereSay.Parts;

namespace HereSay.UI
{
     [ToolboxData("<{0}:MetaTags runat=\"server\"></{0}:MetaTags>")]
    public class MetaTags : Control
    {
         protected static void RenderMetaTags(HtmlTextWriter output, IEnumerable<MetaTag> metaTags)
         {
             foreach (MetaTag tag in metaTags)
             {
                 string content = string.Format(" content=\"{0}\"", tag.Content); // Content is required
                 string httpEquiv = !string.IsNullOrWhiteSpace(tag.HttpEquiv) ? string.Format(" http-equiv=\"{0}\"", tag.HttpEquiv) : string.Empty;
                 string name = !string.IsNullOrWhiteSpace(tag.TagName) ? string.Format(" name=\"{0}\"", tag.TagName) : string.Empty;
                 string scheme = !string.IsNullOrWhiteSpace(tag.Scheme) ? string.Format(" scheme=\"{0}\"", tag.Scheme) : string.Empty;
                 output.Write(string.Format("<meta{0}{1}{2}{3} />", httpEquiv, content, name, scheme));
             }
         }

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
            if (page != null)
            {
                if (page.MetaTagItems.Count > 0)
                {
                    MetaTags.RenderMetaTags(output, page.MetaTagItems);
                }
            }
        }
    }
}
