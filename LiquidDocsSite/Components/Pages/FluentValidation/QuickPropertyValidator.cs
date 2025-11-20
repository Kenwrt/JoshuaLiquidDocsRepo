using FluentValidation;
using LiquidDocsData.Models;

public class QuickPropertyValidator : AbstractValidator<PropertyRecord>
{
    public QuickPropertyValidator()
    {
        RuleFor(x => x.FullAddress)
            .NotEmpty().WithMessage("Street Address is required")
            .MaximumLength(120);
    }
}