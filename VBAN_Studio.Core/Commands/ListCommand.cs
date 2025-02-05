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
            Console.WriteLine("Streams:");
            var index = 0;

            foreach (var stream in _environment.RoutingManager.AudioStreams)
                Console.WriteLine($"{index++}) {stream.Input.Name} -> {stream.Output.Name}");

            Console.WriteLine("\r\n\r\nInputs:");
            index = 0;
            foreach (var d in _environment.RoutingManager.AudioInputs)
                Console.WriteLine($"{index++}) {d.Name}");
            Console.WriteLine("\r\n\r\nOutputs:");
             index = 0;
            foreach (var d in _environment.RoutingManager.AudioOutputs)
                Console.WriteLine($"{index++}) {d.Name}");

        }
    }
}
