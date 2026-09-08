using EventLensAI.Application.DTOs.Storage;using FluentValidation;
namespace EventLensAI.Application.Validators;
public sealed class CreateStorageFolderRequestValidator:AbstractValidator<CreateStorageFolderRequest>{public CreateStorageFolderRequestValidator()=>RuleFor(x=>x.Name).NotEmpty().MaximumLength(200).Must(x=>x.IndexOfAny(Path.GetInvalidFileNameChars())<0);}
public sealed class RenameStorageFolderRequestValidator:AbstractValidator<RenameStorageFolderRequest>{public RenameStorageFolderRequestValidator()=>RuleFor(x=>x.Name).NotEmpty().MaximumLength(200).Must(x=>x.IndexOfAny(Path.GetInvalidFileNameChars())<0);}
public sealed class UpdateStorageFileRequestValidator:AbstractValidator<UpdateStorageFileRequest>{public UpdateStorageFileRequestValidator(){RuleFor(x=>x.FileName).NotEmpty().MaximumLength(255);RuleFor(x=>x.Notes).MaximumLength(2000);RuleFor(x=>x.Tags).Must(x=>x.Count<=30);}}
public sealed class StorageSearchRequestValidator:AbstractValidator<StorageSearchRequest>{public StorageSearchRequestValidator(){RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.Page).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,200);}}
