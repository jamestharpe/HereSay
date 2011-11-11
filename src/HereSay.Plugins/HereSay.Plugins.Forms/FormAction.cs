
namespace HereSay.Plugins.Forms
{
    [N2.Integrity.RestrictParents(typeof(Form)),
     N2.Details.WithEditableTitle(
         "Action Name", 5, Required = true, 
        HelpText = "This value is not shown on the page, it is just here to help you keep organized.")]
    public abstract class FormAction : N2.ContentItem
    {
        protected Form Form
        {
            get { return this.Parent as Form; }
        }

        public abstract void DoWork();
    }
}
