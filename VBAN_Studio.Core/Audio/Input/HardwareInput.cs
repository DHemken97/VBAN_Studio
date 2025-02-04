using NAudio.Wave;
using System.Threading.Channels;
using System;
using VBAN_Studio.Common.Attribute;
using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.Core.Audio.Input
{
    [RegisterInputType("HW")]
    public class HardwareInput : AudioInput
    {
        public override event EventHandler<AudioDataArgs> DataReceived;

        private static List<HardwareInput> _hardwareInputs = new List<HardwareInput>();
        public int DeviceId { get; private set; }
        public byte[] Buffer { get; }
        public WaveInEvent WaveIn { get; private set; }
        public static HardwareInput GetDevice(int deviceId)
        {
            if (_hardwareInputs.Any(x => x.DeviceId == deviceId))
                return _hardwareInputs.First(x => x.DeviceId == deviceId);


            //Get Name, Rate, and channels from hardware device
            var device = WaveInEvent.GetCapabilities(deviceId);
            var name = device.ProductName;
            var channels = device.Channels;
            var sampleRate = 44100;//Static for testing
            
            return new HardwareInput(deviceId,name, sampleRate, channels);
        }
        public HardwareInput(int deviceId): base(string.Empty,44100,2)
        {
            DeviceId = deviceId;
            Buffer = new byte[412 * 256];//static for testing

            WaveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(44100, 16, 2),
                BufferMilliseconds = 20,
                DeviceNumber = DeviceId
            };

            WaveIn.DataAvailable += OnDataAvailable;

        }
        public HardwareInput(int deviceId, string name, int sampleRate, int channels, int bufferMs = 20) : base(name, sampleRate, channels)
        {
            DeviceId = deviceId;
            Buffer = new byte[412 * 256];//static for testing

            WaveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(sampleRate, 16, channels),
                BufferMilliseconds = bufferMs,
                DeviceNumber = DeviceId
            };

            WaveIn.DataAvailable += OnDataAvailable;
        }
        public HardwareInput(int deviceId, int sampleRate, int channels, int bufferMs = 20) : base("", sampleRate, channels)
        {

            var device = WaveInEvent.GetCapabilities(deviceId);
            var name = device.ProductName;
            Name = name;
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
