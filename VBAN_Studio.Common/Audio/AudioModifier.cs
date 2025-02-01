namespace VBAN_Studio.Common.Audio
{
    public abstract class AudioModifier
    {
        public abstract byte[] Apply(byte[] data);
    }

}
