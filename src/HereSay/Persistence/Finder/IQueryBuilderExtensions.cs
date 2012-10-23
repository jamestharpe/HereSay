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
        public static IQueryAction IsPublished(this IQueryBuilder queryBuilder)
        {

            if (queryBuilder == null)
                throw new ArgumentNullException("queryBuilder", "queryBuilder is null.");

            //
            // See IsPublished at https://github.com/n2cms/n2cms/blob/master/src/Framework/N2/Utility.cs
            //
            // item.Published.HasValue && item.Published <= Utility.CurrentTime()
            //			&& (!item.Expires.HasValue || Utility.CurrentTime() < item.Expires.Value);

            var now = DateTime.Now;

            return queryBuilder
                .OpenBracket()
                    .State.Eq(ContentState.New)
                    .Or.State.Eq(ContentState.None)
                    .Or.State.Eq(ContentState.Published)
                .CloseBracket()
                .And.Published.IsNotNull()
                .And.Published.Le(now)
                .And.OpenBracket()
                    .Expires.IsNull()
                    .Or.Expires.Gt(now)
                .CloseBracket();
        }
    }
}
