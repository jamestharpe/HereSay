using System.Text;
using System.Web;
using System.Diagnostics;

namespace HereSay.Pages
{
    /// <summary>
    /// Base class for creating a redirect. No template is required, just add the following line to
    /// the web.config file under system.web/httpHandlers:
    /// <code>
    ///    <add path="/RedirectPage.ashx" verb="*" type="HereSay.Pages.RedirectPage, HereSay" />
    /// </code>
    /// </summary>
    [N2.Web.UI.TabContainer(EditModeTabs.Content, EditModeTabs.Content, PageSorting.FrequentUse),
     N2.PageDefinition(
         Title = "Redirect Page",
         Name = "CmsRedirectBasePage", //TODO: Data conversion: CmsRedirectBasePage -> RedirectPage
         Description = "Redirect to a different location.",
         TemplateUrl = "~/RedirectPage.ashx", 
         InstallerVisibility = N2.Installation.InstallerHint.NeverRootPage | N2.Installation.InstallerHint.NeverStartPage,
         IconUrl = "~/N2/Resources/icons/page_go.png"),
     N2.Details.WithEditableName("Name", 20, ContainerName = EditModeTabs.Content)]
    public class RedirectPage : N2.ContentItem, IHttpHandler
    {
        protected virtual string GetRedirectUrlPath()
        {
            return this.RedirectUrl;
        }

        /// <summary>
        /// Gets and sets a value indicating if the redirect is a permanant (301) redirect.
        /// </summary>
        [N2.Details.EditableCheckBox("301 (permanent) redirect", 100, ContainerName = EditModeTabs.Content)]
        public virtual bool Redirect301
        {
            get { return (bool)(GetDetail("Redirect301") ?? false); }
            set { SetDetail("Redirect301", value, false); }
        }

        /// <summary>
        /// Gets and sets the URL to redirect to.
        /// </summary>
        [N2.Details.EditableUrl("Redirect to", 40, Required = true, ContainerName = EditModeTabs.Content)]
        public virtual string RedirectUrl
        {
            get { return base.GetDetail<string>("RedirectUrl", null); }
            set { base.SetDetail<string>("RedirectUrl", value); }
        }

        /// <summary>
        /// Gets the <see cref="Name"/> property to ensure it is a consistent and visible URL.
        /// </summary>
        public override string Title
        {
            get { return this.Name; }
        }

        #region IHttpHandler Members

        public virtual bool IsReusable
        {
            get { return false; }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            RedirectPage page = ((RedirectPage)N2.Context.CurrentPage);
            string redirectUrl = page.GetRedirectUrlPath();
            StringBuilder query = new StringBuilder("?");
            foreach (string key in context.Request.QueryString.Keys)
            {
                redirectUrl = redirectUrl.Replace(string.Format("{{{0}}}", key), context.Request.QueryString[key]);
                //remove the page=n from the querystring. This is put there by N2, and doesn't 
                // belong on the redirect url.
                if (key != "page")
                    query.AppendFormat("{0}={1}&", key, context.Request.QueryString[key]);
            }

            string queryString = query.ToString();

            if(queryString != "?")
                redirectUrl += query.ToString();

            if (page.Redirect301)
                context.Response.RedirectPermanent(redirectUrl);
            else
                context.Response.Redirect(redirectUrl);
        }

        #endregion
    }
}
