using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Details;
using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;
using Rolcore.Reflection;
using System.Collections;

namespace HereSay.Details
{
    public abstract class EditableListControlAttribute : AbstractEditableAttribute
    {
        protected abstract ListControl CreateEditor();

        public string DataMember { get; set; }
        public string DataTextFieldScript { get; set; }
        public string DataValueFieldScript { get; set; }

        public EditableListControlAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }

        protected override System.Web.UI.Control AddEditor(System.Web.UI.Control container)
        {
            ListControl editor = this.CreateEditor();

            N2.ContentItem currentItem = ((ItemEditor)container.NamingContainer).CurrentItem;

            object source = currentItem.GetPropertyValue(this.DataMember);

            IEnumerable enumerableSource = source as IEnumerable;
            if (enumerableSource == null)
                throw new InvalidOperationException("Data member is not enumerable: " + this.DataMember);

            IDictionary dictionarySource = source as IDictionary;

            if (dictionarySource != null)
            {
                foreach (object key in dictionarySource.Keys)
                    editor.Items.Add(new ListItem(dictionarySource[key].ToString(), key.ToString()));
            }
            else if (enumerableSource != null)
            {
                bool reflect = (!string.IsNullOrEmpty(this.DataTextFieldScript)) && (!string.IsNullOrEmpty(this.DataValueFieldScript));
                foreach (object item in enumerableSource)
                {
                    // Since we are calling ReplaceVars here we need to make sure that our DataTextField and DataValue field
                    // Follow the format {$Field}
                    string itemText = reflect ? ReflectionUtils.ReplaceVars(this.DataTextFieldScript, item).ToString()
                        : item.ToString();
                    string itemValue = reflect ? ReflectionUtils.ReplaceVars(this.DataValueFieldScript, item).ToString()
                        : item.ToString();
                    editor.Items.Add(new ListItem(itemText, itemValue));
                }
            }
            else
            {
                editor.DataSource = source;
                editor.DataTextField = this.DataTextFieldScript;
                editor.DataValueField = this.DataValueFieldScript;
                editor.DataBind();
            }

            container.Controls.Add(editor);
            return editor;
        }
    }
}
