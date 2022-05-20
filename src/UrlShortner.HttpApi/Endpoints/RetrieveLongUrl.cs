using System.ComponentModel.DataAnnotations;
using System.Net;

using Microsoft.AspNetCore.Mvc;

namespace UrlShortner.HttpApi.Endpoints
{
    [ApiController]
    public class RetrieveLongUrl : ControllerBase
    {
        [HttpGet("~/{shortUrl}"), ProducesResponseType(301), ProducesResponseType(400)]
        public IActionResult Handle([Required(AllowEmptyStrings = false)] string shortUrl)
        {
            if (Program.ShortLinksMap.TryGetValue(shortUrl!, out string? longUrl))
            {
                var result = new JsonResult(new { url = longUrl })
                {
                    StatusCode = (int)HttpStatusCode.MovedPermanently
                };

                Response.Headers.Location = longUrl;
                return result;
            }

            return NotFound();
        }
    }
}