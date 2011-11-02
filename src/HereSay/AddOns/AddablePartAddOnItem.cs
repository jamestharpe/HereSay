using System.Web.UI;
using N2.Web.Parts;

namespace HereSay.AddOns
{
    public abstract class AddablePartAddOnItem : AddOnItem, IAddablePart
    {
        protected abstract Control CreateViewControl();

        #region IAddablePart Members

        public System.Web.UI.Control AddTo(Control container)
        {
            System.Web.UI.Control control = this.CreateViewControl();
            container.Controls.Add(control);
            return control;
        }

        #endregion
    }
}
