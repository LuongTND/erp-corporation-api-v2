namespace Application;

public static class NotificationRecipientRulesJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static string Serialize(NotificationRecipientRulesDto rules) =>
        JsonSerializer.Serialize(rules, Options);

    public static NotificationRecipientRulesDto Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new NotificationRecipientRulesDto();

        return JsonSerializer.Deserialize<NotificationRecipientRulesDto>(json, Options)
               ?? new NotificationRecipientRulesDto();
    }
}

public static class NotificationRecipientRulesDefaults
{
    public static NotificationRecipientRulesDto ForUserEvents() => new()
    {
        IncludeSubjectUser = true,
        IncludeActor = true
    };

    public static NotificationRecipientRulesDto ForAdminEvents() => new()
    {
        IncludeActor = true,
        IncludeSuperAdmins = true
    };
}