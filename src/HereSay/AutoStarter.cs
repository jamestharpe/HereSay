using System;
using System.Diagnostics.Contracts;
using HereSay.Pages;
using Rolcore.Reflection;

namespace HereSay
{
    public abstract class AutoStarter : PluginInitializer, N2.Plugin.IAutoStart
    {
        /// <summary>
        /// A utility function for determining if the given <see cref="Type"/> represents a page in
        /// N2.
        /// </summary>
        /// <param name="itemType">Specifies the <see cref="Type"/>.</param>
        /// <param name="defaultValue">Specifies the default value to use if the page status of the
        /// given <see cref="Type"/> cannot be determined.</param>
        /// <returns>True of the <see cref="Type"/> is a page.</returns>
        protected static bool IsPage(Type itemType, bool defaultValue = false)
        {
            if (itemType == null)
                throw new ArgumentNullException("itemType", "itemType is null.");

            if (!itemType.HasEmptyConstructor() || !typeof(N2.ContentItem).IsAssignableFrom(itemType))
                return defaultValue;

            return ((N2.ContentItem)Activator.CreateInstance(itemType)).IsPage;
        }

        /// <summary>
        /// A utility function for determining if the given <see cref="Type"/> represents a HereSay
        /// home page.
        /// <seealso cref="HomePage"/>
        /// <seealso cref="LanguageHomePage"/>
        /// <seealso cref="LanguageHomeRedirectPage"/>
        /// </summary>
        /// <param name="itemType">Specifies the <see cref="Type"/>.</param>
        /// <param name="defaultValue">Specifies the default value to use if the page status of the
        /// given <see cref="Type"/> cannot be determined.</param>
        /// <returns>True of the <see cref="Type"/> is a HereSay Home Page.</returns>
        protected static bool IsHomePage(Type itemType, bool defaultValue = false)
        {
            return (typeof(HomePage).IsAssignableFrom(itemType) || typeof(LanguageHomeRedirectPage).IsAssignableFrom(itemType))
                && IsPage(itemType, defaultValue); //TODO: Unit test
        }

        /// <summary>
        /// Use instead of <see cref="N2.Context.CurrentPage"/> to prevent 
        /// <see cref="NHibernate.LazyInitializationException"/> from being thrown sporadically.
        /// </summary>
        protected static N2.ContentItem GetCurrentPage()
        {
            try
            {
                return N2.Context.CurrentPage;

                //
                // For some reason (need to investigate) this occasionally causes a 
                // NHibernate.LazyInitializationException Exception: "illegal access to loading 
                // collection". I'm guesing it's because the CurrentPage is still loading and
                // has not yet been cached by N2.
            }
            catch
            {
                return null;
            }
        }

        public abstract void Start();
        public abstract void Stop();
    }
}
