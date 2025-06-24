using Mapster;
using QuizApp.Application.Questions.Commands;
using QuizApp.Application.Questions.DTOs;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.Mapping;

public class QuestionMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Question, QuestionDto>();
        config.NewConfig<CreateQuestionCommand, Question>()
            .ConstructUsing(src => new Question(
                src.Text,
                src.Type,
                src.Points,
                src.OrderIndex,
                src.QuizId,
                src.IsRequired,
                src.Explanation,
                src.ImageUrl));
    }
}