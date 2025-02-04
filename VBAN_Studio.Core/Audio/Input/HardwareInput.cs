using NAudio.Wave;
using VBAN_Studio.Common.Attribute;
using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.Core.Audio.Input
{
    [RegisterInputType("HW")]
    public class HardwareInput : AudioInput
    {
        public override event EventHandler<AudioDataArgs> DataReceived;
        public int DeviceId { get; private set; }
        public byte[] Buffer { get; }
        public WaveInEvent WaveIn { get; private set; }

        public HardwareInput(int deviceId): this(deviceId, 44100, 2) { }
        public HardwareInput(int deviceId, int sampleRate, int channels) : this(deviceId,GetDeviceName(deviceId), sampleRate, channels,16,20) { }
        public HardwareInput(int deviceId, int sampleRate, int channels, int bitDepth) : this(deviceId, GetDeviceName(deviceId), sampleRate, channels, bitDepth, 20) { }
        public HardwareInput(int deviceId, int sampleRate, int channels,int bitDepth, int bufferMs) : this(deviceId, GetDeviceName(deviceId), sampleRate, channels, bitDepth, bufferMs) { }
        public HardwareInput(int deviceId, string name, int sampleRate, int channels, int bitDepth, int bufferMs) : base(name, sampleRate, channels)
        {
            DeviceId = deviceId;
            Id = deviceId;
            Buffer = new byte[bufferMs * channels * (sampleRate/1000) * (bitDepth / 8)];

            WaveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(sampleRate, bitDepth, channels),
                BufferMilliseconds = bufferMs,
                DeviceNumber = DeviceId
            };

            WaveIn.DataAvailable += OnDataAvailable;
        }
        public static string GetDeviceName(int deviceId)
        {

            var device = WaveInEvent.GetCapabilities(deviceId);
            var name = device.ProductName;
            return name;
        }


        private int frame;
        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            DataReceived?.Invoke(sender, new AudioDataArgs(this, e.Buffer, frame++));
        }

        public override void Dispose()
        {
            Stop();
            WaveIn?.Dispose();
        }

        public override string GetStatus()
        {
            return IsActive ? "Started" : "Stopped";
        }

        public override void Start()
        {
            if (IsActive) return;
            WaveIn?.StartRecording();
            IsActive = true;
            
        }

        public override void Stop()
        {
            WaveIn?.StopRecording();
            IsActive = false;
        }
    }
}
