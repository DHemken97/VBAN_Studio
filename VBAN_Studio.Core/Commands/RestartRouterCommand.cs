using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VBAN_Studio.Common;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Core.Commands
{
    public class RestartRouterCommand : Command
    {
        public override string Name => "RestartRouter";

        public override string Description => "Restarts all audio streams";

        public override void Execute(VbanStudioEnvironment _environment, string[] tokens)
        {
            Console.Write("Restarting...");
            _environment.RoutingManager.Stop();
            _environment.RoutingManager.Start();
            Console.WriteLine("Ok");
        }
    }
}
