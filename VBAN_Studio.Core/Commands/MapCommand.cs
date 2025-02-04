using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VBAN_Studio.Common;
using VBAN_Studio.Common.Audio;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Core.Commands
{
    public class MapCommand : Command
    {
        public override string Name => "Map";

        public override string Description => "Connects devices";

        public override void Execute(VbanStudioEnvironment _environment, string[] tokens)
        {
            var inputCommandIndex = tokens.ToList().IndexOf(tokens.FirstOrDefault(x => x.StartsWith("--Input")));
            var inputCommandType = tokens[inputCommandIndex + 1];
            var outputCommandIndex = tokens.ToList().IndexOf(tokens.FirstOrDefault(x => x.StartsWith("--Output")));
            var outputCommandType = tokens[outputCommandIndex + 1];


            var inputType = _environment.CommandManager.GetInputType(inputCommandType.ToLower());
            var outputType = _environment.CommandManager.GetOutputType(inputCommandType.ToLower());

            var inputParams = new List<string>();
            var outputParams = new List<string>();

            for (int i = inputCommandIndex+2; i < outputCommandIndex; i++)
                inputParams.Add(tokens[i]);
            for (int i = outputCommandIndex+2; i < tokens.Length; i++)
                outputParams.Add(tokens[i]);

            _environment.RoutingManager.Map(inputType, inputParams, outputType, outputParams);

        }
    }
}
