namespace VBAN_Studio.Common.Audio
{
    public interface IAudioDevice : IDisposable
    {
        int Id { get; set; }

        string Name { get; }
        int SampleRate { get; }
        int Channels { get; }
        bool IsActive { get; }

        void Start();
        void Stop();
        string GetStatus();
    }
    public abstract class AudioDevice : IAudioDevice
    {
        public int Id { get; set; }
        public string Name { get; protected set; }
        public int SampleRate { get; }
        public int Channels { get; }
        public bool IsActive { get; protected set; }

        protected AudioDevice(string name, int sampleRate, int channels)
        {
            Name = name;
            SampleRate = sampleRate;
            Channels = channels;
            IsActive = false;
        }

        public abstract void Start();
        public abstract void Stop();
        public abstract string GetStatus();
        public abstract void Dispose();
    }


}
