using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.SaveGameEditor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        [HttpGet("{path}")]
        public IActionResult Get(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            return File(System.IO.File.ReadAllBytes(path), "application/octet-stream");
        }
    }
}
