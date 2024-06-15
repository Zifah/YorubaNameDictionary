﻿using Core.Dto.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class EmbeddedVideoValidator : AbstractValidator<EmbeddedVideoDto>
    {
        public EmbeddedVideoValidator()
        {
            RuleFor(x => x.VideoId)
                .NotEmpty().WithMessage("VideoId cannot be empty")
                .NotNull().WithMessage("VideoId cannot be null");

            RuleFor(x => x.Caption)
                .NotEmpty().WithMessage("Caption cannot be empty")
                .NotNull().WithMessage("Caption cannot be null");
        }
    }
}
