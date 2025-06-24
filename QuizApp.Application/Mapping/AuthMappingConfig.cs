using Mapster;
using QuizApp.Application.Auth.Commands;
using QuizApp.Application.Auth.DTOs;
using QuizApp.Domain.Entities;


namespace QuizApp.Application.Mapping;

public class AuthMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ApplicationUser, UserDto>()
            .Map(dest => dest.FullName, src => src.FullName);

        config.NewConfig<RegisterDto, RegisterCommand>();
        config.NewConfig<LoginDto, LoginCommand>();
    }
}