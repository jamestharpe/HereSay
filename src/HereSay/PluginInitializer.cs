using System;
using System.Collections.Generic;
using System.Diagnostics;
using N2.Definitions;
using N2.Engine;
using N2.Persistence;
using N2.Plugin;

namespace HereSay
{
    /// <summary>
    /// Serves as a base-class for plug-ins by implementing 
    /// <see cref="N2.Plugin.IPluginInitializer"/> to automatically registering the plug-in with 
    /// the N2 engine (<see cref="N2.Engine.IEngine"/>).
    /// </summary>
    
    public class PluginInitializer : N2.Plugin.IPluginInitializer
    {
        /// <summary>
        /// Implements <see cref="N2.Plugin.IPluginInitializer.Initialize"/> to register the 
        /// instance with N2 under its full type name.
        /// </summary>
        /// <param name="engine">Specifies the <see cref="N2.Engine.IEngine"/> instance to register
        /// the plug-in with.</param>
        public virtual void Initialize(N2.Engine.IEngine engine)
        {
            Type type = this.GetType();
            string fullName = type.FullName;
            engine.Container.AddComponent(fullName, type, type);

            Debug.WriteLine(string.Format(
                "HereSay: {0} initialized.", fullName));
        }
    }
}
