using VBAN_Studio.Common;
using VBAN_Studio.Common.AudioInputs;
using VBAN_Studio.Common.AudioModifiers;
using VBAN_Studio.Common.AudioOutputs;

namespace VBAN_Studio.AudioEngine
{
    public static class CommandInterpreter
    {
        public static void InterpretCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                Console.WriteLine("Invalid command.");
                return;
            }

            var tokens = command.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                Console.WriteLine("Invalid command format.");
                return;
            }

            string primaryCommand = tokens[0].ToLower();

            try
            {
                switch (primaryCommand)
                {
                    case "map":
                        HandleMapCommand(tokens);
                        break;

                    case "unmap":
                        HandleUnmapCommand(tokens);
                        break;

                    case "list":
                        HandleListCommand(tokens);
                        break;

                    case "reinit":
                        ReinitializeEnvironment();
                        break;
                    case "restart":                        
                        RestartEnvironment();
                        break;
                    case "mastervolume":
                        handleMasterVolumeCommand(tokens);
                        break;

                    case "save":
                        HandleSaveLoadCommand(tokens, save: true);
                        break;

                    case "load":
                        HandleSaveLoadCommand(tokens, save: false);
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {command}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing command: {ex.Message}");
            }
        }

        private static void handleMasterVolumeCommand(string[] tokens)
        {
            if (tokens.Length<2)
            {
                Console.WriteLine("[get|set]");
                return;
            }
            if (tokens[1] == "get")
            {
                Console.WriteLine(VolumeModifier.MasterVolume);
            }
            if (tokens[1] == "set")
            {
                var val = float.Parse(tokens[2]);
                VolumeModifier.MasterVolume = val;
            }
        }

        private static void RestartEnvironment()
        {
            var defaultFileName = "restart";
            HandleSaveLoadCommand(["", defaultFileName], true);
            ReinitializeEnvironment();
            HandleSaveLoadCommand(["", defaultFileName], false);

        }

        private static void HandleUnmapCommand(string[] tokens)
        {
            if (tokens.Length < 2)
            {
                Console.WriteLine("Usage: unmap <stream|id>");
                return;
            }

            if (tokens[1].Equals("stream", StringComparison.OrdinalIgnoreCase) && tokens.Length > 2 && int.TryParse(tokens[2], out int streamId))
            {
                UnmapById(streamId);
            }
            else
            {
                UnmapByCommand(string.Join(" ", tokens));
            }
        }

        private static void HandleListCommand(string[] tokens)
        {
            if (tokens.Length < 2)
            {
                Console.WriteLine("Usage: list <streams|devices>");
                return;
            }

            switch (tokens[1].ToLower())
            {
                case "streams":
                    Console.WriteLine(VBAN_Studio_Environment.GetEnvironment());
                    break;
                case "devices":
                    VBAN_Studio_Environment.ListDevices();
                    break;
                default:
                    Console.WriteLine("Invalid list command. Use 'list streams' or 'list devices'.");
                    break;
            }
        }

        private static void ReinitializeEnvironment()
        {
            VBAN_Studio_Environment.Shutdown();
            VBAN_Studio_Environment.Init();
            Console.WriteLine("Environment reinitialized.");
        }

        private static void HandleSaveLoadCommand(string[] tokens, bool save)
        {
            if (tokens.Length < 2)
            {
                Console.WriteLine($"Usage: {(save ? "save" : "load")} <filename>");
                return;
            }

            if (save)
                VBAN_Studio_Environment.SaveEnvironment(tokens[1]+".vbstd");
            else
                VBAN_Studio_Environment.LoadEnvironment(tokens[1] + ".vbstd");

            Console.WriteLine($"{(save ? "Saved" : "Loaded")} environment: {tokens[1]}");
        }

        private static void UnmapById(int streamId)
        {
            var stream = VBAN_Studio_Environment.GetStreams().FirstOrDefault(x => x.Id == streamId);
            if (stream != null)
            {
                VBAN_Studio_Environment.RemoveStream(stream);
                Console.WriteLine($"Stream {streamId} removed.");
            }
            else
            {
                Console.WriteLine($"Stream {streamId} not found.");
            }
        }

        private static void UnmapByCommand(string command)
        {
            var expectedCommand = command.Replace("unmap", "map") + $" stream {command.Split(" ").Last()}";
            var stream = VBAN_Studio_Environment.GetStreams().FirstOrDefault(x => x.GetConfigCommand() == expectedCommand);

            if (stream != null)
            {
                VBAN_Studio_Environment.RemoveStream(stream);
                Console.WriteLine("Stream removed.");
            }
            else
            {
                Console.WriteLine("Stream not found.");
            }
        }

        private static void HandleMapCommand(string[] tokens)
        {
            if (tokens.Length < 5)
            {
                Console.WriteLine("Invalid map command. Usage: map <hw|...> <index> <vban|hw> <destination>");
                return;
            }

            var existingStreams = VBAN_Studio_Environment.GetStreams();
            int id = existingStreams.Any() ? existingStreams.Max(x => x.Id) + 1 : 0;

            if (tokens.Length > 6 && tokens[5].Equals("stream", StringComparison.OrdinalIgnoreCase) && int.TryParse(tokens[6], out int providedId))
            {
                id = providedId;
            }

            var stream = new AudioStream(id);

            if (tokens[1].Equals("hw", StringComparison.OrdinalIgnoreCase) && int.TryParse(tokens[2], out int inputIndex))
            {
                stream.Input = new HardwareInput(44100, 16, 2, deviceIndex: inputIndex);
            }

            if (tokens[3].Equals("vban", StringComparison.OrdinalIgnoreCase))
            {
                var location = tokens[4].Split('@');
                if (location.Length == 2)
                {
                    stream.Output = new VbanOutput(location[0], location[1]);
                }
                else
                {
                    Console.WriteLine("Invalid VBAN output format. Expected format: <streamName>@<ip>");
                    return;
                }
            }
            else if (tokens[3].Equals("hw", StringComparison.OrdinalIgnoreCase) && int.TryParse(tokens[4], out int outputIndex))
            {
                stream.Output = new HardwareOutput(44100, 16, 2, deviceIndex: outputIndex);
            }

            stream.Start();
            VBAN_Studio_Environment.AddStream(stream);
            Console.WriteLine($"Stream {id} mapped successfully.");
        }
    }
}
