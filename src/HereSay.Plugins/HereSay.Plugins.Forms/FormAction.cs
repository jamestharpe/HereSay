using N2.Details;
using N2.Integrity;

namespace HereSay.Plugins.Forms
{
    [RestrictParents(typeof(Form)),
     WithEditableTitle(
         "Action Name", 5, Required = true, 
         HelpText = "This value is not shown on the page, it is just here to help you keep organized.")]
    public abstract class FormAction : N2.ContentItem
    {
        public Form Form
        {
            get { return this.Parent as Form; }
        }

        public abstract void DoWork();
    }
}
