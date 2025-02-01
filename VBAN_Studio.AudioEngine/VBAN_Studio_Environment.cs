using System.Collections.Concurrent;
using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.AudioEngine
{
    public static class VBAN_Studio_Environment
    {
        private static readonly ConcurrentBag<AudioStream> AudioStreams = new();

        public static void Init()
        {
            ListDevices();
            AudioStreams.Clear();
            Console.WriteLine("Environment Started");


            //var in0 = new HardwareInput(44100, 16, 2,15,0);
            //var bus0 = new AudioBus(0,new() { in0 });
            //var bus1 = new AudioBus(1,new() { bus0 });
            //var out0 = new HardwareOutput(44100, 16, 2, 5);
            //var stream0 = new AudioStream(1) { Input = bus1, Output = out0 };
            //stream0.Start();
            //AudioStreams.Add(stream0);


        }

        public static void Shutdown()
        {
            foreach (var stream in AudioStreams.ToArray())
            {
                RemoveStream(stream);
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
            return string.Join(Environment.NewLine, AudioStreams.Select(x => x.GetConfigCommand()));
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
                AudioStreams.Clear();

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

        internal static void AddStream(AudioStream stream)
        {
            AudioStreams.Add(stream);
        }

        public static List<AudioStream> GetStreams()
        {
            return AudioStreams.ToList();
        }

        public static void RemoveStream(AudioStream audioStream)
        {
            if (audioStream == null)
            {
                Console.WriteLine("Attempted to remove a null stream.");
                return;
            }

            audioStream.Input.Dispose();
            audioStream.Output.Dispose();

            AudioStreams.TryTake(out _);
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
