using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N2.Plugin;
using N2.Definitions;
using N2.Details;
using N2.Web.UI;
using Rolcore.Reflection;
using HereSay.Definitions;
using HereSay.Pages;
using N2.Engine;

namespace HereSay.Decorators
{
    /// <summary>
    /// Decorates pages within N2 with properties to manage syndication.
    /// </summary>

    [Service, AutoInitialize]
    public class SyndicationDecorator : AutoStarter
    {
        private readonly IDefinitionManager definitions;

        public SyndicationDecorator() { }
        public SyndicationDecorator(IDefinitionManager definitions)
        {
            this.definitions = definitions;
        }

        public override void Start()
        {
            //add the Syndication tab as a new "WebPage" definition
            IEnumerable<ItemDefinition> definitions = this.definitions.GetDefinitions()
                .Where(definition => 
                    IsPage(definition.ItemType)
                 && definition.ItemType.IsAssignableFrom(typeof(WebPage)));

            Parallel.ForEach<ItemDefinition>(definitions, definition =>
            {
                definition.EnsureTab(
                    EditModeTabs.PageInformationName,
                    EditModeTabs.PageInformationTitle,
                    EditModeTabs.PageInformationSortOrder);

                int sortOrder = definition.GetMaxEditableSortOrder();

                definition.AddEditableDate(
                    EditModeFields.Syndication.ContentPublishDateTitle,
                    ++sortOrder,
                    EditModeTabs.PageInformationName,
                    EditModeFields.Syndication.ContentPublishDateName,
                    EditModeFields.Syndication.ContentPublishDateShowDateBox,
                    EditModeFields.Syndication.ContentPublishDateShowTimeBox,
                    EditModeFields.Syndication.ContentPublishDateHelpTitle,
                    EditModeFields.Syndication.ContentPublishDateHelpBody);                

                definition.AddEditableCheckBox(
                    EditModeFields.Syndication.ExcludeFromFeedTitle,
                    ++sortOrder,
                    EditModeTabs.PageInformationName,
                    EditModeFields.Syndication.ExcludeFromFeedName,
                    false,
                    EditModeFields.Syndication.ExcludeFromFeedTitle,
                    EditModeFields.Syndication.ExcludeFromFeedHelpBody);
            });

            Debug.WriteLine("HereSay: SyndicationDecorator Started");
        }

        public override void Stop()
        {
            Debug.WriteLine("HereSay: SyndicationDecorator Stopped");
        }
    }
}
