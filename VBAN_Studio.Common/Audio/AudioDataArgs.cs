namespace VBAN_Studio.Common.Audio
{
    public class AudioDataArgs : EventArgs
    {
        public IAudioInput Source { get; }
        public byte[] AudioData { get; }
        public int FrameCounter { get; }
        public DateTime Timestamp { get; }

        public AudioDataArgs(IAudioInput source, byte[] audioData, int frameCounter)
        {
            Source = source;
            AudioData = audioData ?? throw new ArgumentNullException(nameof(audioData));
            FrameCounter = frameCounter;
            Timestamp = DateTime.UtcNow;
        }
    }

}
