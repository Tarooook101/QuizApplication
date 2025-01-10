using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using Moq;
using Moq.EntityFrameworkCore;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Database;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;
using FluentAssertions.Execution;

namespace QuizApplication.UnitTesting.RepositoryTesting
{
    [Trait("Category", "Repository")]
    [Collection("Repository Tests")]
    public class QuizRepositoryTests : IDisposable
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly QuizRepository _sut;
        private readonly List<Quiz> _quizzes;
        private readonly DateTimeOffset _now;
        private readonly ITestOutputHelper _output;

        public QuizRepositoryTests(ITestOutputHelper output)
        {
            _output = output;
            _now = DateTimeOffset.UtcNow;
            _quizzes = GenerateTestQuizzes();
            _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockContext.Setup(x => x.Set<Quiz>()).ReturnsDbSet(_quizzes);
            _sut = new QuizRepository(_mockContext.Object);
            
            _output.WriteLine($"Test context initialized at: {_now}");
            _output.WriteLine($"Number of test quizzes: {_quizzes.Count}");
        }

        private List<Quiz> GenerateTestQuizzes()
        {
            var quizzes = new List<Quiz>
            {
                new Quiz
                {
                    Id = 1,
                    Title = "Active Quiz",
                    Description = "Test Quiz 1",
                    Status = QuizStatus.Published,
                    StartDate = _now.AddDays(-1),
                    EndDate = _now.AddDays(1),
                    CreatedById = "user1",
                    CreatedAt = _now.AddDays(-10),
                    Settings = new QuizSettings { QuizId = 1, AllowReview = true },
                    Categories = new List<Category> 
                    { 
                        new Category { Id = 1, Name = "Test Category" } 
                    },
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Id = 1,
                            Text = "Test Question 1",
                            Points = 10,
                            Type = QuestionType.MultipleChoice,
                            Options = new List<Option>
                            {
                                new Option
                                {
                                    Id = 1,
                                    Text = "Option 1",
                                    IsCorrect = true,
                                    QuestionId = 1  // Set the required QuestionId
                                },
                                new Option
                                {
                                    Id = 2,
                                    Text = "Option 2",
                                    IsCorrect = false,
                                    QuestionId = 1  // Set the required QuestionId
                                }
                            }
                        }
                    }
                },
                new Quiz
                {
                    Id = 2,
                    Title = "Expired Quiz",
                    Description = "Test Quiz 2",
                    Status = QuizStatus.Published,
                    StartDate = _now.AddDays(-2),
                    EndDate = _now.AddDays(-1),
                    CreatedById = "user1",
                    CreatedAt = _now.AddDays(-5),
                    Settings = new QuizSettings { QuizId = 2, AllowReview = false }
                },
                new Quiz
                {
                    Id = 3,
                    Title = "Draft Quiz",
                    Description = "Test Quiz 3",
                    Status = QuizStatus.Draft,
                    StartDate = _now.AddDays(-1),
                    EndDate = _now.AddDays(1),
                    CreatedById = "user2",
                    CreatedAt = _now.AddDays(-2),
                    Settings = new QuizSettings { QuizId = 3, AllowReview = true }
                },
                new Quiz
                {
                    Id = 4,
                    Title = "Future Quiz",
                    Description = "Test Quiz 4",
                    Status = QuizStatus.Published,
                    StartDate = _now.AddDays(1),
                    EndDate = _now.AddDays(2),
                    CreatedById = "user1",
                    CreatedAt = _now.AddDays(-1),
                    Settings = new QuizSettings { QuizId = 4, AllowReview = true }
                }
            };

            return quizzes;
        }

        [Fact]
        [Trait("TestType", "Functional")]
        public async Task GetActiveQuizzesAsync_Should_ReturnOnlyActiveQuizzes()
        {
            // Arrange
            _output.WriteLine("Testing GetActiveQuizzesAsync with current time: " + _now);

            // Act
            var result = await _sut.GetActiveQuizzesAsync();
            
            // Assert
            result.Should().HaveCount(1, "only one quiz should be active at the current time");
            
            var activeQuiz = result.First();
            _output.WriteLine($"Active quiz found: {activeQuiz.Title}");
            
            activeQuiz.Should().Match<Quiz>(q =>
                q.Status == QuizStatus.Published &&
                q.StartDate <= _now &&
                q.EndDate > _now);

            using (new AssertionScope())
            {
                activeQuiz.Title.Should().Be("Active Quiz");
                activeQuiz.Settings.Should().NotBeNull();
                activeQuiz.Categories.Should().NotBeEmpty();
            }
        }

        [Theory]
        [InlineData("user1", 3)]
        [InlineData("user2", 1)]
        [InlineData("nonexistent", 0)]
        [Trait("TestType", "Functional")]
        public async Task GetQuizzesByUserAsync_Should_ReturnCorrectNumberOfQuizzes(string userId, int expectedCount)
        {
            // Arrange
            _output.WriteLine($"Testing GetQuizzesByUserAsync for user: {userId}");

            // Act
            var result = await _sut.GetQuizzesByUserAsync(userId);

            // Assert
            result.Should().HaveCount(expectedCount, $"user {userId} should have {expectedCount} quizzes");
            
            if (expectedCount > 0)
            {
                result.Should().OnlyContain(q => q.CreatedById == userId);
                result.Should().BeInDescendingOrder(q => q.CreatedAt);
                
                _output.WriteLine($"Quizzes found for user {userId}:");
                foreach (var quiz in result)
                {
                    _output.WriteLine($"- {quiz.Title} (Created: {quiz.CreatedAt})");
                }
            }
        }

        [Fact]
        [Trait("TestType", "Integration")]
        public async Task IsUserEligibleForQuizAsync_Should_CheckAllEligibilityCriteria()
        {
            // Arrange
            var userId = "user1";
            var quizId = 1;
            var now = DateTimeOffset.UtcNow;

            var quiz = new Quiz
            {
                Id = quizId,
                Title = "Test Quiz",
                Description = "Test Description",
                Status = QuizStatus.Published,
                StartDate = now.AddDays(-1),
                EndDate = now.AddDays(1),
                CreatedById = "creator1",
                CreatedAt = now.AddDays(-10),
                MaxAttempts = 3,
                Attempts = new List<QuizAttempt>
        {
            new QuizAttempt
            {
                UserId = userId,
                QuizId = quizId,
                StartedAt = now.AddHours(-1),
                Status = QuizAttemptStatus.Completed
            }
        },
                AccessControl = new AccessControl
                {
                    IsPublic = true
                }
            };

            var mockSet = new List<Quiz> { quiz }.AsQueryable();
            _mockContext.Setup(x => x.Set<Quiz>())
                .Returns(MockDbSet(mockSet));


            // Act
            var result = await _sut.IsUserEligibleForQuizAsync(userId, quizId);

            // Assert
            result.Should().BeTrue("quiz is active, user has not exceeded maximum attempts (1 of 3), and quiz is public");

            // Verify quiz lookup was performed correctly
            _mockContext.Verify(x => x.Set<Quiz>(), Times.Once);
        }

        // Helper method to create mock DbSet
        private static DbSet<T> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet.Object;
        }

        [Theory]
        [InlineData(1, true)]  // Existing quiz
        [InlineData(999, false)]  // Non-existent quiz
        [Trait("TestType", "Functional")]
        public async Task GetByIdAsync_Should_HandleExistingAndNonExistingQuizzes(int quizId, bool shouldExist)
        {
            // Arrange
            _output.WriteLine($"Testing GetByIdAsync for quiz ID: {quizId}");

            // Act
            var result = await _sut.GetByIdAsync(quizId);

            // Assert
            if (shouldExist)
            {
                result.Should().NotBeNull();
                using (new AssertionScope())
                {
                    result!.Id.Should().Be(quizId);
                    result.Settings.Should().NotBeNull("quiz settings should be loaded");
                    result.Questions.Should().NotBeNull("questions should be loaded");
                    
                    _output.WriteLine($"Quiz found: {result.Title}");
                    _output.WriteLine($"Number of questions: {result.Questions.Count}");
                }
            }
            else
            {
                result.Should().BeNull("quiz should not exist");
                _output.WriteLine("Quiz not found as expected");
            }
        }

        [Fact]
        [Trait("TestType", "Performance")]
        public async Task GetActiveQuizzesAsync_Should_LoadRelatedData()
        {
            // Act
            var result = await _sut.GetActiveQuizzesAsync();
            var quiz = result.FirstOrDefault();

            // Assert
            quiz.Should().NotBeNull();
            using (new AssertionScope())
            {
                quiz!.Settings.Should().NotBeNull("settings should be eagerly loaded");
                quiz.Categories.Should().NotBeNull("categories should be eagerly loaded");
                quiz.Tags.Should().NotBeNull("tags should be eagerly loaded");
            }

            _output.WriteLine("Related data loading test completed:");
            _output.WriteLine($"- Settings loaded: {quiz!.Settings != null}");
            _output.WriteLine($"- Categories loaded: {quiz.Categories != null}");
            _output.WriteLine($"- Tags loaded: {quiz.Tags != null}");
        }

        public void Dispose()
        {
            // Cleanup code if needed
            _output.WriteLine("Test cleanup completed");
        }
    }
}
