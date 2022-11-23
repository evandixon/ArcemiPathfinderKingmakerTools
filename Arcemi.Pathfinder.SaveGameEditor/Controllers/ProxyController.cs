using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web;

namespace Arcemi.Pathfinder.SaveGameEditor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        [HttpGet("{path}")]
        public IActionResult Get(string path)
        {
            path = HttpUtility.UrlDecode(path);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            return File(System.IO.File.ReadAllBytes(path), "application/octet-stream");
        }
    }
}
