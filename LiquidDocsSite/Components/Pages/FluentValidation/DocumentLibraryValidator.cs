using FluentValidation;
using LiquidDocsData.Models;

public class DocumentLibraryValidator : AbstractValidator<DocumentLibrary>
{
    public DocumentLibraryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required.")
            .Must(n => n == n.Trim()).WithMessage("Name cannot start or end with whitespace.")
            .Length(2, 120);

        // If not using the default template, require MasterTemplate (string); ignore bytes.
        When(x => !x.IsUsingDefaultTemplate, () =>
        {
            RuleFor(x => x.MasterTemplate)
                .NotEmpty()
                .WithMessage("MasterTemplate is required when not using the default template.");
        });

        // IsActive: no rule needed.
    }
}