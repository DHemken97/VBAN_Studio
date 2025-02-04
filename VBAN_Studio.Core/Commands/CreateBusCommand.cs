using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VBAN_Studio.Common;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Core.Commands
{
    public class CreateBusCommand : Command
    {
        public override string Name => "CreateBus";

        public override string Description => "Creates an audio bus";

        public override void Execute(VbanStudioEnvironment _environment, string[] tokens)
        {
            var name = tokens[0];
            var bus = new Common.Audio.AudioBus(name, 44100, 2);
            _environment.RoutingManager.AudioInputs.Add(bus);
            _environment.RoutingManager.AudioOutputs.Add(bus);
        }
    }
}
