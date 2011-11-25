using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HereSay.Plugins.Forms
{
    public interface IFormSubmissionsProvider
    {
        /// <summary>
        /// Gets form submissions.
        /// </summary>
        IList<FormSubmission> Submissions { get; }
        Form Form { get; }
        string Name { get; }
    }
}
