using FluentValidation;
using MediatR;
using Validation.Example.PipelineBehaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

var app = builder.Build();

app.MapPost("/create-user", async (IMediator mediator, CreateUserCommandRequest createUserCommandRequest) => await mediator.Send(createUserCommandRequest));

app.Run();

public record CreateUserCommandRequest(string Username, string Email, string Password) : IRequest<CreateUserCommandResponse>;

public record CreateUserCommandResponse(string Message);

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
{
    public Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        => Task.FromResult<CreateUserCommandResponse>(new("..."));
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommandRequest>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
                .WithMessage("Username bo� ge�ilemez!");
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email bo� ge�ilemez!")
            .EmailAddress()
                .WithMessage("Yanl�� e-mail format�!");
        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Password bo� ge�ilemez!")
            .MinimumLength(6)
               .WithMessage("Password minimum 6 karakter olmal�d�r!");
    }
}