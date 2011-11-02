using System.Web;
using System.Web.UI;
using HereSay.Pages;
using System;

namespace HereSay.UI
{
    [ToolboxData("<{0}:MetaDescription runat=\"server\"></{0}:MetaDescription>")]
    public class MetaDescription : Control
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
            if(page != null)
                output.Write(String.Format("<meta name=\"description\" content=\"{0}\" />", page.MetaDescription));
        }
    }
}
