﻿using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using YorubaOrganization.Core.Dto;

namespace Api.Utilities
{
    public class CustomSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.BaseType != null &&
                context.Type.BaseType.IsGenericType &&
                context.Type.BaseType.GetGenericTypeDefinition() == typeof(CharacterSeparatedString<>))
            {
                schema.Type = "string";
                // Additional configurations if needed
            }
        }
    }
}