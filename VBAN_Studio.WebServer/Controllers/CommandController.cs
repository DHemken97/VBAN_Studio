using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VBAN_Studio.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        [HttpGet]
        public string GetCommands()
        {
            var commands = VbanStudioContext.VbanStudioEnvironment.CommandManager.GetCommands();
            return string.Join(Environment.NewLine, commands.Select(x => $"{x.Name} - {x.Description}"));
        }
        [HttpPost("execute/{command}")]
        public string ExecuteCommand(string command) {
            
            VbanStudioContext.VbanStudioEnvironment.CommandManager.ExecuteCommand(command);
            return "";
        }
    }
}
