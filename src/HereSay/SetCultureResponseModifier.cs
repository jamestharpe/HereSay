using System;
using System.Diagnostics;
using System.Web;
using N2.Plugin;
using N2.Web;

namespace HereSay
{
    /// <summary>
    /// A HereSay <see cref="AutoStarter"/> that automatically detects the culture information of 
    /// the requested page and sets the current thread culture accordingly.
    /// </summary>
    [AutoInitialize]
    public class SetCultureResponseModifier : AutoStarter
    {
        private static void ApplicationInstance_AcquireRequestState(object sender, EventArgs e)
        {
            HttpContext.Current.SetCurrentCulture();
        }

        public override void Start()
        {
            EventBroker.Instance.AcquireRequestState += ApplicationInstance_AcquireRequestState;
            Debug.WriteLine("SetCultureResponseModifier Started");
        }

        public override void Stop()
        {
            Debug.WriteLine("SetCultureResponseModifier Stopped");
        }
    }
}
