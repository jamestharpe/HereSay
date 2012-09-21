using System;
using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using HereSay.Pages;
using N2;

namespace HereSay.Decorators
{
    /// <summary>
    /// Implements "recursive" zones functionality.
    /// </summary>
    [N2.Engine.Adapts(typeof(WebPage))]
    public class SectionalZoneDecorator : N2.Web.Parts.PartsAdapter
    {
        public override IEnumerable<ContentItem> GetParts(N2.ContentItem parentItem, string zoneName, string filteredForInterface)
        {
            IEnumerable<ContentItem> items = base.GetParts(parentItem, zoneName, filteredForInterface);
            if ((zoneName.StartsWith("Sectional") || zoneName.StartsWith("hs_Sectional")) && parentItem != null)
            {
                if (parentItem.IsPage)
                    items = items.Union(GetParts(parentItem.GetSafeParent(), zoneName, filteredForInterface));
                else
                    items = items.Union(GetParts(parentItem.VersionOf.Parent, zoneName, filteredForInterface));
            }

            return items;
        }
    }
}
