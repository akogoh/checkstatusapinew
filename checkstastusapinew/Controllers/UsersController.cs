using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using checkstastusapinew.Services;
using checkstastusapinew.Model;

namespace checkstastusapinew.Controllers
{
    [ApiController]
    [Route("api/riskdbusers")]
    public class UsersController : ControllerBase
    {
        private readonly RiskDbService _db;

        public UsersController(RiskDbService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // GET /api/riskdbusers?top=100
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int top = 100)
        {
            try
            {
                var max = Math.Min(Math.Max(top, 1), 1000);
                var rows = await _db.GetTopUsersAsync(max);
                return Ok(rows);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to query users", detail = ex.Message });
            }
        }

        // GET /api/riskdbusers/search?username=alice
        [HttpGet("search")]
        public async Task<IActionResult> GetUserByUsername([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("username query parameter is required.");

            try
            {
                var user = await _db.GetUserByUsernameAsync(username);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to query user", detail = ex.Message });
            }
        }
        // GET /api/riskdbusers/riskregister?page=1&pageSize=20
        [HttpGet("riskregister")]
        public async Task<IActionResult> GetRiskRegisterPage([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            try
            {
                var (data, totalCount) = await _db.GetRiskRegisterPageAsync(page, pageSize);
                return Ok(new { totalCount, page, pageSize, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to query risk register", detail = ex.Message });
            }
        }
        // POST /api/riskdbusers/risk
        [HttpPost("risk")]
        public async Task<IActionResult> PostRisk([FromForm] checkstastusapinew.Model.RiskInputWithFileDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Directorate))
                return BadRequest("Directorate is required.");
            if (input.RiskRegisterId == null && string.IsNullOrWhiteSpace(input.RiskName))
                return BadRequest("Either RiskRegisterId or RiskName is required.");

            try
            {
                // Handle image upload if present
                if (input.ImageFile != null && input.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(input.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await input.ImageFile.CopyToAsync(stream);
                    }
                    // Set the ImageUrls property to the relative path for client access
                    input.ImageUrls = $"/UploadedFiles/{uniqueFileName}";
                }

                var newId = await _db.InsertRiskAsync(
                    input.RiskRegisterId,
                    input.RiskName,
                    input.RiskDescription,
                    input.RiskCategory,
                    input.Directorate,
                    input.RiskOwner,
                    input.RiskChampionId,
                    input.Frequency,
                    input.FrequencyPeriod,
                    input.Impact,
                    input.RiskScore,
                    input.Region,
                    input.District,
                    input.PhoneNumber,
                    input.Landmark,
                    input.UserId,
                    input.UserName,
                    input.ImageUrls
                );
                return Ok(new { success = true, id = newId, imageUrl = input.ImageUrls });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}
