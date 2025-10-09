namespace gymapi.Models;

public class User
{
    public required string Id { get; set; }
    public required string PictureSource { get; set; }
    public required int Weight { get; set; }
    public required string CurrentWorkoutId { get; set; }
}