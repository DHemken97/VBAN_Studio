using VBAN_Studio.Common;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Core.Commands
{
    public class ListCommand : Command
    {
        public override string Name => "List";

        public override string Description => "Generates a list of hardware inputs, outputs, vban outputs, buses, and connections";

        public override void Execute(VbanStudioEnvironment _environment, string[] tokens)
        {
            var routingManager = _environment.RoutingManager;

            // List Hardware Inputs
            ListItems("Hardware Inputs", routingManager.InputDevices, device => $"{device.Name} (ID: {device.Id})");

            // List Hardware Outputs
            ListItems("Hardware Outputs", routingManager.OutputDevices, device => $"{device.Name} (ID: {device.Id})");

            // List VBAN Outputs
           // ListItems("VBAN Outputs", _environment.VbanOutputs, vbanOutput => vbanOutput.GetDisplayName());

            // List Buses
            ListItems("Buses", routingManager.Buses, bus => $"{bus.Name} (ID: {bus.Id})");

            // List Connections
            //ListItems("Connections", routingManager.GetConnections(), connection => $"{connection.SourceDevice.Name} -> {connection.DestinationDevice.Name}");
        }

        private void ListItems<T>(string title, IEnumerable<T> items, Func<T, string> displayFormatter)
        {
            Console.WriteLine($"\n{title}:");
            if (items == null || !items.Any())
            {
                Console.WriteLine($"  No {title.ToLower()} found.");
            }
            else
            {
                foreach (var item in items)
                {
                    Console.WriteLine($"  {displayFormatter(item)}");
                }
            }
        }
    }
}
