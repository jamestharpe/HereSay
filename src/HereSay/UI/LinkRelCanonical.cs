using System.Web;
using System.Web.UI;

namespace HereSay.UI
{
    [ToolboxData("<{0}:LinkRelCanonical runat=\"server\"></{0}:LinkRelCanonical>")]
    public class LinkRelCanonical : Control
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
            output.Write(
                "<link rel=\"canonical\" href=\"{0}\" />", 
                HttpContext.Current.GetFullyQualifiedCanonicalUrl());
        }
    }
}
