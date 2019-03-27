using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExceptionHandlingSample.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExceptionHandlingSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            // We don't capture the exception and we let it bubble up to the global handler...
            throw new Exception("Something really bad happened!");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            try
            {
                _logger.LogInformation("Processing request");
                throw new Exception("Something really bad happened!");
            }
            catch (Exception ex)
            {
                // We captured the exception, so we can enrich it with extra data.
                ex.AddData(new EventId(500, "GeneralError"), new { id }, (s, e) => $"An error occurred: {e.Message}. Id: {s.id}");
                ex.Data["Foo"] = "bar";
                throw;
            }            
        }
    }
}
