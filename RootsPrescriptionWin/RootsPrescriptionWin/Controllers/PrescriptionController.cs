using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace RootsPrescriptionWin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly ILogger<PrescriptionController> _logger;

        public PrescriptionController(ILogger<PrescriptionController> logger)
        {
            _logger = logger;
            //_logger.BeginScope("PrescriptionController");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> Ping()
        {
            const string message = "--CHANGE ME--";

            //_logger.Information(helloWorld);
            return message;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string> Fetch(string id)
        {
            if (id == null || id == "404")
            {
                return NotFound();
            } else
            {
                _logger.LogInformation("Request: " + id);
                return "Request: " + id;
            }

        }

    }
}
