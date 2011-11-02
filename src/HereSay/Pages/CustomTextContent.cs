using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using HereSay.Engine;
using Rolcore.Web;

namespace HereSay.Pages
{
    /// <code>
    ///    <add path="/CustomTextContent.ashx" verb="*" type="HereSay.Pages.CustomTextContent, HereSay" />
    /// </code>
    public class CustomTextContent : CustomTextItem, IHttpHandler
    {

        protected const string HttpHandlerTemplateUrl = "~/CustomTextContent.ashx";

        /// <summary>
        /// Gets and sets the content type to be used in the response.
        /// </summary>
        protected virtual string ResponseContentType
        {
            get { return GetDetail<string>("ContentType", ContentTypes.TextHtml); }
            set { SetDetail<string>("ContentType", value); }
        }

        /// <summary>
        /// Gets and sets the content text to be used in the response.
        /// </summary>
        protected virtual string ResponseContentText
        {
            get { return base.Text; }
        }

        public override bool IsPage { get { return true; } }

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            CustomTextContent item = ((CustomTextContent)N2.Context.CurrentPage);
            HttpResponse response = context.Response;
            response.WriteContent(item.ResponseContentText, item.ResponseContentType);
        }

        #endregion
    }
}
