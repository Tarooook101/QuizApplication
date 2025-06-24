using Mapster;
using QuizApp.Application.Answers.Commands;
using QuizApp.Application.Answers.DTOs;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.Mapping;

public class AnswerMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Answer, AnswerDto>();

        config.NewConfig<CreateAnswerDto, CreateAnswerCommand>();

        config.NewConfig<UpdateAnswerDto, UpdateAnswerCommand>()
            .Ignore(dest => dest.Id);
    }
}