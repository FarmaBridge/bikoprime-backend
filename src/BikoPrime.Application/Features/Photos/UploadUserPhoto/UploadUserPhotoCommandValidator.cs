namespace BikoPrime.Application.Features.Photos.UploadUserPhoto;

using FluentValidation;

public class UploadUserPhotoCommandValidator : AbstractValidator<UploadUserPhotoCommand>
{
    public UploadUserPhotoCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.FileContent)
            .NotEmpty()
            .WithMessage("Arquivo é obrigatório");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("Nome do arquivo é obrigatório");

        RuleFor(x => x.ContentType)
            .Must(IsValidImageType)
            .WithMessage("Tipo de arquivo não permitido. Use JPEG, PNG, WebP ou GIF");

        RuleFor(x => x.FileContent)
            .Must(content => content.Length <= 5 * 1024 * 1024)
            .WithMessage("Arquivo não pode exceder 5MB");
    }

    private bool IsValidImageType(string contentType)
    {
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        return allowedTypes.Contains(contentType);
    }
}
