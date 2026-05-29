namespace EZ.Redact.Lgpd.OpenTelemetry.Options;

public class LgpdOpenTelemetryOptions
{
    internal HashSet<string> TagKeys { get; } = [];
    internal HashSet<string> BaggageKeys { get; } = [];

    public LgpdOpenTelemetryOptions RedactTags(params string[] tags)
    {
        foreach (var tag in tags)
            TagKeys.Add(tag);
        return this;
    }

    public LgpdOpenTelemetryOptions RedactBaggage(params string[] keys)
    {
        foreach (var key in keys)
            BaggageKeys.Add(key);
        return this;
    }
}
