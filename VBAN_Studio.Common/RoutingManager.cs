using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.Common
{
    public class RoutingManager:IDisposable
    {
        public List<AudioDevice> InputDevices { get; } = new();
        public List<AudioBus> Buses { get; } = new();
        public List<AudioDevice> OutputDevices { get; } = new();

        public void Connect(AudioDevice source, AudioDevice destination)
        {
           // if (destination is AudioBus bus)
            //    bus.Inputs.Add(source);
        }

        public void DirectRoute(AudioDevice input, AudioDevice output)
        {
           // output.Process(input.Process(new float[1024], 44100), 44100);
        }

        public void Dispose()
        {
        }
    }

}
