namespace VBAN_Studio.Common
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
