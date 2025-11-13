using FluentValidation;
using Guess.Application.DTOs;
using Guess.Domain.Entities;
using Guess.Domain.Enums;

namespace Guess.Application.Validators;

/// <summary>
/// Validator for creating a new game session
/// </summary>
public class CreateGameSessionDtoValidator : AbstractValidator<CreateGameSessionDto>
{
    public CreateGameSessionDtoValidator()
    {
        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("Invalid difficulty level");

        RuleFor(x => x.CustomMinRange)
            .GreaterThan(0).WithMessage("Minimum range must be greater than 0")
            .When(x => x.CustomMinRange.HasValue);

        RuleFor(x => x.CustomMaxRange)
            .GreaterThan(x => x.CustomMinRange).WithMessage("Maximum range must be greater than minimum range")
            .LessThanOrEqualTo(10000).WithMessage("Maximum range cannot exceed 10,000")
            .When(x => x.CustomMaxRange.HasValue && x.CustomMinRange.HasValue);

        RuleFor(x => x.CustomMaxAttempts)
            .GreaterThan(0).WithMessage("Maximum attempts must be greater than 0")
            .LessThanOrEqualTo(50).WithMessage("Maximum attempts cannot exceed 50")
            .When(x => x.CustomMaxAttempts.HasValue);

        // Validation rule to ensure both custom min and max are provided together
        RuleFor(x => x)
            .Must(x => (x.CustomMinRange.HasValue && x.CustomMaxRange.HasValue) || 
                      (!x.CustomMinRange.HasValue && !x.CustomMaxRange.HasValue))
            .WithMessage("Both minimum and maximum range must be provided when using custom ranges");
    }
}

/// <summary>
/// Validator for making a guess
/// </summary>
public class MakeGuessDtoValidator : AbstractValidator<MakeGuessDto>
{
    public MakeGuessDtoValidator()
    {
        RuleFor(x => x.GuessedNumber)
            .GreaterThan(0).WithMessage("Guessed number must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Guessed number cannot exceed 10,000");
    }
}