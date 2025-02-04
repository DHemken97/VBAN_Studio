using NAudio.CoreAudioApi;
using NAudio.Wave;
using VBAN_Studio.Common.Attributes;
using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.Core.Audio.Output
{
    [RegisterOutputType("HW")]
    public class HardwareOutput : AudioOutput
    {
        private static List<HardwareOutput> _hardwareOutputs = new List<HardwareOutput>();
        public int DeviceId { get; private set; }
        public byte[] Buffer { get; }
        public WaveOutEvent WaveOut { get; private set; }
        private readonly BufferedWaveProvider WaveProvider;

        public static HardwareOutput GetDevice(int deviceId)
        {
            if (_hardwareOutputs.Any(x => x.DeviceId == deviceId))
                return _hardwareOutputs.First(x => x.DeviceId == deviceId);

            using var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);


            //Get Name, Rate, and channels from hardware device
            var device = devices[deviceId];
            var name = device.FriendlyName;
            var channels = 2; //static for testing
            var sampleRate = 44100;//Static for testing

            return new HardwareOutput(deviceId, name, sampleRate, channels);
        }
        public HardwareOutput(int deviceId, string name, int sampleRate, int channels, int bufferMs = 20) : base(name, sampleRate, channels)
        {
            DeviceId = deviceId;
            Buffer = new byte[412 * 256];//static for testing

            WaveOut = new WaveOutEvent { DeviceNumber = deviceId };
            WaveProvider = new BufferedWaveProvider(new WaveFormat(sampleRate, 16, channels));
            WaveOut.Init(WaveProvider);

        }
        public HardwareOutput(int deviceId, int sampleRate, int channels, int bufferMs = 20) : base("", sampleRate, channels)
        {
            DeviceId = deviceId;
            Buffer = new byte[412 * 256];//static for testing

            WaveOut = new WaveOutEvent { DeviceNumber = deviceId };
            WaveProvider = new BufferedWaveProvider(new WaveFormat(sampleRate, 16, channels));
            WaveOut.Init(WaveProvider);
            using var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            var device = devices[deviceId];
            var name = device.FriendlyName;
            Name = name;

        }


        public override void Dispose()
        {
            Stop();
            WaveOut?.Dispose();
        }

        public override string GetStatus()
        {
            return IsActive ? "Started" : "Stopped";
        }

        public override void Start()
        {
            WaveOut?.Play();
            IsActive = true;

        }

        public override void Stop()
        {
            WaveOut?.Stop();
            IsActive = false;
        }

        public override void ProcessAudio(byte[] audio)
        {
            WaveProvider.AddSamples(audio, 0, audio.Length);
        }
    }
}
