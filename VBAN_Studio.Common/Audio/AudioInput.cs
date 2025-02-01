using NAudio.Wave;

namespace VBAN_Studio.Common.Audio
{
    public abstract class AudioInput : IDisposable
    {
        protected int PacketSize = 412;
        public abstract event EventHandler<AudioPacketEventArgs> DataAvailable;
        public abstract byte[] Read(int length);
        public abstract void Write(byte[] bytes);
        public abstract int GetReadCount();
        public abstract void Dispose();
        public abstract void Start();

        public abstract string GetDisplayName();

        public static void ListAudioDevices()
        {

            int deviceCount = WaveInEvent.DeviceCount;
            Console.WriteLine("Available Input Devices:");

            for (int i = 0; i < deviceCount; i++)
            {
                string deviceName = WaveInEvent.GetCapabilities(i).ProductName;
                Console.WriteLine($"Input Device {i}: {deviceName}");
            }
        }

        public abstract string BuildDeviceCommand();

    }
}
