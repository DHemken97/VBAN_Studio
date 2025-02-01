namespace VBAN_Studio.Common.Audio
{
    public interface IAudioDevice : IDisposable
    {

    }
    public abstract class AudioDevice : IAudioDevice
    {
        public string Name { get; internal set; }

        public abstract void Dispose();

        public abstract string GetConfigCommand();

        public float[] Process(float[] buffer, int sampleRate)
        {
            throw new NotImplementedException();
        }

     
    }
}
