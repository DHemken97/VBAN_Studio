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

        public HardwareOutput(int deviceId) : this(deviceId, 44100, 2) { }
        public HardwareOutput(int deviceId, int sampleRate, int channels) : this(deviceId, GetDeviceName(deviceId), sampleRate, channels, 16, 20) { }
        public HardwareOutput(int deviceId, int sampleRate, int channels, int bitDepth) : this(deviceId, GetDeviceName(deviceId), sampleRate, channels, bitDepth, 20) { }
        public HardwareOutput(int deviceId, int sampleRate, int channels, int bitDepth, int bufferMs) : this(deviceId, GetDeviceName(deviceId), sampleRate, channels, bitDepth, bufferMs) { }
        public HardwareOutput(int deviceId, string name, int sampleRate, int channels, int bitDepth, int bufferMs) : base(name, sampleRate, channels)
        {
            DeviceId = deviceId;
            Buffer = new byte[412 * 256];//static for testing
            WaveOut = new WaveOutEvent { DeviceNumber = deviceId };
            WaveProvider = new BufferedWaveProvider(new WaveFormat(sampleRate, bitDepth, channels));
            WaveOut.Init(WaveProvider);
        }


        public static string GetDeviceName(int deviceId)
        {

            using var enumerator = new MMDeviceEnumerator();
                var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                var device = devices[deviceId];
                var name = device.FriendlyName;
            return name;
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
