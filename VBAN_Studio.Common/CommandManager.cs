using System.Reflection;
using VBAN_Studio.Common.Attribute;
using VBAN_Studio.Common.Attributes;
using VBAN_Studio.Common.Audio;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Common
{
    public class CommandManager
    {
        private readonly Dictionary<string, ICommand> _commands = new();
        private readonly Dictionary<string, Type> _inputTypes = new();
        private readonly Dictionary<string, Type> _outputTypes = new();

        private VbanStudioEnvironment _environment;

        public void RegisterCommand(ICommand command)
        {
            _commands[command.Name.ToLower()] = command;
        }

        public void ExecuteCommand(string input)
        {
            var parts = input.Split(' ');
            var commandName = parts[0].ToLower();
            var args = parts.Skip(1).ToArray();

            if (_commands.TryGetValue(commandName, out var command))
            {
                command.Execute(_environment,args);
            }
            else if (commandName == "help" || commandName=="?")
                ShowHelp();
            else
            {
                Console.WriteLine($"Unknown command: {commandName}");
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine("Available commands:");
            foreach (var cmd in _commands.Values)
            {
                Console.WriteLine($"  {cmd.Name} - {cmd.Description}");
            }
        }

        public void RegisterCommandsFromPlugins(string pluginDirectory)
        {
            if (!Directory.Exists(pluginDirectory))
            {
                Console.WriteLine($"Plugin directory not found: {pluginDirectory}");
                return;
            }

            var dllFiles = Directory.GetFiles(pluginDirectory, "*.dll");

            foreach (var dll in dllFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    var commandTypes = assembly.GetTypes()
                        .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsAbstract);
                    var audioInputTypes = assembly.GetTypes().Where(t => typeof(IAudioInput).IsAssignableFrom(t) && !t.IsAbstract);
                    var audioOutputTypes = assembly.GetTypes().Where(t => typeof(IAudioOutput).IsAssignableFrom(t) && !t.IsAbstract);



                    foreach (var type in commandTypes)
                    {
                        if (Activator.CreateInstance(type) is ICommand command)
                        {
                            _commands[command.Name.ToLower()] = command;
                            Console.WriteLine($"Loaded command: {command.Name} from {dll}");
                        }
                    }



                    foreach (var type in audioInputTypes)
                    {
                        var attribute = type.GetCustomAttributes(typeof(RegisterInputTypeAttribute), false)
                                            .FirstOrDefault() as RegisterInputTypeAttribute;

                        if (attribute != null)
                        {
                            _inputTypes.Add(attribute.CommandType.ToLower(), type);
                        }
                    }

                    foreach (var type in audioOutputTypes)
                    {
                        var attribute = type.GetCustomAttributes(typeof(RegisterOutputTypeAttribute), false)
                                            .FirstOrDefault() as RegisterOutputTypeAttribute;

                        if (attribute != null)
                        {
                            _outputTypes.Add(attribute.CommandType.ToLower(), type);
                        }
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load plugin {dll}: {ex.Message}");
                }
            }
        }

        public void RegisterEnvironment(VbanStudioEnvironment vbanStudioEnvironment)
        {
            _environment = vbanStudioEnvironment;
        }

        public Type GetInputType(string command) => _inputTypes[command];
        public Type GetOutputType(string command) => _outputTypes[command];
    }

}
