using Mapster;
using QuizApp.Application.UserAnswers.Commands;
using QuizApp.Application.UserAnswers.DTOs;
using QuizApp.Domain.Entities;


namespace QuizApp.Application.Mapping;

public class UserAnswerMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserAnswer, UserAnswerDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.QuizAttemptId, src => src.QuizAttemptId)
            .Map(dest => dest.QuestionId, src => src.QuestionId)
            .Map(dest => dest.SelectedAnswerId, src => src.SelectedAnswerId)
            .Map(dest => dest.TextAnswer, src => src.TextAnswer)
            .Map(dest => dest.IsCorrect, src => src.IsCorrect)
            .Map(dest => dest.PointsEarned, src => src.PointsEarned)
            .Map(dest => dest.AnsweredAt, src => src.AnsweredAt)
            .Map(dest => dest.TimeSpent, src => src.TimeSpent)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
            .Map(dest => dest.CreatedBy, src => src.CreatedBy)
            .Map(dest => dest.UpdatedBy, src => src.UpdatedBy);

        config.NewConfig<UserAnswer, UserAnswerDetailsDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.QuizAttemptId, src => src.QuizAttemptId)
            .Map(dest => dest.QuestionId, src => src.QuestionId)
            .Map(dest => dest.SelectedAnswerId, src => src.SelectedAnswerId)
            .Map(dest => dest.TextAnswer, src => src.TextAnswer)
            .Map(dest => dest.IsCorrect, src => src.IsCorrect)
            .Map(dest => dest.PointsEarned, src => src.PointsEarned)
            .Map(dest => dest.AnsweredAt, src => src.AnsweredAt)
            .Map(dest => dest.TimeSpent, src => src.TimeSpent)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
            .Map(dest => dest.CreatedBy, src => src.CreatedBy)
            .Map(dest => dest.UpdatedBy, src => src.UpdatedBy)
            .Map(dest => dest.QuestionText, src => src.Question.Text)
            .Map(dest => dest.SelectedAnswerText, src => src.SelectedAnswer != null ? src.SelectedAnswer.Text : null)
            .Map(dest => dest.IsQuestionRequired, src => src.Question.IsRequired)
            .Map(dest => dest.QuestionPoints, src => src.Question.Points);

        config.NewConfig<CreateUserAnswerDto, CreateUserAnswerCommand>();
        config.NewConfig<UpdateUserAnswerDto, UpdateUserAnswerCommand>();
    }
}