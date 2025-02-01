using System.Reflection;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Common
{
    public class CommandManager
    {
        private readonly Dictionary<string, ICommand> _commands = new();

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

                    foreach (var type in commandTypes)
                    {
                        if (Activator.CreateInstance(type) is ICommand command)
                        {
                            _commands[command.Name.ToLower()] = command;
                            Console.WriteLine($"Loaded command: {command.Name} from {dll}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load plugin {dll}: {ex.Message}");
                }
            }
        }

        internal void RegisterEnvironment(VbanStudioEnvironment vbanStudioEnvironment)
        {
            _environment = vbanStudioEnvironment;
        }
    }

}
