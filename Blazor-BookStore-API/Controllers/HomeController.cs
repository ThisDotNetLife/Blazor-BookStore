using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor_BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;

// Links To Swagger-Generated Pages:  https://localhost:44345/swagger/v1/swagger.json
//                                    https://localhost:44345/swagger/

namespace Blazor_BookStore_API.Controllers {
    /// <summary>
    /// This is a test API controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    
    public class HomeController : ControllerBase {
        /// <summary>
        /// To ensure this API is up and running, open (https://localhost:44345/api/home) in a browser.
        /// </summary>
        /// <returns></returns>

        private readonly ILoggerService _logger;

        public HomeController(ILoggerService logger) {
            _logger = logger;
        }

        /// <summary>
        /// Returns simple message to indicate the API is up and running.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index() {
            _logger.LogInfo("Default home controller action method (api/home) was called.");
            return Ok("Hello World!");
        }

        /// <summary>
        /// GET: https://localhost:44345/api/home/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public string Get(int id) {
            _logger.LogDebug($"Default home controller action method (api/home/{id}) was called.");
            return "value";
        }

        /// <summary>
        /// POST api/home
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public void Post([FromBody] string value) {
            _logger.LogError("User trying to post to home controller action  via (api/home/).");
        }

        /// <summary>
        /// PUT api/home/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        /// <summary>
        /// DELETE api/home/5
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(int id) {
            _logger.LogWarn($"User attempting to delete record (api/home/{id}).");
        }
    }
}
