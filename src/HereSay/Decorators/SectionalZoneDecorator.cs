using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.UI;
using N2.Collections;
using HereSay.Pages;

namespace HereSay.Decorators
{
    /// <summary>
    /// Implements "Recusive" zones functionality.
    /// </summary>
    [N2.Engine.Adapts(typeof(WebPage))]
    public class SectionalZoneDecorator : N2.Web.Parts.PartsAdapter
    {
        public override ItemList GetItemsInZone(N2.ContentItem parentItem, string zoneName)
        {
            ItemList items = base.GetItemsInZone(parentItem, zoneName);
            if (zoneName.StartsWith("Sectional") && parentItem != null && parentItem.IsPage)
                items.AddRange(GetItemsInZone(parentItem.GetSafeParent(), zoneName));

            return items;
        }
    }
}
