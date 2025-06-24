using Mapster;
using QuizApp.Application.QuizResults.DTOs;
using QuizApp.Domain.Entities;


namespace QuizApp.Application.Mapping;

public class QuizResultMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuizResult, QuizResultDto>();
        config.NewConfig<QuizResult, QuizResultSummaryDto>();
        config.NewConfig<QuizResult, QuizResultDetailDto>()
            .Map(dest => dest.UserName, src => src.User.FullName)
            .Map(dest => dest.QuizTitle, src => "Quiz Title");
    }
}