using System.Web.UI;
using N2.Web.Parts;

namespace HereSay.Parts
{
    /// <summary>
    /// Base class for a part that can render its self.
    /// <seealso cref="IAddablePart"/>
    /// </summary>
    public abstract class AddablePart : Part, IAddablePart
    {
        /// <summary>
        /// Override to create the contro which renders the current <see cref="Part"/>.
        /// </summary>
        /// <returns>The <see cref="Control"/> to be rendered.</returns>
        protected abstract Control CreateViewControl();

        public System.Web.UI.Control AddTo(Control container)
        {
            System.Web.UI.Control control = this.CreateViewControl();
            container.Controls.Add(control);
            return control;
        }
    }
}
