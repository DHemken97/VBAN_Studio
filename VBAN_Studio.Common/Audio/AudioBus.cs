using VBAN_Studio.Common.Attribute;
using VBAN_Studio.Common.Attributes;

namespace VBAN_Studio.Common.Audio
{
    public interface IAudioBus : IAudioInput, IAudioOutput
    {
        void AddSource(IAudioInput source);
        void RemoveSource(IAudioInput source);
    }
    [RegisterInputType("Bus"), RegisterOutputType("Bus")]
    public class AudioBus : IAudioBus
    {
        public int Id { get; set; }
        public string Name { get; protected set; }
        public int SampleRate { get; }
        public int Channels { get; }
        public bool IsActive { get; protected set; }

        public List<IAudioInput> Inputs { get; } = new List<IAudioInput>();
        private readonly object syncLock = new object();

        public AudioBus(string name, int sampleRate, int channels)
        {
            Name = name;
            SampleRate = sampleRate;
            Channels = channels;
        }

        public event EventHandler<AudioDataArgs> DataReceived;

        public void AddSource(IAudioInput source)
        {
            lock (syncLock)
            {
                if (Inputs.Contains(source)) return;

                Inputs.Add(source);
                source.DataReceived += ProcessAudio;
            }
        }

        public void RemoveSource(IAudioInput source)
        {
            lock (syncLock)
            {
                var index = Inputs.IndexOf(source);
                if (index < 0) return;

                var s = Inputs[index];
                s.DataReceived -= ProcessAudio;
                s.Stop();
                s.Dispose();
                Inputs.RemoveAt(index);
            }
        }

        private void ProcessAudio(object? sender, AudioDataArgs e)
        {
            byte[] inputData = e.AudioData;
            MixAudio(inputData);
        }
        int frame;
        private void MixAudio(byte[] audioData)
        {
            foreach (var input in Inputs)
            {
            }

            DataReceived?.Invoke(this,new AudioDataArgs(this,audioData,frame++));
        }

        public void Dispose()
        {
            lock (syncLock)
            {
                foreach (var input in Inputs.ToList())
                {
                    RemoveSource(input);
                }
            }
            Stop();
        }

        public string GetStatus()
        {
            return IsActive ? $"Streaming {Inputs.Count} Inputs" : "Stopped";
        }

        public void Start()
        {
            lock (syncLock)
            {
                Inputs.ForEach(x => x.Start());
                IsActive = true;
            }
        }

        public void Stop()
        {
            lock (syncLock)
            {
                Inputs.ForEach(x => x.Stop());
                IsActive = false;
            }
        }

        public void ProcessAudio(byte[] audio)
        {
            throw new InvalidOperationException("For Audio Busses, please add the AudioSource using AddSource(IAudioInput source)");
        }
    }
}
