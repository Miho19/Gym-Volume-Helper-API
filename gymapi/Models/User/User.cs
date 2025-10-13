using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace gymapi.Models;

[Index(nameof(Id), IsUnique = true, Name = "Index_Id")]
public class User
{


    public required string Id { get; set; }
    [Required]
    public required string PictureSource { get; set; }
    [Required]
    public required int Weight { get; set; }
    [Required]
    public required string CurrentWorkoutId { get; set; }
}