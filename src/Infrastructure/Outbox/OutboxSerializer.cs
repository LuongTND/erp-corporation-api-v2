using System.Text.Json;
using Domain.Events;

namespace Infrastructure.Outbox;

internal static class OutboxSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static (string Type, string Payload) Serialize(IDomainEvent domainEvent)
    {
        var type = domainEvent.GetType().AssemblyQualifiedName
            ?? throw new InvalidOperationException($"Cannot resolve type for {domainEvent.GetType().Name}.");

        var payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), Options);
        return (type, payload);
    }

    public static object Deserialize(string typeName, string payload)
    {
        var type = ResolveType(typeName)
            ?? throw new InvalidOperationException($"Unknown outbox event type: {typeName}.");

        var notification = JsonSerializer.Deserialize(payload, type, Options)
            ?? throw new InvalidOperationException($"Failed to deserialize outbox payload for {typeName}.");

        return notification;
    }

    private static Type? ResolveType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type is not null)
            return type;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(typeName, throwOnError: false, ignoreCase: false);
            if (type is not null)
                return type;

            type = assembly.GetTypes().FirstOrDefault(t => t.FullName == typeName);
            if (type is not null)
                return type;
        }

        return null;
    }
}
