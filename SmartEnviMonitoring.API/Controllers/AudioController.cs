using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;

namespace SmartEnviMonitoring.API.Controllers;

[Route("api/audio")]
[ApiController]
[AllowAnonymous]
public class AudioController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public AudioController(IWebHostEnvironment env){
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> UploadAudioFile(IFormFile file)
    {
        /* 
         * the content types of Wav are many
         * audio/wave
         * audio/wav
         * audio/x-wav
         * audio/x-pn-wav
         * see "https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types"
        */
        // if (file.ContentType != "audio/wave")
        // {
        //     return BadRequest("Wrong file type");
        // }
        var uploads = Path.Combine(_env.WebRootPath, "uploads");//uploads where you want to save data inside wwwroot
        var filePath = Path.Combine(uploads, file.FileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        return Ok("File uploaded successfully");
    }

}