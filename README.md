# Animed API (.NET)

API REST em **ASP.NET Core / .NET 10** para o produto **Animed** (cliente **Clyvo VET**), um
aplicativo de saude e bem-estar de pets. Esta API .NET compartilha o **mesmo banco Oracle** da API
Java (Spring Boot) do squad: o schema pertence e e versionado pela API Java (Flyway); aqui o
**EF Core apenas mapeia** as tabelas existentes (`ToTable`/`HasColumnName`), **sem migrations**.

Escopo mapeado: **Tutor**, **Pet**, **Consulta** e **Vacina** (CRUD completo), com autenticacao JWT,
observabilidade (health checks, logs estruturados, tracing e metricas) e testes unitarios e de integracao.

## Stack

- .NET 10 / ASP.NET Core (Controllers)
- Entity Framework Core 10 + provider Oracle (`Oracle.EntityFrameworkCore`)
- Autenticacao JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- Serilog (logs estruturados) + OpenTelemetry (tracing e metricas)
- Swagger / OpenAPI (Swashbuckle) em ambiente de desenvolvimento
- Testes: xUnit + NSubstitute (unitarios) e WebApplicationFactory + SQLite in-memory (integracao)

## Arquitetura (camadas)

```
Controllers  -> recebem HTTP, retornam status (finos; sem regra de negocio)
Services     -> regra de aplicacao (orquestra repositorios, valida FKs), traduz Entidade <-> DTO
Domain       -> entidades com factories (Criar/Atualizar) que garantem invariantes (DomainException)
Repositories -> acesso a dados via EF Core (interfaces, para serem testaveis/mockaveis)
DbContext    -> mapeia as tabelas TB_* do Oracle
```

Erros sao centralizados em um **exception handler global** (`GlobalExceptionHandler`): regra de
dominio/argumento invalido vira **400**; erro inesperado vira **500** (formato `ProblemDetails`).
Validacao de entrada (`[Required]`, `[EmailAddress]`, etc.) e automatica via `[ApiController]`.

## Pre-requisitos

- .NET SDK 10
- Acesso ao Oracle da FIAP (para rodar a API de fato; os testes **nao** precisam de banco)

## Configuracao

A **connection string** e a **chave JWT** de producao ficam em **user-secrets** (nunca no repositorio).

```bash
# na pasta AnimedApi
dotnet user-secrets set "ConnectionStrings:AnimedDatabase" "User Id=RMxxxxxx;Password=xxxxxx;Data Source=oracle.fiap.com.br:1521/ORCL"
dotnet user-secrets set "Jwt:Key" "uma-chave-secreta-longa-para-assinar-os-tokens"
```

Em **desenvolvimento**, `appsettings.Development.json` ja traz uma `Jwt:Key` de teste e as credenciais
de login (`Auth:Usuario` / `Auth:Senha` = `admin` / `admin`) para subir rapidamente. **Troque em producao.**

## Rodando a API

```bash
cd AnimedApi
dotnet run
```

Swagger (em Development): `https://localhost:<porta>/swagger`.

## Autenticacao

Todos os endpoints de negocio exigem **JWT**. Fluxo:

1. `POST /api/auth/login` com corpo `{ "usuario": "admin", "senha": "admin" }` -> devolve `{ token, expiraEm }`.
2. Envie o token no header: `Authorization: Bearer <token>` nas demais chamadas.

Sem token, os endpoints protegidos respondem **401**.

## Endpoints

| Metodo | Rota                     | Descricao                        | Auth |
|--------|--------------------------|----------------------------------|------|
| POST   | `/api/auth/login`        | Login, devolve token JWT         | Nao  |
| GET    | `/health`                | Health check (status do Oracle)  | Nao  |
| GET/POST/PUT/DELETE | `/api/tutores`  | CRUD de tutores                  | Sim  |
| GET/POST/PUT/DELETE | `/api/pets`     | CRUD de pets (`?tutorId=`)       | Sim  |
| GET/POST/PUT/DELETE | `/api/consultas`| CRUD de consultas (`?petId=` etc)| Sim  |
| GET/POST/PUT/DELETE | `/api/vacinas`  | CRUD de vacinas (`?petId=`)      | Sim  |

## Observabilidade

### Health Checks

- **Endpoint:** `GET /health` (aberto, sem token, para monitoramento externo).
- Usa `Microsoft.Extensions.Diagnostics.HealthChecks`. A verificacao `oracle`
  (`OracleHealthCheck`) testa a conectividade real com o banco via `Database.CanConnectAsync()`.
- **Resposta (JSON):**

```json
{
  "status": "Healthy",
  "duracao": "00:00:00.0123",
  "verificacoes": [
    { "nome": "oracle", "status": "Healthy", "descricao": "Conexao com o Oracle OK." }
  ]
}
```

- **Como monitorar:** aponte um monitor de uptime (ou o proprio orquestrador) para `GET /health`.
  `status` = `Healthy` (HTTP 200) indica app + banco OK; `Unhealthy` (HTTP 503) indica falha de
  conectividade com o Oracle.

### Logs (Serilog)

- Logs estruturados com niveis **Information / Warning / Error**.
- Saida para **console** e para **arquivo** (`logs/animed-<data>.log`, um arquivo por dia).
- `UseSerilogRequestLogging` registra cada requisicao (metodo, rota, status e tempo de resposta),
  servindo de correlacao por requisicao.

### Tracing e metricas (OpenTelemetry)

- Servico identificado como `AnimedApi`.
- **Tracing** e **metricas** de requisicoes HTTP via `AddAspNetCoreInstrumentation`, exportados para
  o **console** (`AddConsoleExporter`). As metricas incluem tempo de resposta e permitem calcular a
  taxa de erros por status HTTP. Trocar o exporter (ex.: OTLP) integra a um backend como Jaeger/Prometheus.

## Testes

Estrutura separada em dois projetos:

- **`AnimedApi.UnitTests`** — testes unitarios (xUnit + NSubstitute), padrao **AAA** e nomes
  `Metodo_Cenario_ResultadoEsperado`:
  - `Domain/` — invariantes das entidades (ex.: peso do pet > 0, data de vacina nao futura).
  - `Services/` — regra de aplicacao com repositorios mockados.
- **`AnimedApi.IntegrationTests`** — sobem a API real com `WebApplicationFactory<Program>` e um
  **SQLite in-memory** no lugar do Oracle. Usam **Collection Fixture** para compartilhar a instancia.
  Cobrem autenticacao (login valido/invalido, 401 sem token) e endpoints (respostas de sucesso e de erro).

Rodar todos:

```bash
dotnet test          # na raiz da solution (Animed.slnx)
```

## Observacoes

- Os testes de integracao usam SQLite in-memory: validam roteamento, autenticacao, status e regra de
  negocio, mas **nao** validam detalhes especificos do Oracle (tipos, identity, CHECK). Isso e
  intencional para os testes rodarem sem banco externo.
- Aviso `NU1903` (pacote transitivo `Microsoft.OpenApi`, usado pelo Swashbuckle): nao e bloqueante;
  sera resolvido ao atualizar o Swashbuckle para uma versao que traga o pacote corrigido.
