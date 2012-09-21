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

        public abstract void Start();
        public abstract void Stop();
    }
}
