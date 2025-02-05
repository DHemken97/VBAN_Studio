using System;
using System.IO;
using System.Timers;
using VBAN_Studio.Common.Attribute;
using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.Core.Audio.Input
{
    [RegisterInputType("file")]
    public class FileInput : AudioInput
    {
        public byte[] Data { get; private set; }
        private readonly System.Timers.Timer _timer;
        private int _position;
        public FileInput(string filename) : this(filename,44100,2)
        {

        }
        public FileInput(string name, int sampleRate, int channels) : base(name, sampleRate, channels)
        {
            Data = File.ReadAllBytes(name);
            _position = 0;
            _timer = new System.Timers.Timer(20) { AutoReset = true };
            _timer.Elapsed += Timer_Elapsed;
        }
        int frame = 0;
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_position >= Data.Length)
            {
                Stop();
                return;
            }
            var bitDepth = 16;
            //int chunkSize = 1024; // Define appropriate chunk size
            int chunkSize = (SampleRate / (1000 / (bitDepth / 8) / Channels)) * (int)_timer.Interval; // Define appropriate chunk size
            byte[] chunk = new byte[Math.Min(chunkSize, Data.Length - _position)];
            Array.Copy(Data, _position, chunk, 0, chunk.Length);
            _position += chunk.Length;

            DataReceived?.Invoke(this, new AudioDataArgs(this,chunk,frame++));
        }

        public override event EventHandler<AudioDataArgs> DataReceived;

        public override void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        public override string GetStatus()
        {
            return _timer.Enabled ? "Playing" : "Stopped";
        }

        public override void Start()
        {
            _position = 0;
            _timer.Start();
        }

        public override void Stop()
        {
            _timer.Stop();
        }
    }
}
