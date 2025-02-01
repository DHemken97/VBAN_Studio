using VBAN_Studio.Common;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Core.Commands
{
    public class ClsCommand : Command
    {
        public override string Name => "Clear";

        public override string Description => "Clear Screen";

        public override void Execute(VbanStudioEnvironment _environment, string[] tokens)
        {
           Console.Clear();
        }
    }
}
