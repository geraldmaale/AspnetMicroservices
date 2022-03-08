using Catalog.API.DTOs;
using FluentValidation;

namespace Catalog.API.Validators;
public class CategoryValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryValidator()
    {

    }
}
