using Mapster;
using QuizApp.Application.Categories.Commands;
using QuizApp.Application.Categories.DTOs;
using QuizApp.Domain.Entities;


namespace QuizApp.Application.Mapping;

public class CategoryMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryDto>();

        config.NewConfig<CreateCategoryDto, CreateCategoryCommand>();

        config.NewConfig<UpdateCategoryDto, UpdateCategoryCommand>();
    }
}
