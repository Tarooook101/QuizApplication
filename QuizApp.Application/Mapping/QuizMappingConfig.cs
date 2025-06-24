using Mapster;
using QuizApp.Application.Quizzes.Commands;
using QuizApp.Application.Quizzes.DTOs;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.Mapping;

public class QuizMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Quiz, QuizDto>()
            .Map(dest => dest.Difficulty, src => src.Difficulty.ToString());

        config.NewConfig<CreateQuizCommand, Quiz>()
            .ConstructUsing(src => new Quiz(
                src.Title,
                src.Description,
                src.TimeLimit,
                src.MaxAttempts,
                src.Difficulty,
                Guid.Empty,
                src.Instructions,
                src.IsPublic,
                src.ThumbnailUrl,
                src.Tags));
    }
}