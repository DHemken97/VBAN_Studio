using VBAN_Studio.Common;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Core.Commands
{
    public class MapCommand : Command
    {
        public override string Name => "Map";

        public override string Description => "Creates an audio stream between 2 points";

        public override void Execute(VbanStudioEnvironment _environment, string[] tokens)
        {
            var routingManager = _environment.RoutingManager; 
        }
    }
}
