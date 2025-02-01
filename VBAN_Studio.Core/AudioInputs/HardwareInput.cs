using NAudio.Wave;
using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.Core.AudioInputs
{
    public class HardwareInput : AudioInput
    {
        private const int BUFFER_MULTIPLIER = 256;

        private volatile int Available;
        private volatile int SeekPosition;
        private volatile int WritePosition;
        private readonly byte[] Buffer;
        private readonly WaveInEvent waveIn;

        public override event EventHandler<AudioPacketEventArgs> DataAvailable;

        public HardwareInput(int sampleRate, int bitsPerSample, int channels, int buffer = 15, int deviceIndex = -1)
        {
            Buffer = new byte[PacketSize * BUFFER_MULTIPLIER];

            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(sampleRate, bitsPerSample, channels),
                BufferMilliseconds = buffer,
                DeviceNumber = deviceIndex
            };

            waveIn.DataAvailable += OnWaveInDataAvailable;
        }

        private void OnWaveInDataAvailable(object sender, WaveInEventArgs e)
        {
            Write(e.Buffer.Take(e.BytesRecorded).ToArray());
        }

        public override void Dispose()
        {
            try
            {
                waveIn.StopRecording();
                waveIn.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disposing hardware input: {ex.Message}");
            }
        }

        public override int GetReadCount() => Available;

        public override byte[] Read(int length)
        {
            if (length > Available)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Underrun");
                return Read(Available);
            }

            byte[] data = new byte[length];
            int bytesToEnd = Buffer.Length - SeekPosition;

            if (length <= bytesToEnd)
            {
                Array.Copy(Buffer, SeekPosition, data, 0, length);
            }
            else
            {
                Array.Copy(Buffer, SeekPosition, data, 0, bytesToEnd);
                Array.Copy(Buffer, 0, data, bytesToEnd, length - bytesToEnd);
            }

            SeekPosition = (SeekPosition + length) % Buffer.Length;
            Available -= length;

            return data;
        }

        public override void Write(byte[] bytes)
        {
            int bytesToEnd = Buffer.Length - WritePosition;
            int length = bytes.Length;

            if (length <= bytesToEnd)
            {
                Array.Copy(bytes, 0, Buffer, WritePosition, length);
            }
            else
            {
                Array.Copy(bytes, 0, Buffer, WritePosition, bytesToEnd);
                Array.Copy(bytes, bytesToEnd, Buffer, 0, length - bytesToEnd);
            }

            WritePosition = (WritePosition + length) % Buffer.Length;
            Available += length;

            OnDataReceived();
        }

        private void OnDataReceived()
        {
            int packetCount = GetReadCount() / PacketSize;

            for (int i = 0; i < packetCount; i++)
            {
                if (GetReadCount() >= PacketSize)
                {
                    var data = Read(PacketSize);
                    DataAvailable?.Invoke(this, new AudioPacketEventArgs(data));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{GetDisplayName()} - Underrun");
                }
            }
        }

        public override void Start()
        {
            try
            {
                waveIn.StartRecording();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting recording: {ex.Message}");
            }
        }

        public override string GetDisplayName()
        {
            try
            {
                return WaveInEvent.GetCapabilities(waveIn.DeviceNumber).ProductName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving device name: {ex.Message}");
                return "Unknown Device";
            }
        }

        public override string BuildDeviceCommand() => $"hw {waveIn.DeviceNumber}";
    }
}
