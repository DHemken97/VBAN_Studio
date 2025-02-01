using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace VBAN_Studio.Common.AudioOutputs
{
    public class HardwareOutput : AudioOutput
    {
        private readonly WaveOutEvent waveOut;
        private readonly BufferedWaveProvider waveProvider;

        public HardwareOutput(int sampleRate, int bitsPerSample, int channels, int deviceIndex = -1)
        {
            waveOut = new WaveOutEvent { DeviceNumber = deviceIndex };
            waveProvider = new BufferedWaveProvider(new WaveFormat(sampleRate, bitsPerSample, channels));
            waveOut.Init(waveProvider);
        }

        public override void Dispose()
        {
            try
            {
                waveOut?.Stop();
            }
            finally
            {
                waveOut?.Dispose();
            }
        }

        public override void Write(byte[] bytes)
        {
            waveProvider.AddSamples(bytes, 0, bytes.Length);
        }

        public override void Start() => waveOut.Play();

        public override string GetDisplayName()
        {
            using var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            return waveOut.DeviceNumber >= 0 && waveOut.DeviceNumber < devices.Count
                ? devices[waveOut.DeviceNumber]?.DeviceFriendlyName ?? "Unknown Device"
                : "Unknown Device";
        }

        public override string BuildDeviceCommand() => $"hw {waveOut.DeviceNumber}";
    }
}
