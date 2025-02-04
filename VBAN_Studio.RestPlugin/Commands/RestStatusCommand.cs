using VBAN_Studio.Common;
using VBAN_Studio.Common.Command;

namespace VBAN_Studio.RestPlugin.Commands
{
    public class RestStatusCommand : Command
    {
        public override string Name => "RestStatus";

        public override string Description => "Displays Status info for the Rest Server";

        public override void Execute(VbanStudioEnvironment _environment, string[] tokens)
        {
            Console.WriteLine("No Data...");

        }
    }
}
