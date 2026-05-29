using EZ.Redact.Lgpd.Core;
using EZ.Redact.Lgpd.OpenTelemetry.Options;
using EZ.Redact.Lgpd.OpenTelemetry.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EZ.Redact.Lgpd.OpenTelemetry;

public static class LgpdRedactionBuilderExtensions
{
    public static ILGPDRedactionBuilder AddOpenTelemetryRedaction(
        this ILGPDRedactionBuilder builder,
        Action<LgpdOpenTelemetryOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        builder.Services.Configure(configure);
        RegisterCore(builder.Services);

        return builder;
    }

    public static ILGPDRedactionBuilder AddOpenTelemetryRedaction(
        this ILGPDRedactionBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        RegisterCore(builder.Services);

        return builder;
    }

    private static void RegisterCore(IServiceCollection services)
    {
        services.TryAddSingleton<LgpdRedactionProcessor>();
    }
}
