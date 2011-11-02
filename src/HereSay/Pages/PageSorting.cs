using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HereSay.Pages
{
    public static class PageSorting
    {
        public const int First = int.MinValue;

        public const int
            ExtremelyFrequentUse = 0,
            VeryFrequentUse = 1000,
            FrequentUse = 2000,
            InfrequentUse = 3000,
            RareUse = 4000,
            VeryRareUse = 5000;


        public const int Last = int.MaxValue;
    }
}
