using System.Web;

namespace HereSay.Plugins.Caching
{
    /// <summary>
    /// A HereSay-friendly replacement for <see cref="System.Web.HttpCacheability"/>.
    /// <seealso cref="CachingDecorator"/>
    /// </summary>
    public enum Cacheability
    {
        Default          = 0, // Force to be first in the list for N2
        NoCache          = (int) HttpCacheability.NoCache,
        Private          = (int) HttpCacheability.Private,
        // same as ServerAndNoCache: Server           = (int) HttpCacheability.Server,
        ServerAndNoCache = (int) HttpCacheability.ServerAndNoCache,
        Public           = (int) HttpCacheability.Public,
        ServerAndPrivate = (int) HttpCacheability.ServerAndPrivate,
    }
}
