using System.ComponentModel.DataAnnotations;

namespace Catalog.API.DTOs;

public class CategoryUpdateDto : CategoryCreationDto
{
    [Required(ErrorMessage = "Id is required")]
    public string? Id { get; set; }

}