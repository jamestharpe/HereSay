using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Rolcore.Web.UI;
using System;

namespace HereSay.Plugins.Forms
{
    public static class FormUtils
    {
        public static NameValueCollection NameValueCollectionFromRequest(HttpRequest request, bool removeAspNetFields = true)
        {
            NameValueCollection formFields = (request.RequestType == "POST")
                ? request.Form
                : request.QueryString;

            NameValueCollection result = new NameValueCollection(formFields.Count);

            foreach (string key in formFields.AllKeys)
            {
                string fieldName = WebUtils.GetFormControlNameFromPostbackName(key);
                bool addToResult = !(
                       removeAspNetFields &&
                       (string.Equals(fieldName, "__eventvalidation", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(fieldName, "__eventargument", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(fieldName, "__eventtarget", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(fieldName, "__viewstate", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(fieldName, Form.FormIdFieldName, StringComparison.OrdinalIgnoreCase)));

                if (addToResult)
                    result.Add(
                        fieldName,
                        formFields[key]);
            };

            return result;
        }
    }
}
