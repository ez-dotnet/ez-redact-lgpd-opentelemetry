using System.Diagnostics;
using EZ.Redact.Lgpd.Core;
using EZ.Redact.Lgpd.OpenTelemetry.Options;
using EZ.Redact.Lgpd.OpenTelemetry.Processors;
using NSubstitute;
using Xunit;

namespace EZ.Redact.Lgpd.OpenTelemetry.UnitTests;

public class LgpdRedactionProcessorTests
{
    [Fact]
    public void OnEnd_RedactsMatchingTag()
    {
        var service = Substitute.For<ILGPDRedactService>();
        var options = new LgpdOpenTelemetryOptions();
        options.RedactTags("user.cpf");

        using var activity = new Activity("test");
        activity.SetTag("user.cpf", "123.456.789-00");
        activity.SetTag("user.nome", "João");

        var processor = new LgpdRedactionProcessor(service, Microsoft.Extensions.Options.Options.Create(options));
        processor.OnEnd(activity);

        var cpfTag = activity.Tags.First(t => t.Key == "user.cpf").Value;
        Assert.Equal("123********-00", cpfTag);
    }

    [Fact]
    public void OnEnd_DoesNotRedactNonMatchingTag()
    {
        var service = Substitute.For<ILGPDRedactService>();
        var options = new LgpdOpenTelemetryOptions();
        options.RedactTags("user.cpf");

        using var activity = new Activity("test");
        activity.SetTag("user.nome", "João");

        var processor = new LgpdRedactionProcessor(service, Microsoft.Extensions.Options.Options.Create(options));
        processor.OnEnd(activity);

        var nomeTag = activity.Tags.First(t => t.Key == "user.nome").Value;
        Assert.Equal("João", nomeTag);
    }

    [Fact]
    public void OnEnd_RedactsMatchingBaggage()
    {
        var service = Substitute.For<ILGPDRedactService>();
        var options = new LgpdOpenTelemetryOptions();
        options.RedactBaggage("user.email");

        using var activity = new Activity("test");
        activity.SetBaggage("user.email", "joao@example.com");
        activity.SetBaggage("user.role", "admin");

        var processor = new LgpdRedactionProcessor(service, Microsoft.Extensions.Options.Options.Create(options));
        processor.OnEnd(activity);

        var baggage = activity.Baggage.ToDictionary(k => k.Key, k => k.Value);
        Assert.Equal("joa**********com", baggage["user.email"]);
        Assert.Equal("admin", baggage["user.role"]);
    }

    [Theory]
    [InlineData("12345678901", "123*****901")]
    [InlineData("abcdef", "abcdef")]
    [InlineData("a", "a")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void RedactGeneric_ReturnsExpected(string? input, string? expected)
    {
        var result = LgpdRedactionProcessor.RedactGeneric(input!);
        Assert.Equal(expected, result);
    }
}
