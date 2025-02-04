using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VBAN_Studio.Common.Audio;

namespace VBAN_Studio.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioRouterController : ControllerBase
    {
        [HttpGet]
        public List<IAudioInput> GetInputDevices()
        {
            return VbanStudioContext.VbanStudioEnvironment.RoutingManager.AudioInputs;
        }
    }
}
