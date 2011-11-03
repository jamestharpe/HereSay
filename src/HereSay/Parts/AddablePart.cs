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

        /// <summary>
        /// Adds the instance to the specified container.
        /// </summary>
        /// <param name="container">Specifies the container to which the instance should be added.</param>
        /// <returns>The <see cref="Control"/> representing the current part.</returns>
        public Control AddTo(Control container)
        {
            System.Web.UI.Control control = this.CreateViewControl();
            container.Controls.Add(control);
            return control;
        }
    }
}
