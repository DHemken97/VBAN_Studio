namespace VBAN_Studio.Common.Audio
{
    public class AudioPacketEventArgs : EventArgs
    {
        public byte[] bytes;

        public AudioPacketEventArgs(byte[] bytes)
        {
            this.bytes = bytes;
        }
    }
}
