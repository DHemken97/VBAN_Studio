namespace VBAN_Studio.Common.Audio
{
    public interface IAudioDevice : IDisposable
    {

    }
    public abstract class AudioDevice : IAudioDevice
    {
        public readonly int Id;

        public abstract string Name { get; protected set; }

        public abstract void Dispose();

        public abstract string GetConfigCommand();

        public abstract float[] Process(float[] buffer, int sampleRate);

     
    }
}
