using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Word2Rtf;
using Word2Rtf.Models;
using Word2Rtf.Exceptions;

namespace word2rtf.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConverterController : ControllerBase
    {
        private readonly ILogger<ConverterController> _logger;

        public ConverterController(ILogger<ConverterController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public JsonResult Post(object input)
        {
            try 
            {
                var payload = JsonConvert.DeserializeObject<Payload>(input.ToString());
                var function = new Function();
                var result = function.FunctionHandler(payload, null);
                return new JsonResult(result);
            }
            catch (ImbalancedLanguagesException ex) 
            {
                return new JsonResult(
                new
                {
                    errorType = typeof(ImbalancedLanguagesException).Name,
                    errorMessage = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }

        }

        [HttpGet]
        public JsonResult Get()
        {
            return new JsonResult(new { Status = "OK"});
        }
    }
}