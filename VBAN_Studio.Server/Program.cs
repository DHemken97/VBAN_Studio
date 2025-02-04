


using VBAN_Studio.Common;
using VBAN_Studio.Common.Audio;
using VBAN_Studio.Core.Audio.Input;
using VBAN_Studio.Core.Audio.Output;

var env = new VbanStudioEnvironment();
/*
var hwi = HardwareInput.GetDevice(0);
var hwo = HardwareOutput.GetDevice(5);
env.RoutingManager.Map(hwi, hwo);

env.RoutingManager.Start();
*/
var exit = false;
while (!exit)
{
    var command = Console.ReadLine();
    if (command == "exit")  
        exit = true;
    env.CommandManager.ExecuteCommand(command);
}

env.RoutingManager.Dispose();