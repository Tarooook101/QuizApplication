using Mapster;
using QuizApp.Application.QuizReviews.Commands;
using QuizApp.Application.QuizReviews.DTOs;


namespace QuizApp.Application.Mapping;

public static class QuizReviewMappingProfile
{
    public static void Configure()
    {
        TypeAdapterConfig<QuizApp.Domain.Entities.QuizReview, QuizReviewDto>
            .NewConfig()
            .Map(dst => dst.UserName, src => src.CreatedBy)
            .Map(dst => dst.UserFullName, src => src.CreatedBy);

        TypeAdapterConfig<CreateQuizReviewDto, CreateQuizReviewCommand>
            .NewConfig();

        TypeAdapterConfig<UpdateQuizReviewDto, UpdateQuizReviewCommand>
            .NewConfig();
    }
}