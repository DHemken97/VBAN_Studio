﻿using NAudio.CoreAudioApi;

namespace VBAN_Studio.Common.Audio
{
    public abstract class AudioOutput : AudioDevice
    {
        public abstract void Write(byte[] data);

        public abstract string GetDisplayName();

        public abstract void Start();
        public static void ListAudioDevices()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            Console.WriteLine("Available Output Devices:");
            int i = 0;
            foreach (var device in devices)
            {
                Console.WriteLine($"Output Device: {i++}:{device.FriendlyName}");

            }
        }

        public abstract string BuildDeviceCommand();

    }
}
