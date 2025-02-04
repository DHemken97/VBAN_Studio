namespace VBAN_Studio.Common.Audio
{
    public interface IAudioStream : IDisposable
    {
        void AddModifier(IAudioModifier modifier);
        void RemoveModifier(IAudioModifier modifier);
        void Start();
        void Stop();
        void ProcessAudio(object sender, AudioDataArgs e);
    }
    public class AudioStream : IAudioStream
    {
        public IAudioInput Input { get; }
        public IAudioOutput Output { get; }
        public List<IAudioModifier> Modifiers { get; } = new List<IAudioModifier>();
        private bool isActive;

        public AudioStream(IAudioInput input, IAudioOutput output)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Output = output ?? throw new ArgumentNullException(nameof(output));
            if (output.GetType() == typeof(AudioBus))
            {
                var bus = (AudioBus)output;
                bus.AddSource(input);
            } 
                
                else
            Input.DataReceived += ProcessAudio;
        }

        public void AddModifier(IAudioModifier modifier)
        {
            Modifiers.Add(modifier);
        }

        public void RemoveModifier(IAudioModifier modifier)
        {
            Modifiers.Remove(modifier);
        }

        public void Start()
        {
            if (isActive) return;

            Input.Start();
            Output.Start();
            isActive = true;
        }

        public void Stop()
        {
            if (!isActive) return;

            Input.Stop();
            Output.Stop();
            isActive = false;
        }

        public void ProcessAudio(object sender, AudioDataArgs e)
        {
            byte[] processedAudio = e.AudioData;

            Modifiers.ForEach(mod => processedAudio = mod.Apply(processedAudio));    
            
            Output.ProcessAudio(processedAudio);
        }

        public void Dispose()
        {
            Stop();
            Input.DataReceived -= ProcessAudio;
        }
    }
}
