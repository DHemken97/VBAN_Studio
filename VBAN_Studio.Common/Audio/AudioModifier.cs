namespace VBAN_Studio.Common.Audio
{
    public interface IAudioModifier
    {
        public byte[] Apply(byte[] data);
    }
    public abstract class AudioModifier : IAudioModifier
    {
        public abstract byte[] Apply(byte[] data);
    }

}
