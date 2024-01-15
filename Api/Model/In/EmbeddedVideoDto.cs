﻿using Core.Entities.NameEntry.Collections;
using Core.Entities;
using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Model.In
{
    /// <summary>
    /// Add field validations
    /// </summary>
    public record EmbeddedVideoDto(string VideoId, string Caption)
    {
    }

}
