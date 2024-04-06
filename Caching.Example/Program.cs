using Caching.Example.PipelineBehaviors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddSingleton<RedisService>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

var app = builder.Build();

app.MapGet("/", async (IMediator mediator) => await mediator.Send(new GetProductsQueryRequest()));

app.Run();

public record GetProductsQueryRequest() : IRequest<List<GetProductsQueryResponse>>;

public record GetProductsQueryResponse(string ProductName, int Quantity);

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQueryRequest, List<GetProductsQueryResponse>>
{
    public Task<List<GetProductsQueryResponse>> Handle(GetProductsQueryRequest request, CancellationToken cancellationToken)
        => Task.FromResult<List<GetProductsQueryResponse>>(new() {
            new("Product1", 10),
            new("Product2", 20),
            new("Product3", 30),
            new("Product4", 40),
            new("Product5", 50),
        });
}

public class RedisService
{
    ConnectionMultiplexer? connectionMultiplexer;
    public void Connect()
        => connectionMultiplexer = ConnectionMultiplexer.Connect("localhost:6379");
    public IDatabase Database
    {
        get
        {
            if (!connectionMultiplexer?.IsConnected == null)
                Connect();

            return connectionMultiplexer.GetDatabase(0);
        }
    }
}