using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Questions.DTOs;


namespace QuizApp.Application.Questions.Queries;

public class GetQuestionsByQuizIdQuery : IQuery<IEnumerable<QuestionDto>>
{
    public Guid QuizId { get; set; }
    public bool OrderByIndex { get; set; } = true;

    public GetQuestionsByQuizIdQuery(Guid quizId, bool orderByIndex = true)
    {
        QuizId = quizId;
        OrderByIndex = orderByIndex;
    }
}
