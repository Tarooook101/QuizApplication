using FluentValidation;
using QuizApp.Application.Quizzes.Commands;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Quizzes.Validators;

public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
{
    private readonly IQuizRepository _quizRepository;

    public UpdateQuizCommandValidator(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Quiz ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .MustAsync(BeUniqueTitle).WithMessage("A quiz with this title already exists");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Instructions)
            .MaximumLength(1000).WithMessage("Instructions cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Instructions));

        RuleFor(x => x.TimeLimit)
            .GreaterThanOrEqualTo(1).WithMessage("Time limit must be at least 1 minute")
            .LessThanOrEqualTo(480).WithMessage("Time limit cannot exceed 480 minutes");

        RuleFor(x => x.MaxAttempts)
            .GreaterThanOrEqualTo(1).WithMessage("Max attempts must be at least 1")
            .LessThanOrEqualTo(10).WithMessage("Max attempts cannot exceed 10");

        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("Invalid difficulty level");

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(500).WithMessage("Thumbnail URL cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl));

        RuleFor(x => x.Tags)
            .MaximumLength(500).WithMessage("Tags cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Tags));
    }

    private async Task<bool> BeUniqueTitle(UpdateQuizCommand command, string title, CancellationToken cancellationToken)
    {
        return !await _quizRepository.ExistsByTitleAsync(title, command.Id, cancellationToken);
    }
}