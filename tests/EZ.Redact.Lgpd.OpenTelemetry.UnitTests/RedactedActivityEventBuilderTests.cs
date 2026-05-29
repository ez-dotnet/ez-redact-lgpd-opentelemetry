using System.Diagnostics;
using EZ.Redact.Lgpd.Core;
using EZ.Redact.Lgpd.OpenTelemetry.Builders;
using NSubstitute;
using Xunit;

namespace EZ.Redact.Lgpd.OpenTelemetry.UnitTests;

public class RedactedActivityEventBuilderTests
{
    [Fact]
    public void Add_ReplacesPlaceholdersAndAddsEvent()
    {
        var redactService = Substitute.For<ILGPDRedactService>();
        redactService.Redact(DadoPessoal.CPF, "123.456.789-00").Returns("***.***.***-**");
        redactService.Redact(DadoPessoal.ContaBancaria, "12345-6").Returns("*****-*");

        using var activity = new Activity("test");

        new RedactedActivityEventBuilder(
            activity,
            "Cliente {cpf} realizou saque de {valor} na conta {conta}",
            redactService)
            .WithRedacted("cpf", DadoPessoal.CPF, "123.456.789-00")
            .WithRaw("valor", "R$ 1.500,00")
            .WithRedacted("conta", DadoPessoal.ContaBancaria, "12345-6")
            .Add();

        var evt = Assert.Single(activity.Events);
        Assert.Equal(
            "Cliente ***.***.***-** realizou saque de R$ 1.500,00 na conta *****-*",
            evt.Name);
    }

    [Fact]
    public void WithRaw_StoresValueUnchanged()
    {
        var redactService = Substitute.For<ILGPDRedactService>();

        using var activity = new Activity("test");

        new RedactedActivityEventBuilder(
            activity,
            "Valor: {valor}",
            redactService)
            .WithRaw("valor", "R$ 1.500,00")
            .Add();

        var evt = Assert.Single(activity.Events);
        Assert.Equal("Valor: R$ 1.500,00", evt.Name);
    }
}
