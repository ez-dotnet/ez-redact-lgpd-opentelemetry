using System.Diagnostics;
using EZ.Redact.Lgpd.Core;

namespace EZ.Redact.Lgpd.OpenTelemetry.Builders;

public sealed class RedactedActivityEventBuilder
{
    private readonly Activity _activity;
    private readonly string _messageTemplate;
    private readonly ILGPDRedactService _redactService;
    private readonly Dictionary<string, string> _values = [];

    internal RedactedActivityEventBuilder(Activity activity, string messageTemplate, ILGPDRedactService redactService)
    {
        _activity = activity;
        _messageTemplate = messageTemplate;
        _redactService = redactService;
    }

    public RedactedActivityEventBuilder WithRedacted(string key, DadoPessoal tipo, string value)
    {
        _values[key] = _redactService.Redact(tipo, value);
        return this;
    }

    public RedactedActivityEventBuilder WithRaw(string key, string value)
    {
        _values[key] = value;
        return this;
    }

    public void Add()
    {
        var message = _messageTemplate;
        foreach (var kvp in _values)
        {
            message = message.Replace($"{{{kvp.Key}}}", kvp.Value);
        }
        _activity.AddEvent(new ActivityEvent(message));
    }
}
