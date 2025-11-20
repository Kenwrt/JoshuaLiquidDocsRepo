using FluentValidation;
using LiquidDocsData.Enums;
using LiquidDocsData.Models;

public class DocumentValidator : AbstractValidator<Document>
{
    public DocumentValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required.")
            .Must(n => n == n.Trim()).WithMessage("Name cannot start or end with whitespace.")
            .Length(2, 200);

        RuleFor(x => x.MasterTemplateDocumentUsedName)
            .NotEmpty().WithMessage("MasterTemplateName is required.");

        // Language/State required only when the flag is set.
        When(x => x.IsStateLanguageRequired, () =>
        {
            RuleFor(x => x.Language)
                .NotEmpty().WithMessage("Language is required when IsStateLanguageRequired is true.");

            RuleFor(x => x.State)
                .Must(s => s != UsStates.UsState.None)
                .WithMessage("State must be set when IsStateLanguageRequired is true.");
        });
    }
}