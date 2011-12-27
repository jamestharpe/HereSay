using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Engine.Globalization;
using System.Web;
using N2;

namespace HereSay.Engine
{
    /// <summary>
    /// Extension methods for <see cref="IEngine"/>.
    /// </summary>
    public static class IEngineExtensions
    {
        /// <summary>
        /// Get's the current <see cref="ILanguageGateway"/> implementation. The result is cached 
        /// for the duration of the current request.
        /// </summary>
        /// <param name="engine">Specifies the engine managing the language gateway.</param>
        /// <returns>An <see cref="ILanguageGateway"/>.</returns>
        public static ILanguageGateway GetLanguageGateway(this IEngine engine)
        {
            //
            // Pre-conditions

            if (engine == null)
                throw new ArgumentNullException("engine", "engine is null.");

            //
            // Check context

            HttpContext httpContext = HttpContext.Current;
            ILanguageGateway result = (httpContext != null)
                ? httpContext.Items["LanguageGateway"] as ILanguageGateway
                : null;

            //
            // Find, if not cached

            if (result == null)
            {
                result = Context.Current.Resolve<ILanguageGateway>();
                if (httpContext != null)
                    httpContext.Items["LanguageGateway"] = result;
            }

            return result;
        }
    }
}
