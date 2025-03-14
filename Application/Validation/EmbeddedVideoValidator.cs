﻿using FluentValidation;
using YorubaOrganization.Core.Dto.Request;

namespace Application.Validation
{
    public class EmbeddedVideoValidator : AbstractValidator<EmbeddedVideoDto>
    {
        public EmbeddedVideoValidator()
        {
            RuleFor(x => x.VideoId)
                .NotEmpty().WithMessage("VideoId cannot be empty");

            RuleFor(x => x.Caption)
                .NotEmpty().WithMessage("Caption cannot be empty");
        }
    }
}
