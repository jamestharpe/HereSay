
using System;
namespace HereSay
{
    [Obsolete("Use a more specific class.")]
    public static class ItemSorting
    {
        public const int First = int.MinValue;

        public const int Content = 0;
        public const int Syndication = Content + 1000;
        public const int Redirect = Content + 2000;
        public const int Functional = Content + 3000;
        public const int Language = Content + 4000;
        public const int Custom = Content + 5000;

        public const int Last = int.MaxValue;
    }
}
