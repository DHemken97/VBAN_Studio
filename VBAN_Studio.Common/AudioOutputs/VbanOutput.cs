﻿using System.Net.Sockets;
using System.Text;

namespace VBAN_Studio.Common.AudioOutputs
{
    public class VbanOutput : AudioOutput
    {
        private readonly string streamName;
        private readonly string targetIp;
        private readonly int targetPort;
        private readonly UdpClient udpClient;
        private readonly byte[] header;
        private int frameCounter;

        private const int SampleRate = 44100;
        private const int Channels = 2;
        private const int BitsPerSample = 16;
        private const int AudioChunkSize = 412;
        private const int VbanHeaderSize = 28;

        public VbanOutput(string name, string ip, int port = 6980)
        {
            streamName = name;
            targetIp = ip;
            targetPort = port;
            udpClient = new UdpClient();
            header = GenerateHeader();
        }

        private byte[] GenerateHeader()
        {
            byte[] vbanHeader = new byte[VbanHeaderSize];
            Encoding.ASCII.GetBytes("VBAN").CopyTo(vbanHeader, 0);

            vbanHeader[4] = 0x10; // PCM format, stream index 0
            vbanHeader[5] = 0x66; // Placeholder sample rate
            vbanHeader[6] = 0x01; // Sample rate index for 44100 Hz
            vbanHeader[7] = 0x01; // 2 channels, 16-bit PCM

            byte[] streamNameBytes = Encoding.ASCII.GetBytes(streamName);
            Array.Copy(streamNameBytes, 0, vbanHeader, 8, Math.Min(16, streamNameBytes.Length));

            UpdateFrameCounter(vbanHeader);
            return vbanHeader;
        }

        private byte[] GenerateFrameHeader()
        {
            byte[] frameHeader = (byte[])header.Clone();
            UpdateFrameCounter(frameHeader);
            return frameHeader;
        }

        private void UpdateFrameCounter(byte[] vbanHeader)
        {
            vbanHeader[24] = (byte)(frameCounter & 0xFF);
            vbanHeader[25] = (byte)((frameCounter >> 8) & 0xFF);
            vbanHeader[26] = (byte)((frameCounter >> 16) & 0xFF);
            vbanHeader[27] = (byte)((frameCounter >> 24) & 0xFF);
            frameCounter++;
        }

        private void SendVBANPacket(byte[] audioData, int bytesRecorded)
        {
            int offset = 0;
            while (offset + AudioChunkSize <= bytesRecorded)
            {
                byte[] frameHeader = GenerateFrameHeader();
                byte[] packet = new byte[VbanHeaderSize + AudioChunkSize];

                Array.Copy(frameHeader, 0, packet, 0, VbanHeaderSize);
                Array.Copy(audioData, offset, packet, VbanHeaderSize, AudioChunkSize);

                udpClient.Send(packet, packet.Length, targetIp, targetPort);
                offset += AudioChunkSize;
            }
        }

        public override void Write(byte[] data)
        {
            SendVBANPacket(data, data.Length);
        }

        public override string GetDisplayName()
        {
            return $"{streamName} @ {targetIp}:{targetPort}";
        }

        public override void Start() { }

        public override string BuildDeviceCommand()
        {
            return $"vban {streamName}@{targetIp}";
        }

        public override void Dispose()
        {
            udpClient?.Dispose();
        }
    }
}
