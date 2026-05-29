using System.Diagnostics;
using EZ.Redact.Lgpd.Core;
using EZ.Redact.Lgpd.OpenTelemetry.Builders;
using NSubstitute;
using Xunit;

namespace EZ.Redact.Lgpd.OpenTelemetry.UnitTests;

public class ActivityExtensionsTests
{
    [Fact]
    public void CreateRedactedEvent_ReturnsBuilder()
    {
        using var activity = new Activity("test");
        var redactService = Substitute.For<ILGPDRedactService>();

        var builder = activity.CreateRedactedEvent("template {key}", redactService);

        Assert.IsType<RedactedActivityEventBuilder>(builder);
    }

    [Fact]
    public void CreateRedactedEvent_ThrowsOnNullActivity()
    {
        Activity? activity = null;
        var redactService = Substitute.For<ILGPDRedactService>();

        Assert.Throws<ArgumentNullException>(() =>
            activity!.CreateRedactedEvent("template", redactService));
    }

    [Fact]
    public void CreateRedactedEvent_ThrowsOnNullService()
    {
        using var activity = new Activity("test");

        Assert.Throws<ArgumentNullException>(() =>
            activity.CreateRedactedEvent("template", null!));
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    [InlineData(" ", typeof(ArgumentException))]
    public void CreateRedactedEvent_ThrowsOnInvalidTemplate(string? template, Type expectedException)
    {
        using var activity = new Activity("test");
        var redactService = Substitute.For<ILGPDRedactService>();

        Assert.Throws(expectedException, () =>
            activity.CreateRedactedEvent(template!, redactService));
    }
}
