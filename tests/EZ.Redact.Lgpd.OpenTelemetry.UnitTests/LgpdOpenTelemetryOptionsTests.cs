using EZ.Redact.Lgpd.OpenTelemetry.Options;
using Xunit;

namespace EZ.Redact.Lgpd.OpenTelemetry.UnitTests;

public class LgpdOpenTelemetryOptionsTests
{
    [Fact]
    public void RedactTags_AddsTagKeys()
    {
        var options = new LgpdOpenTelemetryOptions();
        options.RedactTags("user.cpf", "user.email");

        Assert.Contains("user.cpf", options.TagKeys);
        Assert.Contains("user.email", options.TagKeys);
        Assert.Equal(2, options.TagKeys.Count);
    }

    [Fact]
    public void RedactBaggage_AddsBaggageKeys()
    {
        var options = new LgpdOpenTelemetryOptions();
        options.RedactBaggage("user.cpf", "user.email");

        Assert.Contains("user.cpf", options.BaggageKeys);
        Assert.Contains("user.email", options.BaggageKeys);
        Assert.Equal(2, options.BaggageKeys.Count);
    }

    [Fact]
    public void RedactTags_ReturnsSelfForChaining()
    {
        var options = new LgpdOpenTelemetryOptions();
        var result = options.RedactTags("key1");
        Assert.Same(options, result);
    }

    [Fact]
    public void RedactBaggage_ReturnsSelfForChaining()
    {
        var options = new LgpdOpenTelemetryOptions();
        var result = options.RedactBaggage("key1");
        Assert.Same(options, result);
    }
}
