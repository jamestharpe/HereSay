using System.Web;
using System.Web.UI;
using HereSay.Pages;
using System;

namespace HereSay.UI
{
    [ToolboxData("<{0}:MetaKeywords runat=\"server\"></{0}:MetaKeywords>")]
    public class MetaKeywords : Control
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
                output.Write(String.Format("<meta name=\"keywords\" content=\"{0}\" />", page.MetaKeywords));
        }
    }
}
