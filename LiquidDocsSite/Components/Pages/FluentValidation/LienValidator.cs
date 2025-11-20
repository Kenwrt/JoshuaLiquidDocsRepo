using FluentValidation;
using LiquidDocsData.Models;

public class LienValidator : AbstractValidator<Lien>
{
    public LienValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(120);
    }
}