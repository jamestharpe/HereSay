using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit.Workflow;
using HereSay.Definitions;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using N2;

namespace HereSay
{
    /// <summary>
    /// Use instead of N2.Find. Provides additional methods for finding items within the current
    /// <see cref="N2.Context"/>.
    /// </summary>
    public class Find : N2.Persistence.GenericFind<WebsiteRoot, N2.ContentItem>
    {
        protected static N2.Definitions.IDefinitionManager Definitions
        {
            get { return N2.Context.Current.Definitions; }
        }

        /// <summary>
        /// Use instead of <see cref="N2.Context.CurrentPage"/> to prevent unwanted exceptions.
        /// Exceptions may be thrown, for example, when a database upgrade is needed and a plug-in 
        /// is initialized before N2 realizes the upgrade is needed.
        /// </summary>
        /// <returns>The current page or NULL if the current page is unavailable for any reason.</returns>
        public new static ContentItem CurrentPage
        {
            get
            {
                try
                {
                    return Context.Current.UrlParser.CurrentPage;

                    //
                    // For some reason (need to investigate) this occasionally causes a 
                    // NHibernate.LazyInitializationException Exception: "illegal access to loading 
                    // collection". I'm guessing it's because the CurrentPage is still loading and
                    // has not yet been cached by N2.
                }
                catch
                {
                    return null;
                }
            }
        }

        public static N2.ContentItem ByUrl(string url)
        {
            if (String.IsNullOrEmpty(url))
                throw new ArgumentException("url is null or empty.", "url");

            N2.ContentItem result;
            try
            {
                result = N2.Context.UrlParser.Parse(url);
                if (result == null)
                {
                    string startPageHost = new Uri(Find.StartPage.GetSafeUrl()).Host;
                    Uri uri = new Uri(url);
                    if (string.Equals(startPageHost, uri.Host, StringComparison.OrdinalIgnoreCase))
                        result = N2.Context.UrlParser.Parse(HttpUtility.UrlDecode(uri.AbsolutePath));
                }
            }
            catch (UriFormatException ex) // Cannot parse URL to URI
            {
                Debug.WriteLine("ByUrl() failed and returned null for " + url);
                Debug.WriteLine(ex);
                return null;
            }
            catch (NHibernate.ObjectNotFoundException ex)
            {
                // I have run into this NHibernate exception when doing an import of data. 
                // For now let's just return null. In the future we might want to handle this differently.
                Debug.WriteLine("ByUrl() failed and returned null for " + url);
                Debug.WriteLine(ex);
                return null;
            }

            return result;
        }

