using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.Common
{
    public class VbanStudioEnvironment
    {
        public VbanStudioEnvironment(string pluginDirectory = "./Plugins")
        {
            RoutingManager = new RoutingManager();
            CommandManager = new CommandManager();
            CommandManager.RegisterEnvironment(this);
            CommandManager.RegisterCommandsFromPlugins(pluginDirectory);

        }

        public RoutingManager RoutingManager { get; set; }
        public CommandManager CommandManager { get; set; }
    }
}
