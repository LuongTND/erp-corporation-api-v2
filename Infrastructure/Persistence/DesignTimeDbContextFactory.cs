using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        LoadEnvFile();

        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "CONNECTION_STRING is not set. Add it to API/.env before running EF migrations.");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options, new DesignTimeMediator());
    }

    private static void LoadEnvFile()
    {
        var candidates = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "API", ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "API", ".env")
        };

        var envFile = candidates.Select(Path.GetFullPath).FirstOrDefault(File.Exists);
        if (envFile is not null)
            DotNetEnv.Env.Load(envFile);
    }

    private sealed class DesignTimeMediator : IMediator
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task Publish<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification =>
            Task.CompletedTask;

        public Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("Design-time only.");

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest =>
            throw new NotSupportedException("Design-time only.");

        public Task<object?> Send(object request, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("Design-time only.");

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("Design-time only.");

        public IAsyncEnumerable<object?> CreateStream(
            object request,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("Design-time only.");
    }
}
