using VBAN_Studio.Common.Command;

namespace VBAN_Studio.Core.Commands
{
    public class ListCommand : Command
    {
        public override string Name => "List";

        public override string Description => "Generates a list";

        public override void Execute(VbanStudioEnvironment _environment, string[] tokens)
        {
            Console.WriteLine("List [options]");
        }
    }
}
