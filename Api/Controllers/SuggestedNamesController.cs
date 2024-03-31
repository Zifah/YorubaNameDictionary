﻿using Api.Mappers;
using Application.Services;
using Core.Dto.Request;
using Core.Dto.Response;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SuggestedNamesController : ControllerBase
{
    private readonly SuggestedNameService _suggestedNameService;

    public SuggestedNamesController(SuggestedNameService suggestedNameService)
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
}
