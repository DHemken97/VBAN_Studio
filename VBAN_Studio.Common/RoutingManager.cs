using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.Common
{
    public class RoutingManager : IDisposable
    {
        public List<AudioDevice> InputDevices { get; } = new();
        public List<AudioBus> Buses { get; } = new();
        public List<AudioDevice> OutputDevices { get; } = new();

        public void AddRoute(string inputDevice, string outputDevice)
        {
            var input = InputDevices.FirstOrDefault(d => d.Name == inputDevice);
            var output = OutputDevices.FirstOrDefault(d => d.Name == outputDevice);

            if (input == null)
            {
                Console.WriteLine($"Error: Input device '{inputDevice}' not found.");
                return;
            }

            if (output == null)
            {
                Console.WriteLine($"Error: Output device '{outputDevice}' not found.");
                return;
            }

            DirectRoute(input, output);
            Console.WriteLine($"Route added: {inputDevice} → {outputDevice}");
        }

        public void AddRouteToBus(string inputDevice, string bus)
        {
            var input = InputDevices.FirstOrDefault(d => d.Name == inputDevice);
            var busObj = Buses.FirstOrDefault(b => b.Name == bus);

            if (input == null)
            {
                Console.WriteLine($"Error: Input device '{inputDevice}' not found.");
                return;
            }

            if (busObj == null)
            {
                Console.WriteLine($"Error: Bus '{bus}' not found.");
                return;
            }

            busObj.AddInput(input);
            Console.WriteLine($"Input device {inputDevice} added to bus {bus}");
        }

        public void AddRouteToOutput(string bus, string outputDevice)
        {
            var busObj = Buses.FirstOrDefault(b => b.Name == bus);
            var output = OutputDevices.FirstOrDefault(d => d.Name == outputDevice);

            if (busObj == null)
            {
                Console.WriteLine($"Error: Bus '{bus}' not found.");
                return;
            }

            if (output == null)
            {
                Console.WriteLine($"Error: Output device '{outputDevice}' not found.");
                return;
            }

            busObj.AddOutput(output);
            Console.WriteLine($"Bus {bus} routed to output device {outputDevice}");
        }

        public bool BusExists(string bus)
        {
            return Buses.Any(b => b.Name == bus);
        }

        public void Connect(AudioDevice source, AudioDevice destination)
        {
            if (destination is AudioBus bus)
            {
                bus.AddInput(source);
            }
            else
            {
                destination.Process(source.Process(new float[1024], 44100), 44100);
            }
        }

        public bool DeviceExists(string deviceName)
        {
            return InputDevices.Any(d => d.Name == deviceName) ||
                   OutputDevices.Any(d => d.Name == deviceName);
        }

        public void DirectRoute(AudioDevice input, AudioDevice output)
        {
            // Assume that DirectRoute means an instant audio data pass-through
            output.Process(input.Process(
                new float[1024], 44100), 44100);
            Console.WriteLine($"Direct routing: {input.Name} → {output.Name}");
        }

        public void Dispose()
        {
        }
    }
}
