
using VBAN_Studio.AudioEngine;

VBAN_Studio_Environment.Init();
//VBAN_Studio_Environment.SaveEnvironment("AudioStreams.json");
//VBAN_Studio_Environment.LoadEnvironment("AudioStreams.json");
var exit = false;
while (!exit)
{
    var command = Console.ReadLine();
    if (command == "exit")  
        exit = true;
    else
        CommandInterpreter.InterpretCommand(command);
}
VBAN_Studio_Environment.Shutdown();
