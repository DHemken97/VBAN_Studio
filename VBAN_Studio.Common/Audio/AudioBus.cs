namespace VBAN_Studio.Common.Audio
{
    public class AudioBus : AudioInput
    {
        private readonly List<AudioInput> _inputs;
        private readonly byte[] _buffer;
        private int _bufferIndex;
        public int Id { get; private set; }
        public AudioBus(int id, List<AudioInput> inputs, int packetSize = 412)
        {
            Id = id;
            _inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
            PacketSize = packetSize;
            _buffer = new byte[packetSize * inputs.Count];
            _bufferIndex = 0;

            // Subscribe to the DataAvailable event for each input device
            foreach (var input in _inputs)
            {
                input.DataAvailable += OnInputDataAvailable;
            }
        }

        // Event to trigger when data is available from any of the inputs
        public override event EventHandler<AudioPacketEventArgs> DataAvailable;

        private void OnInputDataAvailable(object sender, AudioPacketEventArgs e)
        {
            // Add the incoming data to the buffer
            Array.Copy(e.bytes, 0, _buffer, _bufferIndex, e.bytes.Length);
            _bufferIndex += e.bytes.Length;

            // If the buffer is full, trigger the DataAvailable event
            if (_bufferIndex >= PacketSize)
            {
                var outputData = new byte[PacketSize];
                Array.Copy(_buffer, outputData, PacketSize);
                _bufferIndex -= PacketSize;

                DataAvailable?.Invoke(this, new AudioPacketEventArgs(outputData));
            }
        }

        // Start each input device
        public override void Start()
        {
            foreach (var input in _inputs)
            {
                input.Start();
            }
        }

        // Dispose of each input device
        public override void Dispose()
        {
            foreach (var input in _inputs)
            {
                input.Dispose();
            }
        }

        // Not applicable for AudioBus, since it combines multiple inputs
        public override byte[] Read(int length)
        {
            // This should not be used directly on the AudioBus as it doesn't read from a single input.
            throw new InvalidOperationException("Use DataAvailable event to get combined audio data.");
        }

        // Write method is not necessary since AudioBus acts as an input
        public override void Write(byte[] bytes)
        {
            throw new InvalidOperationException("AudioBus does not support direct write operations.");
        }

        // Returns the combined display name of all input devices
        public override string GetDisplayName()
        {
            return $"Bus {Id}";
            // return string.Join(" + ", _inputs.Select(input => input.GetDisplayName()));
        }

        // Return a custom command for the bus
        public override string BuildDeviceCommand()
        {
            return $"audioBus {string.Join(" ", _inputs.Select(input => input.BuildDeviceCommand()))}";
        }

        // Returns the count of how many devices are in the bus
        public override int GetReadCount()
        {
            return _inputs.Sum(input => input.GetReadCount());
        }

        public override string GetConfigCommand()
        {
            throw new NotImplementedException();
        }
    }

}
