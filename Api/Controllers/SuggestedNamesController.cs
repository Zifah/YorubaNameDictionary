﻿using Api.Mappers;
using Application.Services;
using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/v1/suggestions")]
[ApiController]
public class SuggestedNamesController : ControllerBase
{
    private readonly SuggestedNamesService _suggestedNameService;

    public SuggestedNamesController(SuggestedNamesService suggestedNameService)
    {
        _suggestedNameService = suggestedNameService;
    }

    [HttpGet]
    [Route("meta")]
    [ProducesResponseType(typeof(Dictionary<string, int>), 200)]
    public async Task<IActionResult> GetSuggestedMetaData()
    {
        var suggestname = await _suggestedNameService.CountAsync();

        return Ok(suggestname);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SuggestedNameDto), 200)]
    public async Task<IActionResult> SuggestName([FromBody] CreateSuggestedNameDto request)
    {
        try
        {
            var data = await _suggestedNameService
                .SuggestedNameAsync(request.MapToEntity());

            return Ok(data.MapToDto());
        }
        catch (Exception ex)
        {
            // Handle exception if save operation fails
            return StatusCode(500, $"Failed to save suggested name: {ex.Message}");
        }
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(SuggestedNameDto[]), 200)]
    public async Task<IActionResult> GetAllSuggestedNames()
    {
        var data = await _suggestedNameService.GetAllAsync();

        return Ok(data.MapToDtoCollection());
    }

    [HttpGet]
    [Route("id")]
    [ProducesResponseType(typeof(SuggestedNameDto[]), 200)]
    public async Task<IActionResult> GetSuggestedName(string id)
    {
        var data = await _suggestedNameService.GetAsync(id);

        if(data == null)
            return NotFound($"Suggested name with id: {id} not found");


        return Ok(data.MapToDto());
    }

    [HttpDelete]
    [Route("id")]
    public async Task<IActionResult> DeleteSuggestedName(string id)
    {
        var result = await _suggestedNameService.DeleteSuggestedNameAsync(id);

        if(result)
        {
            return NoContent();
        }

        return BadRequest($"Suggested name with id: {id} not found as a suggested name");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllSuggestions()
    {
        bool deletionResult = await _suggestedNameService.DeleteAllSuggestionsAsync();

        if (deletionResult)
        {
            return Ok("All suggested names has been deleted");
        }
        else
        {
            return StatusCode(500, "Failed to delete all suggested names");
        }
    }
}
