using System;

namespace HereSay.Plugins.Forms
{
    [N2.PartDefinition(
        Title = "Redirect",
        Name = "FormRedirecter",
        IconUrl = "~/N2/Resources/icons/application_go.png")]    
    public class FormRedirecter : FormAction
    {
        [N2.Details.EditableUrl("Redirect Url", 10, Required = true, RequiredText = "*")]
        public virtual string RedirectUrl
        {
            get { return GetDetail<string>("RedirectUrl", String.Empty); }
            set { SetDetail<string>("RedirectUrl", value, String.Empty); }
        }

        public override void DoWork()
        {
            System.Web.HttpContext.Current
                .Response.Redirect(this.RedirectUrl, true);
        }
    }
}
