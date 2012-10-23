using N2;
using N2.Persistence.Finder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HereSay.Persistence.Finder
{
    public static class IQueryBuilderExtensions
    {
        public static IQueryAction MayBePublished(this IQueryBuilder q)
        {

            if (q == null)
                throw new ArgumentNullException("q", "q is null.");

            // See IsPublished at https://github.com/n2cms/n2cms/blob/master/src/Framework/N2/Utility.cs

            return q
                .OpenBracket()
                    .State.Eq(ContentState.New)          // Potentially published
                    .Or.State.Eq(ContentState.None)      // Potentially published
                    .Or.State.Eq(ContentState.Published) // Definitely published!
                .CloseBracket();
        }
    }
}