        public static T ByUrl<T>(string url) where T : N2.ContentItem
        {
            return (T)ByUrl(url);
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable"/> of <see cref="N2.ContentItem"/> instances which are
        /// assignable from the given type within the current website.
        /// </summary>
        /// <typeparam name="TContentItem">Specifies the type to which the desired pages are assignable.</typeparam>
        /// <param name="publishedOnly">When true, only published pages will be returned.</param>
        /// <returns>An <see cref="IEnumerable"/> of <see cref="N2.ContentItem"/> instances which are
        /// assignable from the given type.</returns>
        public static IEnumerable<TContentItem> PagesAssignableFrom<TContentItem>(bool publishedOnly)
            where TContentItem : N2.ContentItem
        {
            //TODO: Determine why this is returning items in the trash.

            Type[] types = Find.Definitions.GetTypesAssignableFrom(typeof(TContentItem));

            if (types.Length == 0)
                return new List<TContentItem>(); // Empty list

            IEnumerable<TContentItem> result;

            if (publishedOnly)
                result = N2.Find.Items
                    .Where
                        .Type.In(types)
                        .And.State.Eq(ContentState.Published)
                    .Select().Cast<TContentItem>();
            else
                result = N2.Find.Items
                    .Where
                        .Type.In(types)
                     .Select().Cast<TContentItem>();

            return result;
        }

        /// <summary>
        /// Gets children of the given parent that are of, or optionally assignable from, the given
        /// type.
        /// </summary>
        /// <typeparam name="TContentItem">The type of item to return.</typeparam>
        /// <param name="parent">The parent <see cref="N2.ContentItem"/> from which to retrieve 
        /// child items.</param>
        /// <param name="includeItemsAssignableFromTContentItem">When true, any child assignable 
        /// from TContentItem will be returned. When false, only items of the exact specified type
        /// will be returned.</param>
        /// <returns>An <see cref="IEnumerable"/> of <see cref="N2.ContentItem"/> instances which 
        /// are of or assignable from the given type.</returns>
        public static IEnumerable<TContentItem> PublishedChildren<TContentItem>(N2.ContentItem parent, bool includeItemsAssignableFromTContentItem)
        {
            if (includeItemsAssignableFromTContentItem)
            {
                Type[] typeImplementingPages = N2.Context.Current.Definitions.GetDefinitions()
                    .Where(definition =>
                        typeof(TContentItem).IsAssignableFrom(definition.ItemType))
                    .Select(definition => definition.ItemType)
                    .ToArray();

                if (typeImplementingPages.Length == 0)
                    return new List<TContentItem>();

                IEnumerable<TContentItem> result = Find.Items
                    .Where.Type.In(typeImplementingPages)
                        .And.Parent.Eq(parent)
                        .And.Published.Le(DateTime.Now)
                        //.And.State.Eq(ContentState.Published)
                    .Select()
                    .OrderBy(item => item.SortOrder)
                    .Cast<TContentItem>();

                return result;
            }
            return Find.Items
                .Where
                    .Parent.Eq(parent)
                    .And.Type.Eq(typeof(TContentItem))
                    .And.State.Eq(ContentState.Published)
                .Select()
                .OrderBy(item => item.SortOrder)
                .Cast<TContentItem>();
        }

        /// <summary>
        /// Gets parents of the given item that are of, or optionally assignable from, the given
        /// type.
        /// </summary>
        /// <typeparam name="TContentItem">The type of item to return.</typeparam>
        /// <param name="item">The <see cref="N2.ContentItem"/> from which to retrieve 
        /// parent items.</param>
        /// <param name="includeItemsAssignableFromTContentItem">When true, any parent assignable 
        /// from TContentItem will be returned. When false, only items of the exact specified type
        /// will be returned.</param>
        /// <returns>An <see cref="IEnumerable"/> of <see cref="N2.ContentItem"/> instances which 
        /// are of or assignable from the given type.</returns>
        public static IEnumerable<TContentItem> PublishedParents<TContentItem>(N2.ContentItem item, bool includeItemsAssignableFromTContentItem)
            where TContentItem : N2.ContentItem
        {
            N2.ContentItem parentItem = item;
            do
            {
                parentItem = parentItem.GetSafeParent();
                TContentItem typedParent = parentItem as TContentItem;
                if (typedParent != null && typedParent.State == ContentState.Published && (includeItemsAssignableFromTContentItem || parentItem.GetType() == typeof(TContentItem)))
                    yield return typedParent;
            } while (parentItem != null);
        }

        /// <summary>
        /// Gets the first parent of the given item that is of, or optionally assignable from, the given
        /// type.
        /// </summary>
        /// <typeparam name="TContentItem">The type of item to return.</typeparam>
        /// <param name="item">The <see cref="N2.ContentItem"/> from which to receive the parent.</param>
        /// <param name="includeItemsAssignableFromTContentItem">When true, any parent assignable 
        /// from TContentItem will be returned. When false, only items of the exact specified type
        /// will be returned.</param>
        /// <returns>The <see cref="N2.ContentItem"/> which is or assignable from the given type.</returns>
        public static TContentItem FirstPublishedParent<TContentItem>(N2.ContentItem item, bool includeItemsAssignableFromTContentItem)
            where TContentItem : class
        {
            TContentItem result = null;
            N2.ContentItem parentItem = item;
            do
            {
                parentItem = parentItem.GetSafeParent();
                TContentItem typedParent = parentItem as TContentItem;
                if ((typedParent != null) 
                    && (parentItem.Published <= DateTime.Now) 
                    && (parentItem.GetType() == typeof(TContentItem) || includeItemsAssignableFromTContentItem))
                    result = typedParent;
            } while (parentItem != null);
            return result;
        }
    }
}
