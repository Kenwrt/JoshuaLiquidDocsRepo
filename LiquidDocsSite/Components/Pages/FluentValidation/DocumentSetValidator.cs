using FluentValidation;
using LiquidDocsData.Models;
using LiquidDocsSite.Components.Pages.FluentValidation;

public class DocumentSetValidator : AbstractValidator<DocumentSet>
{
    public DocumentSetValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required.")
            .Must(n => n == n.Trim()).WithMessage("Name cannot start or end with whitespace.")
            .Length(2, 160);

        RuleFor(x => x.Description)
            .Must(d => d == null || d == d.Trim())
            .WithMessage("Description cannot start or end with whitespace.")
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Documents)
            .NotNull().WithMessage("Documents collection must be initialized.");

        //RuleForEach(x => x.Documents)
        //    .SetValidator(documentValidator ?? new DocumentValidator());

        // All documents must have the same UserId as the parent set
        RuleFor(x => x)
            .Must(set => set.Documents is null || set.Documents.All(d => d.UserId == set.UserId))
            .WithMessage("All documents must have the same UserId as the DocumentSet.");

        // Unique Document.Id within the set
        RuleFor(x => x.Documents)
            .Must(docs => docs == null || docs.Select(d => d.Id).Distinct().Count() == docs.Count)
            .WithMessage("Each Document.Id must be unique within the set.");

        //// Unique Document names inside the set (normalized)
        //RuleFor(x => x.Documents)
        //    .Must(ValidationHelper.NamesAreUniqueByNormalizedKey)
        //    .WithMessage("Document names must be unique within the set (ignoring case, spaces, punctuation).");

        //// Optional: per-user unique DocumentSet name
        //if (repo is not null)
        //{
        //    RuleFor(x => x.Name)
        //        .MustAsync(async (model, name, ct) =>
        //            !await repo.ExistsByNameAsync(model.UserId, name, ct))
        //        .WithMessage("A DocumentSet with this name already exists for this user.");
        //}
    }
}