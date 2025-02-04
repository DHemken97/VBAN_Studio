namespace VBAN_Studio.Common.Audio
{
    public interface IRoutingManager : IDisposable
    {
        void Map(IAudioInput input, IAudioOutput output);
        void UnMap(IAudioInput input, IAudioOutput output);
        void UnMap(int streamIndex);
        void Start();
        void Stop();
    }
    public class RoutingManager : IRoutingManager
    {
        private readonly List<AudioStream> audioStreams = new List<AudioStream>();
        private bool isRoutingActive;

        public void Map(IAudioInput input, IAudioOutput output)
        {
            if (input == null || output == null)
                throw new ArgumentNullException("Input and Output cannot be null");

            var audioStream = new AudioStream(input, output);
            audioStreams.Add(audioStream);
        }

        public void UnMap(IAudioInput input, IAudioOutput output)
        {
            var audioStream = audioStreams.FirstOrDefault(s => s.Input == input && s.Output == output);
            if (audioStream != null)
            {
                audioStream.Dispose();
                audioStreams.Remove(audioStream);
            }
        }

        public void UnMap(int streamIndex)
        {
            if (streamIndex < 0 || streamIndex >= audioStreams.Count)
                throw new ArgumentOutOfRangeException(nameof(streamIndex), "Invalid stream index.");

            var audioStream = audioStreams[streamIndex];
            audioStream.Dispose();
            audioStreams.RemoveAt(streamIndex);
        }

        public void Start()
        {
            if (isRoutingActive) return;

            foreach (var stream in audioStreams)
            {
                stream.Start();
            }
            isRoutingActive = true;
        }

        public void Stop()
        {
            if (!isRoutingActive) return;

            foreach (var stream in audioStreams)
            {
                stream.Stop();
            }
            isRoutingActive = false;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
