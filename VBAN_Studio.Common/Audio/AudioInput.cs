namespace VBAN_Studio.Common.Audio
{
    public interface IAudioInput : IAudioDevice
    {
        public event EventHandler<AudioDataArgs> DataReceived;

    }
    public abstract class AudioInput : AudioDevice, IAudioInput
    {
        protected AudioInput(string name, int sampleRate, int channels) : base(name, sampleRate, channels)
        {
        }

        public abstract event EventHandler<AudioDataArgs> DataReceived;
    }
}
