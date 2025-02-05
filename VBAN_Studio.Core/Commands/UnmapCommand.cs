using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VBAN_Studio.Common;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Core.Commands
{
    public class UnmapCommand : Command
    {
        public override string Name => "Unmap";

        public override string Description => "Remove Audio Stream";

        public override void Execute(VbanStudioEnvironment _environment, string[] tokens)
        {
            var index = int.Parse(tokens[0]);
            _environment.RoutingManager.UnMap(index);
            _environment.RoutingManager.Stop();
            _environment.RoutingManager.Start();
            
        }
    }
}
