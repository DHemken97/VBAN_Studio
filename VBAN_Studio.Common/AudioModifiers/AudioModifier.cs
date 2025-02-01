namespace VBAN_Studio.Common.AudioModifiers
{
    public abstract class AudioModifier
    {
        public abstract byte[] Apply(byte[] data);
    }

}
