using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Web.UI.WebControls;
using Rolcore.Reflection;
using System;

namespace HereSay.Details
{
    public class EditableDropDownListAttribute : EditableListControlAttribute
    {
        protected override Control AddEditor(Control container)
        {
            DropDownList result = (DropDownList)base.AddEditor(container);

            if (!string.IsNullOrEmpty(this.DefaultSelectionText))
                result.Items.Insert(0, this.DefaultSelectionText);

            return result;
        }

        protected override ListControl CreateEditor()
        {
            DropDownList result = new DropDownList();

            return result;
        }

        #region Constructors
        public EditableDropDownListAttribute(string title, int order, string dataMember, string dataTextField, string dataValueField, string defaultDdlSelectionText)
            : base(title, order)
        {
            this.DataMember = dataMember;
            this.DataTextFieldScript = dataTextField;
            this.DataValueFieldScript = dataValueField;
            this.DefaultSelectionText = defaultDdlSelectionText;
        }

        public EditableDropDownListAttribute(string title, int order, string dataMember, string dataTextField, string dataValueField)
            : this(title, order, dataMember, dataTextField, dataValueField, null) { }
        
        public EditableDropDownListAttribute(string title, int order, string defaultDdlSelectionText, string dataMember)
            : this(title, order, dataMember, null, null, defaultDdlSelectionText) { }

        public EditableDropDownListAttribute(string title, int order, string dataMember)
            : this(title, order, dataMember, null, null, null) { }
        #endregion Constructors

        public string DefaultSelectionText { get; set; }

        public override void UpdateEditor(N2.ContentItem item, Control editor)
        {
            if (item == null)
                throw new ArgumentNullException("item", "item is null.");
            if (editor == null)
                throw new ArgumentNullException("editor", "editor is null.");

            DropDownList listControl = (DropDownList)editor;
            string defaultValue;
            if (listControl.Items.Count != 0)
                defaultValue = listControl.Items[0].Value;
            else
                defaultValue = string.Empty;

            listControl.SelectedValue = ((item[this.Name] as string) ?? defaultValue);
        }

        public override bool UpdateItem(N2.ContentItem item, Control editor)
        {
            if (item == null)
                throw new ArgumentNullException("item", "item is null.");
            if (editor == null)
                throw new ArgumentNullException("editor", "editor is null.");

            DropDownList listControl = editor as DropDownList;
            item[this.Name] = (listControl.SelectedValue != this.DefaultSelectionText) ? listControl.SelectedValue : null;
            return true;
        }
    }
}