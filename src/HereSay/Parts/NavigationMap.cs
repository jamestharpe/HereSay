using System.Collections.Generic;
using System.Linq;
using HereSay.Pages;
using System.Diagnostics.Contracts;

namespace HereSay.Parts
{
    [N2.PartDefinition(
        Title = "Navigation Map",
        IconUrl = "~/N2/Resources/icons/sitemap_color.png"),
     N2.Integrity.RestrictParents(typeof(HomePage)),
     N2.Integrity.RestrictChildren(typeof(NavigationMapItem)),
     N2.Details.WithEditableTitle(Required = true, Title = "Map Name")]
    public class NavigationMap : N2.ContentItem
    {
        private NavigationMapItem _CurrentMapItem;
        private IList<NavigationMapItem> _MapItems;
        private IList<NavigationMapItem> _Trail;

        [N2.Details.EditableChildren("Map Items", "MapItems", 100)]
        public IList<NavigationMapItem> MapItems
        {
            get 
            { 
                if(this._MapItems == null)
                    this._MapItems = this.GetPublishedChildren<NavigationMapItem>(true).ToList();

                return this._MapItems;
            }
        }

        /// <summary>
        /// The "trail" of <see cref="NavigationMapItem"/>s leading to the current page, starting 
        /// with the home page and ending at the current page. If multiple trails exist, the first 
        /// one is returned.
        /// <seealso cref="CurrentMapItem"/>
        /// </summary>
        public IList<NavigationMapItem> Trail
        {
            get
            {
                if (this._Trail == null)
                {
                    this._Trail = new List<NavigationMapItem>();
                    NavigationMapItem item = this.CurrentMapItem;

                    if (item == null)
                        return this._Trail;

                    while (item != null)
                    {
                        this._Trail.Add(item);
                        item = item.Parent as NavigationMapItem;
                    }

                    this._Trail = this._Trail.Reverse().ToList();
                }

                return this._Trail;
            }
        }

        /// <summary>
        /// The <see cref="NavigationMapItem"/> that links to the page being requested in the 
        /// current context. If multime items exist, the first one is returned.
        /// </summary>
        public NavigationMapItem CurrentMapItem
        {
            get
            {
                if (_CurrentMapItem == null)
                {
                    //
                    // Find Child

                    _CurrentMapItem = this.MapItems
                        .Where(item =>
                            item.DestinationIsCurrentPage)
                        .FirstOrDefault();

                    if (_CurrentMapItem != null)
                        return _CurrentMapItem;

                    //
                    // Not child, try grand-children

                    N2.ContentItem currentPage = N2.Context.CurrentPage;

                    _CurrentMapItem = this.MapItems
                        .Where(item =>
                            item.ChildLinksTo(currentPage))
                        .FirstOrDefault();

                    if (_CurrentMapItem != null)
                        _CurrentMapItem = _CurrentMapItem.ChildrenThatLinkTo(currentPage).First();
                }

                return _CurrentMapItem;
            }
        }
    }
}
