using Mapster;
using QuizApp.Application.QuizAttempts.Commands;
using QuizApp.Application.QuizAttempts.DTOs;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.Mapping;

public class QuizAttemptMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuizAttempt, QuizAttemptDto>();
        config.NewConfig<CreateQuizAttemptDto, CreateQuizAttemptCommand>();
        config.NewConfig<CompleteQuizAttemptDto, CompleteQuizAttemptCommand>();
        config.NewConfig<UpdateQuizAttemptProgressDto, UpdateQuizAttemptProgressCommand>();
    }
}