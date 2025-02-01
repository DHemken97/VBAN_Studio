using VBAN_Studio.Common.AudioInputs;
using VBAN_Studio.Common.AudioModifiers;
using VBAN_Studio.Common.AudioOutputs;

namespace VBAN_Studio.Common
{
    public class AudioStream
    {
        public int Id { get; }
        public AudioInput Input { get; set; }
        public List<AudioModifier> Modifiers { get; }
        public AudioOutput Output { get; set; }

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

            Console.WriteLine($"Mapped {Input.GetDisplayName()} -> {Output.GetDisplayName()}");
        }

        private void OnInputAvailable(object sender, AudioPacketEventArgs e)
        {
            Output.Write(e.bytes);
        }

        public string GetConfigCommand()
        {
            return $"map {Input?.BuildDeviceCommand()} {Output?.BuildDeviceCommand()} stream {Id}";
        }
    }
}
