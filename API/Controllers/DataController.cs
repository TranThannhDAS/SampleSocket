using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public DataController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("xsmb")]
        public IActionResult ReadJson()
        {
            try
            {
                // Đường dẫn tới file JSON trong wwwroot
                string filePath = Path.Combine(_env.WebRootPath, "dataMB.json");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "File not found." });
                }

                // Đọc nội dung file
                string jsonData = System.IO.File.ReadAllText(filePath);

                // Chuyển đổi JSON sang đối tượng
                var data = JsonConvert.DeserializeObject<DataMB>(jsonData);

                return Ok(new ResultData
                {
                    Data = data,
                    Message = "",
                    Success = true,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reading JSON file.", error = ex.Message });
            }
        }
        [HttpGet("xsmn")]
        public IActionResult ReadJsonMN()
        {
            try
            {
                // Đường dẫn tới file JSON trong wwwroot
                string filePath = Path.Combine(_env.WebRootPath, "dataMN1.json");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "File not found." });
                }

                // Đọc nội dung file
                string jsonData = System.IO.File.ReadAllText(filePath);

                // Chuyển đổi JSON sang đối tượng
                var data = JsonConvert.DeserializeObject<DataMB>(jsonData);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reading JSON file.", error = ex.Message });
            }
        }
    }
}
