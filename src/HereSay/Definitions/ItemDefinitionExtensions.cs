using System;
using System.Diagnostics.Contracts;
using System.Linq;
using N2.Definitions;
using N2.Details;
using N2.Web.UI;
using System.Diagnostics;

namespace HereSay.Definitions
{
    /// <summary>
    /// Extension methods for <see cref="ItemDefinition"/>.
    /// </summary>
    public static class ItemDefinitionExtensions
    {
        /// <summary>
        /// Ensures that a <see cref="TabContainerAttribute"/> exists on the item definition and 
        /// adds it if it does not.
        /// </summary>
        /// <param name="itemDefinition">Specifies the <see cref="ItemDefinition"/> that should 
        /// include the tab.</param>
        /// <param name="tabName">Specifies the desired <see cref="TabContainerAttribute.Name"/>.</param>
        /// <param name="tabText">Specifies the desired <see cref="TabContainerAttribute.TabText"/>
        /// this value is only used if no value is already present.</param>
        /// <param name="tabSortOrder">Specifies the desired 
        /// <see cref="TabContainerAttribute.SortOrder"/> this value is only used if no value is 
        /// already present.</param>
        public static void EnsureTab(this ItemDefinition itemDefinition, string tabName, string tabText, int tabSortOrder)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException("itemDefinition", "itemDefinition is null.");
            if (String.IsNullOrEmpty(tabName))
                throw new ArgumentException("tabName is null or empty.", "tabName");
            if (String.IsNullOrEmpty(tabText))
                throw new ArgumentException("tabText is null or empty.", "tabText");

            if (itemDefinition.Containers.Any(container => container.Name == tabName))
                return; // Already has tab

            TabContainerAttribute tab = new TabContainerAttribute(
                tabName,
                tabText,
                tabSortOrder);

            Debug.WriteLine(string.Format("{0} tab added to {1}", tabText, itemDefinition.Title));

            itemDefinition.Add(tab);
        }

        /// <summary>
        /// Returns the highest <see cref="IContainable.SortOrder"/> value for all values in 
        /// <see cref="ItemDefinition.Editables"/>.
        /// </summary>
        public static int GetMaxEditableSortOrder(this ItemDefinition itemDefinition)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException("itemDefinition", "itemDefinition is null.");

            if (itemDefinition.Editables.Count == 0)
                return 0;

