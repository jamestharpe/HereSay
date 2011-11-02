using System.Collections.Generic;
using System.Linq;
using System;
using System.Web;
using System.Diagnostics.Contracts;
using Rolcore.Web;
using Rolcore;

namespace HereSay.Parts
{
    [N2.PartDefinition(
        Title = "Link",
        Name = "NavigationMapItem2", // compatability issue with legacy items
        IconUrl = "~/N2/Resources/icons/link.png"),
     N2.Integrity.RestrictParents(typeof(NavigationMap), typeof(NavigationMapItem)),
     N2.Integrity.RestrictChildren(typeof(NavigationMapItem))]
    public class NavigationMapItem : Hyperlink
    {
        [N2.Details.EditableChildren("Map Items", "MapItems", 100)]
        public virtual IList<NavigationMapItem> MapItems
        {
            get { return this.Children.Cast<NavigationMapItem>().ToList(); }
        }

        /// <summary>
        /// Returns a list of child <see cref="NavigationMapItem"/>s that link to the given
        /// <see cref="N2.ContentItem"/>, with the option to search recusivly.
        /// </summary>
        /// <param name="item">Specifies the <see cref="N2.ContentItem"/> that children link to.</param>
        /// <param name="recurse">Specifies if recursion should be used to generate the result.</param>
        /// <returns>Zero (0) or more <see cref="NavigationMapItem"/> which link to the given 
        /// <see cref="N2.ContentItem"/></returns>
        public IEnumerable<NavigationMapItem> ChildrenThatLinkTo(N2.ContentItem item, bool recurse = true)
        {
            IEnumerable<NavigationMapItem> candidates = this.MapItems
                .Where(mapItem => (mapItem.DestinationWebPage != null
                    && mapItem.DestinationWebPage.ID == item.ID)
                    || mapItem.ChildLinksTo(item));

            foreach (NavigationMapItem child in candidates)
            {
                if (child.DestinationIsCurrentPage)
                    yield return child;

                if(recurse)
                {
                    IEnumerable<NavigationMapItem> children = child.ChildrenThatLinkTo(item);
                    foreach (NavigationMapItem grandChild in children)
                        yield return grandChild;
                }
            }
        }

        public bool ChildLinksTo(N2.ContentItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item", "item is null.");
            if(!item.IsPage)
                throw new InvalidOperationException("item is not a page");

            string url = item.GetSafeUrl().ToUri().AbsolutePath;

            // Destination URLs are forced to match the absolute path of the Safe URL for 
            // internally Hyperlink items so there is no need to fetch the destination web page to 
            // determine its safe url.

            return this.MapItems
                .Any(mapItem =>
                    mapItem.DestinationUrl == url
                 || mapItem.ChildLinksTo(item));
        }
    }
}
