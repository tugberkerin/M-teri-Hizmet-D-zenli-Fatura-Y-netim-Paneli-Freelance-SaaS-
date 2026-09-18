using FluentValidation;

namespace ClientPulse.Core.Features.Clients.Commands.CreateClient;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Müşteri adı boş olamaz.")
            .MaximumLength(100).WithMessage("Müşteri adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
            
        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.");
            
        RuleFor(x => x.CompanyName)
            .MaximumLength(100).WithMessage("Şirket adı en fazla 100 karakter olabilir.");
    }
}
