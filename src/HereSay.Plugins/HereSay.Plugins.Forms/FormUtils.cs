using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Rolcore.Web.UI;

namespace HereSay.Plugins.Forms
{
    internal static class FormUtils
    {
        public static NameValueCollection NameValueCollectionFromRequest(HttpRequest request, bool removeAspNetFields = true)
        {
            NameValueCollection formFields = (request.RequestType == "POST")
                ? request.Form
                : request.QueryString;

            NameValueCollection result = new NameValueCollection(formFields.Count);

            foreach (string key in formFields.AllKeys)
            {
                bool addToResult = !(
                       removeAspNetFields &&
                       (string.Equals(key, "__eventvalidation", System.StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(key, "__eventargument", System.StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(key, "__eventtarget", System.StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(key, "__viewstate", System.StringComparison.OrdinalIgnoreCase)));

                if (addToResult)
                    result.Add(
                        WebUtils.GetFormControlNameFromPostbackName(key),
                        formFields[key]);
            };

            return result;
        }
    }
}
