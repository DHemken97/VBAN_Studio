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

            if (tokens.Length < 3)
            {
                Console.WriteLine("Usage: Map <input> <output> [bus]");
                return;
            }

            string inputDevice = tokens[0];
            string outputDevice = tokens[1];
            string? bus = tokens.Length > 3 ? tokens[2] : null; // Optional bus

            try
            {
                if (!routingManager.DeviceExists(inputDevice))
                {
                    Console.WriteLine($"Error: Input device '{inputDevice}' not found.");
                    return;
                }

                if (!routingManager.DeviceExists(outputDevice))
                {
                    Console.WriteLine($"Error: Output device '{outputDevice}' not found.");
                    return;
                }

                if (bus != null && !routingManager.BusExists(bus))
                {
                    Console.WriteLine($"Error: Bus '{bus}' not found.");
                    return;
                }

                if (bus != null)
                {
                    routingManager.AddRouteToBus(inputDevice, bus);
                    routingManager.AddRouteToOutput(bus, outputDevice);
                    Console.WriteLine($"Mapped {inputDevice} → {bus} → {outputDevice}");
                }
                else
                {
                    routingManager.AddRoute(inputDevice, outputDevice);
                    Console.WriteLine($"Mapped {inputDevice} → {outputDevice}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while mapping: {ex.Message}");
            }
        }
    }
}
