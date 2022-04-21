using System.ComponentModel.DataAnnotations;

namespace Client.Wasm.Models;

public class CategoryUpdateDto : CategoryCreationDto
{
    [Required(ErrorMessage = "Id is required")]
    public string? Id { get; set; }

}