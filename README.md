# EZ.Redact.Lgpd.OpenTelemetry

[![NuGet Version](https://img.shields.io/badge/nuget-v1.0.0-blue.svg)](https://www.nuget.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET 8.0+](https://img.shields.io/badge/.NET-8.0%2B%20|%209.0%2B%20|%2010.0%2B-512bd4.svg)](https://dotnet.microsoft.com/download)

**EZ.Redact.Lgpd.OpenTelemetry** é uma extensão do ecossistema EZ.Redact.Lgpd que aplica redação de dados sensíveis (LGPD) em traces do OpenTelemetry. Ela redige tags e baggage de spans automaticamente antes da exportação, além de fornecer uma API fluente para criar eventos com dados mascarados.

---

## Instalação

```bash
dotnet add package EZ.Redact.Lgpd.OpenTelemetry
```

Registre os serviços no DI com `AddLGPDRedaction()` e configure a redação OpenTelemetry:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLGPDRedaction();
builder.Logging.EnableRedaction(options => options.ApplyDiscriminator = false);
```

---

## Exemplo rápido

```csharp
using EZ.Redact.Lgpd.OpenTelemetry;
using EZ.Redact.Lgpd.OpenTelemetry.Processors;
using OpenTelemetry.Trace;

builder.Services.AddLGPDRedaction()
    .AddOpenTelemetryRedaction(options =>
    {
        options.RedactTags("user.cpf", "user.email", "customer.document");
        options.RedactBaggage("user.cpf", "user.email");
    });

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddProcessor(sp => sp.GetRequiredService<LgpdRedactionProcessor>()));
```

Tags e baggage cujos nomes estiverem na blacklist são redigidos automaticamente ao final do span:

| Entrada | Resultado |
| :--- | :--- |
| `123.456.789-00` | `123*******00` |
| `joao.silva@example.com` | `joa***********com` |

---

## Eventos redigidos

Crie eventos com dados sensíveis usando a API fluente:

```csharp
activity.CreateRedactedEvent(
    "Cliente {cpf} realizou saque de {valor} na conta {conta}",
    redactService)
    .WithRedacted("cpf", DadoPessoal.CPF, "123.456.789-00")
    .WithRaw("valor", "R$ 1.500,00")
    .WithRedacted("conta", DadoPessoal.ContaBancaria, "12345-6")
    .Add();
```

Saída do evento redigido:

```
Cliente ***.***.***-** realizou saque de R$ 1.500,00 na conta *****-*
```

### API do builder

| Método | Descrição |
| :--- | :--- |
| `WithRedacted(key, tipo, value)` | Redige o valor usando `ILGPDRedactService` antes de armazenar |
| `WithRaw(key, value)` | Armazena o valor sem alteração |
| `Add()` | Substitui os placeholders `{key}` no template e chama `activity.AddEvent()` |

---

## Configuração

### `LgpdOpenTelemetryOptions`

| Método | Descrição |
| :--- | :--- |
| `RedactTags(params string[] tags)` | Adiciona nomes de tags que devem ser redigidas ao final do span |
| `RedactBaggage(params string[] keys)` | Adiciona nomes de baggage que devem ser redigidas ao final do span |

```csharp
builder.Services.AddLGPDRedaction()
    .AddOpenTelemetryRedaction(options =>
    {
        options.RedactTags("user.cpf", "user.email", "user.nome", "customer.document");
        options.RedactBaggage("user.cpf", "user.email");
    });
```

---

## Comportamento

### `LgpdRedactionProcessor`

O `LgpdRedactionProcessor` implementa `BaseProcessor<Activity>` do OpenTelemetry e:

1. **No método `OnEnd`** intercepta cada span antes da exportação
2. Verifica se a redação está habilitada — respeita o controle configurado no Core (via `ILGPDRedactService`)
3. Para cada **tag** cujo nome estiver na blacklist, substitui o valor pela redação genérica
4. Para cada **baggage** cujo nome estiver na blacklist, substitui o valor pela redação genérica

### Redação genérica

Preserva 3 caracteres no início e 3 no final do valor, substituindo o meio por asteriscos:

```
"123.456.789-00" → "123*******00"
"joao@example.com" → "joa*******com"
"curto" → "curto" (inalterado — < 6 caracteres)
```

---

## Projetos Relacionados

| Projeto | Descrição |
| :--- | :--- |
| [EZ.Redact.Lgpd.Core](https://github.com/ez-dotnet/ez-redact-lgpd-core) | Biblioteca central de redação de dados sensíveis LGPD |
| [EZ.Redact.Lgpd.EntityFramework](https://github.com/ez-dotnet/ez-redact-lgpd-entityframework) | Redação em consultas Entity Framework |
| [EZ.Redact.Lgpd.Json](https://github.com/ez-dotnet/ez-redact-lgpd-json) | Redação em serialização JSON |
| [EZ.Redact.Lgpd.MongoDb](https://github.com/ez-dotnet/ez-redact-lgpd-mongodb) | Redação em consultas MongoDB |
| [EZ.Redact.Lgpd.Xml](https://github.com/ez-dotnet/ez-redact-lgpd-xml) | Redação em serialização XML |

---

## Licença

Distribuído sob a licença MIT.
