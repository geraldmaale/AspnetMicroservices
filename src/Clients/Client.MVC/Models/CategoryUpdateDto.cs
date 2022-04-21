using System.ComponentModel.DataAnnotations;

namespace Client.Web.Models;

public class CategoryUpdateDto : CategoryCreationDto
{
    [Required(ErrorMessage = "Id is required")]
    public string? Id { get; set; }

}