using System.ComponentModel.DataAnnotations;

namespace Catalog.API.DTOs;

public class CategoryCreationDto
{
    [Required(ErrorMessage = "Category name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; }
}