namespace VBAN_Studio.Common.Audio
{
    public class AudioStream : AudioDevice
    {
        public int Id { get; }
        public AudioInput Input { get; set; }
        public List<AudioModifier> Modifiers { get; }
        public AudioOutput Output { get; set; }
        public override string Name { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public AudioStream(int id)
        {
            Id = id;
            Modifiers = new List<AudioModifier>();
        }

        public void Start()
        {
            if (Input == null || Output == null) return;

            Input.DataAvailable += OnInputAvailable;
            Input.Start();
            Output.Start();

            Console.WriteLine($"Stream {Id} Mapped {Input.GetDisplayName()} -> {Output.GetDisplayName()}");
        }

        private void OnInputAvailable(object sender, AudioPacketEventArgs e)
        {
            var data = e.bytes;
            foreach (AudioModifier modifier in Modifiers)
                data = modifier.Apply(data);
            Output.Write(data);
        }

        public override string GetConfigCommand()
        {
            return $"map {Input?.BuildDeviceCommand()} {Output?.BuildDeviceCommand()} stream {Id}";
        }

        public override void Dispose()
        {
        }

        public override float[] Process(float[] buffer, int sampleRate)
        {
            throw new NotImplementedException();
        }
    }
}
