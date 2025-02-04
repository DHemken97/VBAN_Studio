namespace VBAN_Studio.Common.Audio
{
    public interface IAudioOutput : IAudioDevice
    {
        public void ProcessAudio(byte[] audio);
    }
    public abstract class AudioOutput : AudioDevice, IAudioOutput
    {
        protected AudioOutput(string name, int sampleRate, int channels) : base(name, sampleRate, channels)
        {
        }

        public abstract void ProcessAudio(byte[] audio);
    }
}
