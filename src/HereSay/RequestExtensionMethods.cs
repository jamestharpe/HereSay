using System.Collections.Generic;
using System.Collections.Specialized;

namespace HereSay
{
    public static class RequestExtensionMethods
    {
        public static string[] GetValuesLike(this NameValueCollection request, string index)
        {
            List<string> result = new List<string>();
            foreach(string key in request.Keys)
            {
                if (key.Contains(index))
                    result.Add(request[key]);
            }
            return result.ToArray();
        }
    }
}
