namespace QuizApplication.API.Models.Option
{
    public class OptionResponse
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
        public string? Explanation { get; set; }

        public static OptionResponse FromEntity(DAL.Entities.Option entity)
        {
            return new OptionResponse
            {
                Id = entity.Id,
                Text = entity.Text,
                IsCorrect = entity.IsCorrect,
                DisplayOrder = entity.DisplayOrder,
                Explanation = entity.Explanation
            };
        }
    }

}
