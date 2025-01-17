namespace QuizApplication.API.Models.Tag
{
    public class TagResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
