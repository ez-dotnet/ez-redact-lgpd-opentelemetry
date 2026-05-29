using System.Diagnostics;
using EZ.Redact.Lgpd.Core;
using EZ.Redact.Lgpd.OpenTelemetry.Options;
using Microsoft.Extensions.Options;
using OpenTelemetry;

namespace EZ.Redact.Lgpd.OpenTelemetry.Processors;

public sealed class LgpdRedactionProcessor : BaseProcessor<Activity>
{
    private readonly ILGPDRedactService _redactService;
    private readonly LgpdOpenTelemetryOptions _options;

    public LgpdRedactionProcessor(ILGPDRedactService redactService, IOptions<LgpdOpenTelemetryOptions> options)
    {
        _redactService = redactService;
        _options = options.Value;
    }

    public override void OnEnd(Activity activity)
    {
        foreach (var tag in activity.Tags)
        {
            if (_options.TagKeys.Contains(tag.Key) && tag.Value is string stringValue)
            {
                activity.SetTag(tag.Key, RedactGeneric(stringValue));
            }
        }

        var baggageEntries = activity.Baggage.ToList();
        foreach (var entry in baggageEntries)
        {
            if (_options.BaggageKeys.Contains(entry.Key) && entry.Value is not null)
            {
                activity.SetBaggage(entry.Key, RedactGeneric(entry.Value));
            }
        }
    }

    internal static string RedactGeneric(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= 6)
            return value;

        return string.Concat(value.AsSpan(0, 3), new string('*', value.Length - 6), value.AsSpan(value.Length - 3));
    }

}
