using System;
using System.Collections.Generic;
using System.Linq;
using HereSay.Pages;
using N2;
using HereSay.Persistence.Finder;

namespace HereSay.Parts
{
    [N2.PartDefinition(
        Title = "Link (Auto)",
        Name = "NavigationAutoMapItem",
        IconUrl = "~/N2/Resources/icons/link_go.png"),
     N2.Integrity.RestrictParents(typeof(NavigationMap), typeof(NavigationMapItem)),
     N2.Integrity.RestrictChildren(typeof(NavigationMapItem))]
    public class NavigationAutoMapItem : NavigationMapItem
    {
        public override IList<NavigationMapItem> MapItems
        {
            get
            {
                string cacheKey = string.Format("auto_{0}{1}", this.Name, this.ID);
                IList<NavigationMapItem> result = System.Web.HttpContext.Current.Cache[cacheKey] as IList<NavigationMapItem>;

                if (result == null)
                {

                    //
                    // Check for destination

                    N2.ContentItem destination = this.DestinationWebPage;
                    result = base.MapItems;
                    if (destination == null)
                        return result;

                    //
                    // Auto-generate child items

                    DateTime now = DateTime.Now;

                    IEnumerable<N2.ContentItem> childPages = Find.Items
                        .Where
                            .Parent.Eq(destination)
                        //.And.State.Eq(ContentState.Published)
                            .And.MayBePublished()
                            .And.Type.NotEq(typeof(CustomContent))              // Exclude items
                            .And.Type.NotEq(typeof(CustomCssContent))           // you wouldn't 
                            .And.Type.NotEq(typeof(CustomJavaScriptContent))    // want to link to.
                            .And.Type.NotEq(typeof(CustomTextContent))
                            .And.Type.NotEq(typeof(FeedPage))
                            .And.Type.NotEq(typeof(RedirectPage))
                        .Select()
                        .Where(child => 
                            child.IsPage
                            && child.IsPublished());

                    foreach (N2.ContentItem page in childPages)
                    {
                        NavigationAutoMapItem autoChild = new NavigationAutoMapItem()
                        {
                            DestinationWebPage = page,
                            Name = Guid.NewGuid().ToString(),
                            Published = now,
                            SavedBy = this.SavedBy,
                            Title = page.Title,
                            Parent = this
                        };
                        result.Add(autoChild);
                    }

                    System.Web.HttpContext.Current.Cache[cacheKey] = result;
                }
                return result;
            }
        }
    }
}
