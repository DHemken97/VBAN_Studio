//using System.Net.Sockets;
//using System.Text;
//using VBAN_Studio.Common.Audio;

//namespace VBAN_Studio.Core.AudioOutputs
//{
//    public class VbanOutput : AudioOutput
//    {
//        private readonly string streamName;
//        private readonly string targetIp;
//        private readonly int targetPort;
//        private readonly UdpClient udpClient;
//        private readonly byte[] header;
//        private int frameCounter;

//        private const int SampleRate = 44100;
//        private const int Channels = 2;
//        private const int BitsPerSample = 16;
//        private const int AudioChunkSize = 412;
//        private const int VbanHeaderSize = 28;

//        public override string Name { get => streamName; protected set => throw new NotImplementedException(); }

//        public VbanOutput(string name, string ip, int port = 6980)
//        {
//            streamName = name;
//            targetIp = ip;
//            targetPort = port;
//            udpClient = new UdpClient();
//            header = GenerateHeader();
//        }

//        // Generates the static header for the VBAN packet
//        private byte[] GenerateHeader()
//        {
//            byte[] vbanHeader = new byte[VbanHeaderSize];
//            Encoding.ASCII.GetBytes("VBAN").CopyTo(vbanHeader, 0);

//            vbanHeader[4] = 0x10; // PCM format, stream index 0
//            vbanHeader[5] = 0x66; // Placeholder sample rate
//            vbanHeader[6] = 0x01; // Sample rate index for 44100 Hz
//            vbanHeader[7] = 0x01; // 2 channels, 16-bit PCM

//            byte[] streamNameBytes = Encoding.ASCII.GetBytes(streamName);
//            Array.Copy(streamNameBytes, 0, vbanHeader, 8, Math.Min(16, streamNameBytes.Length));

//            UpdateFrameCounter(vbanHeader);
//            return vbanHeader;
//        }

//        // Creates a new header for each frame, ensuring unique frame counters
//        private byte[] GenerateFrameHeader()
//        {
//            byte[] frameHeader = (byte[])header.Clone();
//            UpdateFrameCounter(frameHeader);
//            return frameHeader;
//        }

//        // Updates the frame counter in the header
//        private void UpdateFrameCounter(byte[] vbanHeader)
//        {
//            vbanHeader[24] = (byte)(frameCounter & 0xFF);
//            vbanHeader[25] = (byte)(frameCounter >> 8 & 0xFF);
//            vbanHeader[26] = (byte)(frameCounter >> 16 & 0xFF);
//            vbanHeader[27] = (byte)(frameCounter >> 24 & 0xFF);
//            frameCounter++;
//        }

//        // Sends the audio data in chunks, each with a header
//        private void SendVBANPacket(byte[] audioData, int bytesRecorded)
//        {
//            int offset = 0;
//            while (offset + AudioChunkSize <= bytesRecorded)
//            {
//                byte[] frameHeader = GenerateFrameHeader();
//                byte[] packet = new byte[VbanHeaderSize + AudioChunkSize];

//                Array.Copy(frameHeader, 0, packet, 0, VbanHeaderSize);
//                Array.Copy(audioData, offset, packet, VbanHeaderSize, AudioChunkSize);

//                udpClient.Send(packet, packet.Length, targetIp, targetPort);
//                offset += AudioChunkSize;
//            }
//        }

//        // Writes data to the VBAN output
//        public override void Write(byte[] data)
//        {
//            SendVBANPacket(data, data.Length);
//        }

//        // Returns the display name for the VBAN output
//        public override string GetDisplayName()
//        {
//            return $"{streamName} @ {targetIp}:{targetPort}";
//        }

//        // Starts the VBAN output (no operation in this implementation)
//        public override void Start() { }

//        // Builds the command to configure the VBAN output device
//        public override string BuildDeviceCommand()
//        {
//            return $"vban {streamName}@{targetIp}";
//        }

//        // Disposes of the UDP client to release resources
//        public override void Dispose()
//        {
//            udpClient?.Dispose();
//        }

//        // Gets the configuration command for the device (not implemented)
//        public override string GetConfigCommand()
//        {
//            throw new NotImplementedException();
//        }

//        // Processes the audio buffer (not implemented)
//        public override float[] Process(float[] buffer, int sampleRate)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
