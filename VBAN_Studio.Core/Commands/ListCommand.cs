using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VBAN_Studio.Common;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Core.Commands
{
    public class ListCommand : Command
    {
        public override string Name => "list";

        public override string Description => "List";

        public override void Execute(VbanStudioEnvironment _environment, string[] tokens)
        {
            foreach (var stream in _environment.RoutingManager.AudioStreams)
                Console.WriteLine($"{stream.Input.Name} -> {stream.Output.Name}");
        }
    }
}
