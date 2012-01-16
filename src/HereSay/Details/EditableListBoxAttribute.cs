using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Web.UI.WebControls;
using Rolcore.Reflection;
using System;

namespace HereSay.Details
{
    public class EditableListBoxAttribute : EditableListControlAttribute
    {
        private const char ItemSeperator = ',';

        protected override ListControl CreateEditor()
        {
            ListBox result = new ListBox { SelectionMode = ListSelectionMode.Multiple };

            return result;
        }

        public EditableListBoxAttribute(string title, int sortOrder, string dataMember, string dataTextFieldScript, string dataValueFieldScript)
            : this(title, sortOrder, dataMember)
        {
            this.DataTextFieldScript = dataTextFieldScript;
            this.DataValueFieldScript = dataValueFieldScript;
        }

        public EditableListBoxAttribute(string title, int sortOrder, string dataMember) 
            : base(title, sortOrder)
        {
            this.DataMember = dataMember;
        }

        protected override Control AddEditor(Control container)
        {
            ListControl result = (ListControl)base.AddEditor(container);

            //TODO: Make the width and height part of the attribute (we can default them too 400 & 100, respectively)
            result.Style.Add("min-width", "400px");
            result.Height = new Unit(100, UnitType.Pixel);
            
            return result;
        }

        public override void UpdateEditor(N2.ContentItem item, Control editor)
        {
            if (item == null)
                throw new ArgumentNullException("item", "item is null.");
            if (editor == null)
                throw new ArgumentNullException("editor", "editor is null.");

            ListBox lb = (ListBox)editor;
            string currentlySelectedItemsString = (string)item[this.Name] ?? string.Empty;

            if (!string.IsNullOrEmpty(currentlySelectedItemsString))
            {
                string[] currentlySelectedItems = currentlySelectedItemsString.Split(ItemSeperator);
                if (currentlySelectedItems.Length > 0)
                {
                    foreach (string currentItem in currentlySelectedItems)
                    {
                        ListItem listItem = lb.Items.FindByValue(currentItem);

                        if (listItem != null)
                            listItem.Selected = true;
                    }
                }
            }
        }

        public override bool UpdateItem(N2.ContentItem item, Control editor)
        {
            ListBox lb = editor as ListBox;

            string selectedItems = string.Empty;

            foreach (ListItem listItem in lb.Items)
                if (listItem.Selected)
                    selectedItems += listItem.Value + ItemSeperator;

            selectedItems = selectedItems.TrimEnd(new char[] { ItemSeperator });

            item[this.Name] = (!string.IsNullOrEmpty(selectedItems)) ? selectedItems : null;
            return true;
        }
    }
}
