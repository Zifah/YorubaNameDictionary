using Application.Services.Words;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers.Words
{
    [Route("api/v1/words/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminAndLexicographers")]
    public class DefinitionsController(WordEntryService entryService) : ControllerBase
    {
        /// <summary>
        /// Returns English definitions for each requested word, irrespective of review status.
        /// </summary>
        [HttpGet("in-english")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetEnglishDefinitions([FromQuery] string words)
        {
            if (string.IsNullOrWhiteSpace(words))
            {
                return BadRequest("At least one word is required in query parameter words.");
            }

            var requestedWords = words
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToArray();

            var result = await entryService.GetEnglishDefinitionsOf(requestedWords);
            return Ok(result);
        }

        /// <summary>
        /// Adds English definitions that do not already exist for each supplied word.
        /// New definitions created via this endpoint are marked as needing review.
        /// </summary>
        [HttpPost("in-english")]
        [ProducesResponseType(typeof(IDictionary<string, object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddEnglishDefinitions([FromBody] IDictionary<string, string>? payload)
        {
            if (payload == null || payload.Count == 0)
            {
                return BadRequest("Payload must be a dictionary of word to English definition.");
            }

            var statuses = await entryService.AddEnglishDefinitionsAsync(payload);
            var response = statuses.ToDictionary(kvp => kvp.Key, kvp => (object)new { status = kvp.Value });

            return Ok(response);
        }
    }
}
