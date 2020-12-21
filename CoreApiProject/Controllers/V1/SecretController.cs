using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiProject.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApiProject.Controllers.V1
{    
    [ApiController]
    [ApiKeyAuth]
    public class SecretController : ControllerBase
    {
        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok("Nothing to find, move ahead");
        }
    }
}