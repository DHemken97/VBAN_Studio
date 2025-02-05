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
        int mods;
        float[][] buffer;

        private void MixAudio(byte[] audioData)
        {
            var newBuffer = ConvertToFloats(audioData);

            if (buffer == null)
                buffer = newBuffer;
            else
                buffer = AverageBuffer(buffer, newBuffer);

            mods++;
            if (mods < Inputs.Count) return;

            DataReceived?.Invoke(this, new AudioDataArgs(this, audioData, frame++));
            mods = 0;
            buffer = null; // Reset buffer for next batch
        }

        private float[][] AverageBuffer(float[][] buffer, float[][] floats)
        {
            if (buffer.Length != floats.Length)
                throw new InvalidOperationException("Buffer size mismatch.");

            for (int i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i];
                var b = floats[i];

                if (a.Length != b.Length)
                    throw new InvalidOperationException("Channel length mismatch.");

                for (int j = 0; j < a.Length; j++)
                    buffer[i][j] = (a[j] + b[j]) / 2f;
            }
            return buffer;
        }

        private float[][] ConvertToFloats(byte[] audioData)
        {
            var floats = new float[audioData.Length / 2];
            for (int i = 0; i < floats.Length; i++)
                floats[i] = BitConverter.ToInt16(audioData, i * 2) / 32768f; // Normalize

            var channelValues = new float[Channels][];
            for (int i = 0; i < Channels; i++)
                channelValues[i] = new float[floats.Length / Channels];

            for (int i = 0; i < floats.Length; i += Channels)
                for (int j = 0; j < Channels; j++)
                    channelValues[j][i / Channels] = floats[i + j];

            return channelValues;
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
