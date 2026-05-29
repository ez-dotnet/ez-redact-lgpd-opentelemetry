using System.Diagnostics;
using EZ.Redact.Lgpd.Core;
using EZ.Redact.Lgpd.OpenTelemetry.Builders;

namespace EZ.Redact.Lgpd.OpenTelemetry;

public static class ActivityExtensions
{
    public static RedactedActivityEventBuilder CreateRedactedEvent(
        this Activity activity,
        string messageTemplate,
        ILGPDRedactService redactService)
    {
        ArgumentNullException.ThrowIfNull(activity);
        ArgumentException.ThrowIfNullOrWhiteSpace(messageTemplate);
        ArgumentNullException.ThrowIfNull(redactService);

        return new RedactedActivityEventBuilder(activity, messageTemplate, redactService);
    }
}
