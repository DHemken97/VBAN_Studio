using System.Collections.Concurrent;
using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.AudioEngine
{
    public static class VBAN_Studio_Environment
    {
        private static readonly ConcurrentBag<AudioDevice> AudioDevices = new();

        public static void Init()
        {
            ListDevices();
            AudioDevices.Clear();
            Console.WriteLine("Environment Started");


            //var in0 = new HardwareInput(44100, 16, 2,15,0);
            //var bus0 = new AudioBus(0,new() { in0 });
            //var bus1 = new AudioBus(1,new() { bus0 });
            //var out0 = new HardwareOutput(44100, 16, 2, 5);
            //var device0 = new AudioDevice(1) { Input = bus1, Output = out0 };
            //device0.Start();
            //AudioDevices.Add(device0);


        }

        public static void Shutdown()
        {
            foreach (var device in AudioDevices.ToArray())
            {
                RemoveDevice(device);
            }
        }

        public static void SaveEnvironment(string path)
        {
            try
            {
                File.WriteAllText(path, GetEnvironment());
                Console.WriteLine($"Environment saved to {path}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to save environment: {ex.Message}");
            }
        }

        public static string GetEnvironment()
        {
            return string.Join(Environment.NewLine, AudioDevices.Select(x => x.GetConfigCommand()));
        }

        public static void LoadEnvironment(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"File not found: {path}");
                    return;
                }

                Shutdown();
                Init();

                var commands = File.ReadAllLines(path);
                AudioDevices.Clear();

                foreach (var command in commands)
                {
                    //CommandInterpreter.InterpretCommand(command);
                }
                Console.WriteLine($"Environment loaded from {path}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to load environment: {ex.Message}");
            }
        }

        internal static void AddDevice(AudioDevice device)
        {
            AudioDevices.Add(device);
        }

        public static List<AudioDevice> GetDevices()
        {
            return AudioDevices.ToList();
        }

        public static void RemoveDevice(AudioDevice audioDevice)
        {
            if (audioDevice == null)
            {
                Console.WriteLine("Attempted to remove a null device.");
                return;
            }


            if (audioDevice is AudioStream)
            {
                var stream = (AudioStream)audioDevice;
                stream.Input.Dispose();
                stream.Output.Dispose();
            }


            AudioDevices.TryTake(out _);
        }

        public static void ListDevices()
        {
            AudioInput.ListAudioDevices();
            Console.WriteLine();
            AudioOutput.ListAudioDevices();
            Console.WriteLine();
        }
    }
}
