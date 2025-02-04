using VBAN_Studio.Common;

var env = new VbanStudioEnvironment();

var exit = false;
while (!exit)
{
    var command = Console.ReadLine();
    if (command == "exit")  
        exit = true;
    env.CommandManager.ExecuteCommand(command);
}

env.RoutingManager.Dispose();