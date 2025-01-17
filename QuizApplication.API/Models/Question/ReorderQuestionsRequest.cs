using System.ComponentModel.DataAnnotations;

namespace QuizApplication.API.Models.Question
{
    public class ReorderQuestionsRequest
    {
        [Required]
        public Dictionary<int, int> QuestionOrders { get; set; } = new();
    }
}
