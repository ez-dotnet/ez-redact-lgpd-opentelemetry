using EZ.Redact.Lgpd.Core;
using EZ.Redact.Lgpd.OpenTelemetry;
using EZ.Redact.Lgpd.OpenTelemetry.Processors;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.EnableRedaction(o => o.ApplyDiscriminator = false);

builder.Services.AddLGPDRedaction()
    .AddOpenTelemetryRedaction(options =>
    {
        options.RedactTags("user.cpf", "user.email", "customer.document", "user.nome");
        options.RedactBaggage("user.cpf", "user.email");
    });

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddProcessor(sp => sp.GetRequiredService<LgpdRedactionProcessor>())
        .AddConsoleExporter());

var app = builder.Build();

app.MapGet("/", async (ILGPDRedactService redact, HttpContext context) =>
{
    var activity = System.Diagnostics.Activity.Current;
    activity?.SetTag("user.cpf", "123.456.789-00");
    activity?.SetTag("user.email", "joao.silva@example.com");
    activity?.SetTag("user.nome", "João Silva");
    activity?.SetTag("http.remote", context.Connection.RemoteIpAddress?.ToString());

    activity?.CreateRedactedEvent(
        "Cliente {cpf} realizou saque de {valor} na conta {conta}",
        redact)
        .WithRedacted("cpf", DadoPessoal.CPF, "123.456.789-00")
        .WithRaw("valor", "R$ 1.500,00")
        .WithRedacted("conta", DadoPessoal.ContaBancaria, "12345-6")
        .Add();

    return new
    {
        message = "Span created with redacted data. Check your OpenTelemetry console exporter output."
    };
});

app.Run();
