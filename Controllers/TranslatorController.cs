using Microsoft.AspNetCore.Mvc;

public class TranslateRequest
{
    public string Text { get; set; }
    public string ToLanguage { get; set; }
}

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TranslatorController : ControllerBase
    {
        private readonly ServiceTranslator _serviceTranslator;

        public TranslatorController(ServiceTranslator serviceTranslator)
        {
            _serviceTranslator = serviceTranslator;
        }

        [HttpPost]
        public async Task<IActionResult> Translate([FromBody] TranslateRequest info)
        {
            var translated = await _serviceTranslator.TranslateText(info.Text, info.ToLanguage);
            return Ok(new { original = info.Text, translated = translated });
        }
    }
}
