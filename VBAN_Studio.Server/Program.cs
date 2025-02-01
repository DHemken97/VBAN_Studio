


using VBAN_Studio.Common;

var routingManager = new RoutingManager();
var commandManager = new CommandManager();
commandManager.RegisterCommandsFromPlugins("./Plugins");

var exit = false;
while (!exit)
{
    var command = Console.ReadLine();
    if (command == "exit")  
        exit = true;
    commandManager.ExecuteCommand(command);
}

routingManager.Dispose();