using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Engine.Globalization;
using System.Web;

namespace HereSay.Engine
{
    /// <summary>
    /// Extension methods for <see cref="IEngine"/>.
    /// </summary>
    public static class IEngineExtensions
    {
        public static IEnumerable<ILanguage> GetAvailableLanguages(this IEngine engine)
        {
            return engine.Resolve<ILanguageGateway>().GetAvailableLanguages();
        }
    }
}
