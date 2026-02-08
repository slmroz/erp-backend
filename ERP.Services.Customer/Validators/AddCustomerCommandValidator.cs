using FluentValidation;

namespace ERP.Services.Customer.Validators;

using ERP.Services.Customer.Commands;

public sealed class AddCustomerCommandValidator
    : AbstractValidator<AddCustomerCommand>
{
    public AddCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.TaxId)
            .MaximumLength(100);

        RuleFor(x => x.ZipCode)
            .MaximumLength(10);

        RuleFor(x => x.City)
            .MaximumLength(50);

        RuleFor(x => x.Country)
            .MaximumLength(50);

        RuleFor(x => x.Www)
            .MaximumLength(100)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.Www));
    }

    private bool BeValidUrl(string url)
        => Uri.TryCreate(url, UriKind.Absolute, out _);
}