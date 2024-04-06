using Logging.Example.PipelineBehaviors;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs.txt")
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<Program>());

var app = builder.Build();

app.MapGet("/", async (IMediator mediator) => await mediator.Send(new MyRequest("MyRequest Sending...")));

app.Run();

public record MyRequest(string Text) : IRequest<MyResponse>;

public record MyResponse(string Message);

public sealed class MyHandler : IRequestHandler<MyRequest, MyResponse>
{
    public Task<MyResponse> Handle(MyRequest request, CancellationToken cancellationToken)
        => Task.FromResult<MyResponse>(new("..."));
}