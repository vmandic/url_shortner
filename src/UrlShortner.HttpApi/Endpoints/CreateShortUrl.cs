using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;

using UrlShortner.HttpApi.Helpers;

namespace UrlShortner.HttpApi.Endpoints
{
    [ApiController]
    public class CreateShortUrl : Controller
    {
        public class CreateShortUrlRequest
        {
            [NotNull, Required(AllowEmptyStrings = false)]
            public string? Url { get; set; }

            /// <summary>
            /// Forces a valid URL from the given input even if the input is a simple string like 'google' resulting in 'http://google.com'. Only for faster testing purposes.
            /// </summary>
            /// <exception cref="FormatException">
            /// Thrown if for some reason we hit an edge case and cant form a valid URL after all.
            /// </exception>
            string GetValidUrl()
            {
                if (Url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) == false &&
                    Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) == false)
                {
                    Url = $"http://{Url}";
                }

                if (IsValidUrl(Url) == false)
                {
                    Url = $"{Url}.com";
                }

                return IsValidUrl(Url) == false
                    ? throw new InvalidUrlFormatException($"Invalid URL format of '{nameof(CreateShortUrlRequest)}.{nameof(Url)}' property value.")
                    : Url;
            }

            /// <summary>
            /// Gets an 8 chars fixed length CRC32 hash value of the given <see cref="Url"/> property value.
            /// </summary>
            public string GetUrlCrc32Hash()
            {
                byte[] asciiBytes = Encoding.ASCII.GetBytes(GetValidUrl()!);
                string crc32Hash = new Crc32Helper().Get(asciiBytes).ToString("X");

                return crc32Hash;
            }

            /// <summary>
            /// Validates if a passed input string is a valid URL. <para/>
            /// Will satisfy for: <para/>
            /// http(s)://www.example.com <para/>
            /// http(s)://stackoverflow.example.com <para/>
            /// http(s)://www.example.com/page <para/>
            /// http(s)://www.example.com/page?id=1 <para/>
            /// http(s)://www.example.com/page#start <para/>
            /// http(s)://www.example.com:8080 <para/>
            /// http(s)://127.0.0.1 <para/>
            /// 127.0.0.1 <para/>
            /// www.example.com <para/>
            /// example.com
            /// </summary>
            /// <remarks>ref: https://stackoverflow.com/a/56116499/1534753</remarks>
            static bool IsValidUrl(string url)
            {
                string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
                var regex = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                return regex.IsMatch(url);
            }
        }

        [HttpPost("~/"), Consumes("application/json"), ProducesResponseType(200), ProducesResponseType(400)]
        public IActionResult Handle([FromBody] CreateShortUrlRequest requestPayload)
        {
            string shortUrl;
            try
            {
                shortUrl = requestPayload.GetUrlCrc32Hash();
            }
            catch (InvalidUrlFormatException fe)
            {
                return BadRequest(new { detail = fe.Message });
            }

            bool hashSavedSuccessfully = Program.ShortLinksMap.TryAdd(shortUrl, requestPayload.Url!);

            if (hashSavedSuccessfully == false)
            {
                return BadRequest(new { detail = "Please modify the long URL. Our system does not support hash collision evasion strategy at the moment. We advise that you add a random query string parameter." });
            }

            return Json(new { short_url = $"/{shortUrl}", requestPayload.Url });
        }
    }
}
