using QuizApp.Application.Answers.DTOs;
using QuizApp.Application.Common.Interfaces;


namespace QuizApp.Application.Answers.Queries;

public class GetAnswersByQuestionIdQuery : IQuery<IEnumerable<AnswerDto>>
{
    public Guid QuestionId { get; set; }

    public GetAnswersByQuestionIdQuery(Guid questionId)
    {
        QuestionId = questionId;
    }
}