            return itemDefinition.Editables.Max(editable => editable.SortOrder);
        }

        /// <summary>
        /// Adds an <see cref="EditableEnumAttribute"/> to the item definition.
        /// </summary>
        /// <param name="itemDefinition">Specifies the <see cref="ItemDefinition"/> the 
        /// <see cref="EditableEnumAttribute"/> will be added to.</param>
        /// <param name="title">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.Title"/>.</param>
        /// <param name="sortOrder">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.SortOrder"/>.</param>
        /// <param name="enumType">Specifies the value type of enum.</param>
        /// <param name="containerName">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.ContainerName"/>.</param>
        /// <param name="propertyKey">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.Name"/>.</param>
        /// <param name="helpTitle">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.HelpTitle"/>.</param>
        /// <param name="helpText">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.HelpText"/>.</param>
        /// <returns>The <see cref="EditableEnumAttribute"/> added to the item definition.</returns>
        public static EditableEnumAttribute AddEditableEnum(this ItemDefinition itemDefinition, string title, int sortOrder, Type enumType, string containerName, string propertyKey, string helpTitle = null, string helpText = null)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException("itemDefinition", "itemDefinition is null.");
            if (String.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty.", "title");
            if (enumType == null)
                throw new ArgumentNullException("enumType", "enumType is null.");
            if (String.IsNullOrEmpty(containerName))
                throw new ArgumentException("containerName is null or empty.", "containerName");
            if (String.IsNullOrEmpty(propertyKey))
                throw new ArgumentException("propertyKey is null or empty.", "propertyKey");

            EditableEnumAttribute result = new EditableEnumAttribute(title, sortOrder, enumType)
            {
                ContainerName = containerName,
                Name = propertyKey,
                HelpTitle = helpTitle,
                HelpText = helpText
            };

            itemDefinition.Add(result);

            Debug.WriteLine(string.Format("{0} property added to {1}", propertyKey, itemDefinition.Title));

            return result;
        }

        /// <summary>
        /// Adds an <see cref="EditableTextBoxAttribute"/> to the item definition.
        /// </summary>
        /// <param name="itemDefinition">Specifies the <see cref="ItemDefinition"/> the 
        /// <see cref="EditableEnumAttribute"/> will be added to.</param>
        /// <param name="title">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.Title"/>.</param>
        /// <param name="sortOrder">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.SortOrder"/>.</param>
        /// <param name="maxLength">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.MaxLength"/>.</param>
        /// <param name="containerName">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.ContainerName"/>.</param>
        /// <param name="propertyKey">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.Name"/>.</param>
        /// <param name="helpTitle">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.HelpTitle"/>.</param>
        /// <param name="helpText">Specifies the value to use for 
        /// <see cref="EditableEnumAttribute.HelpText"/>.</param>
        /// <returns>The <see cref="EditableTextBoxAttribute"/> added to the item definition.</returns>
        public static EditableTextBoxAttribute AddEditableTextBox(this ItemDefinition itemDefinition, string title, int sortOrder, int maxLength, string containerName, string propertyKey, string helpTitle = null, string helpText = null)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException("itemDefinition", "itemDefinition is null.");
            if (String.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty.", "title");
            if (String.IsNullOrEmpty(containerName))
                throw new ArgumentException("containerName is null or empty.", "containerName");
            if (String.IsNullOrEmpty(propertyKey))
                throw new ArgumentException("propertyKey is null or empty.", "propertyKey");
            if (maxLength <= 0)
                throw new ArgumentOutOfRangeException("maxLength");

            EditableTextBoxAttribute result = new EditableTextBoxAttribute(title, sortOrder, maxLength)
            {
                ContainerName = containerName,
                Name = propertyKey,
                HelpTitle = helpTitle,
                HelpText = helpText
            };

            itemDefinition.Add(result);

            Debug.WriteLine(string.Format("{0} property added to {1}", propertyKey, itemDefinition.Title));

            return result;
        }

        /// <summary>
        /// Adds an <see cref="EditableCheckBoxAttribute"/> to the item definition.
        /// </summary>
        /// <param name="itemDefinition">Specifies the <see cref="ItemDefinition"/> the 
        /// <see cref="EditableCheckBoxAttribute"/> will be added to.</param>
        /// <param name="title">Specifies the value to use for 
        /// <see cref="EditableCheckBoxAttribute.CheckBoxText"/>.</param>
        /// <param name="sortOrder">Specifies the value to use for 
        /// <see cref="EditableCheckBoxAttribute.SortOrder"/>.</param>
        /// <param name="containerName">Specifies the value to use for 
        /// <see cref="EditableCheckBoxAttribute.ContainerName"/>.</param>
        /// <param name="propertyKey">Specifies the value to use for 
        /// <see cref="EditableCheckBoxAttribute.Name"/>.</param>
        /// <param name="helpTitle">Specifies the value to use for 
        /// <see cref="EditableCheckBoxAttribute.HelpTitle"/>.</param>
        /// <param name="helpText">Specifies the value to use for 
        /// <see cref="EditableCheckBoxAttribute.HelpText"/>.</param>
        /// <returns>The <see cref="EditableCheckBoxAttribute"/> added to the item definition.</returns>
        public static EditableCheckBoxAttribute AddEditableCheckBox(this ItemDefinition itemDefinition, string title, int sortOrder, string containerName, string propertyKey, bool? defaultValue = null, string helpTitle = null, string helpText = null)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException("itemDefinition", "itemDefinition is null.");
            if (String.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty.", "title");
            if (String.IsNullOrEmpty(containerName))
                throw new ArgumentException("containerName is null or empty.", "containerName");
            if (String.IsNullOrEmpty(propertyKey))
                throw new ArgumentException("propertyKey is null or empty.", "propertyKey");

            EditableCheckBoxAttribute result = new EditableCheckBoxAttribute(title, sortOrder)
            {
                ContainerName = containerName,
                Name = propertyKey,
                HelpTitle = helpTitle,
                HelpText = helpText,
                DefaultValue = defaultValue
            };

            itemDefinition.Add(result);

            Debug.WriteLine(string.Format("{0} property added to {1}", propertyKey, itemDefinition.Title));

            return result;
        }

        /// <summary>
        /// Adds an <see cref="EditableDateAttribute"/> to the item definition.
        /// </summary>
        /// <param name="itemDefinition">Specifies the <see cref="ItemDefinition"/> the 
        /// <see cref="EditableDateAttribute"/> will be added to.</param>
        /// <param name="title">Specifies the value to use for 
        /// <see cref="EditableDateAttribute.Title"/>.</param>
        /// <param name="sortOrder">Specifies the value to use for 
        /// <see cref="EditableDateAttribute.SortOrder"/>.</param>
        /// <param name="containerName">Specifies the value to use for 
        /// <see cref="EditableDateAttribute.ContainerName"/>.</param>
        /// <param name="propertyKey">Specifies the value to use for 
        /// <see cref="EditableDateAttribute.Name"/>.</param>
        /// <param name="showDate">Specifies the value to use for
        /// <see cref="EditableDateAttribute.ShowDate"/>.</param>
        /// <param name="showTime">Specifies the value to use for
        /// <see cref="EditableDateAttribute.ShowTime"/>.</param>
        /// <param name="helpTitle">Specifies the value to use for 
        /// <see cref="EditableDateAttribute.HelpTitle"/>.</param>
        /// <param name="helpText">Specifies the value to use for 
        /// <see cref="EditableDateAttribute.HelpText"/>.</param>
        /// <returns>The <see cref="EditableCheckBoxAttribute"/> added to the item definition.</returns>
        public static EditableDateAttribute AddEditableDate(this ItemDefinition itemDefinition, string title, int sortOrder, string containerName, string propertyKey, bool showDate = true, bool showTime = true, string helpTitle = null, string helpText = null)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException("itemDefinition", "itemDefinition is null.");
            if (String.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty.", "title");
            if (String.IsNullOrEmpty(containerName))
                throw new ArgumentException("containerName is null or empty.", "containerName");
            if (String.IsNullOrEmpty(propertyKey))
                throw new ArgumentException("propertyKey is null or empty.", "propertyKey");

            EditableDateAttribute result = new EditableDateAttribute(title, sortOrder)
            {
                ContainerName = containerName,
                ShowDate = showDate,
                ShowTime = showTime,
                Name = propertyKey,
                HelpTitle = helpTitle,
                HelpText = helpText
            };

            itemDefinition.Add(result);

            Debug.WriteLine(string.Format("{0} property added to {1}", propertyKey, itemDefinition.Title));

            return result;
        }
    }
}
