# EchoBeacon-DotNet

API REST para organizar motos em oficinas: cadastro de motos e EchoBeacons, registro de localizações no pátio e previsão de tempo/economia na busca por motos usando ML.NET.

## Stack
- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core + Oracle
- Swagger (Swashbuckle)
- API Versioning (v1)
- Health Checks
- ML.NET (previsão para operação do pátio)

## Recursos principais
- Motos: CRUD, filtros e paginação; vínculo 1:1 com EchoBeacon
- EchoBeacons: CRUD, filtros; vínculo opcional à moto
- Localizações: histórico por moto/beacon, filtros e paginação
- Segurança: API Key em todos os endpoints (exceto Swagger e /health)
- Previsões (ML):
  - Tempo para localizar moto no pátio
  - Economia mensal (R$) com base no tempo previsto

## Configuração
Edite `appsettings.json`:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_HOST/SEU_SERVICE_NAME;"  
  },
  "ApiKey": "EchoBeacon2025SecureKey!@#"
}
```

EF Tools (opcional para migrações):
```
dotnet tool install --global dotnet-ef
dotnet ef database update
```

## Executar
```
dotnet restore
dotnet run
```

- Base: http://localhost:5207
- Swagger: http://localhost:5207/swagger
- Health: http://localhost:5207/health (sem API Key)

## Autenticação (API Key)
- Header: `X-Api-Key: EchoBeacon2025SecureKey!@#`
- No Swagger, clique em “Authorize” e informe a chave.

## Endpoints (resumo)
- Motos: `/api/v1/motos`
- EchoBeacons: `/api/v1/echobeacons`
- Localizações: `/api/v1/localizacoes`
- ML (Pátio): `/api/v1/ml`
  - POST `/predict-search-time` → body:
    ```json
    {
      "patioAreaM2": 1500,
      "motosNoPatio": 60,
      "percentualComBeacon": 0.8,
      "funcionariosBuscando": 2,
      "horaPico": 0
    }
    ```
  - POST `/predict-savings` → body:
    ```json
    {
      "patioAreaM2": 1500,
      "motosNoPatio": 60,
      "percentualComBeacon": 0.8,
      "funcionariosBuscando": 2,
      "horaPico": 1,
      "baselineTimePerSearchMin": 12,
      "searchesPerDay": 25,
      "hourlyCostBRL": 45
    }
    ```

Observações:
- `percentualComBeacon` aceita fração (0..1) ou porcentagem (0..100).
- Os modelos de ML são treinados em memória na inicialização (dados sintéticos, demonstrativos).

## Exemplos rápidos (HTTP)
- Criar moto:
  ```
  POST /api/v1/motos
  X-Api-Key: EchoBeacon2025SecureKey!@#
  Content-Type: application/json

  { "placa": "MOT2025", "modelo": "Honda CG 160" }
  ```
- Prever tempo de busca:
  ```
  POST /api/v1/ml/predict-search-time
  X-Api-Key: EchoBeacon2025SecureKey!@#
  Content-Type: application/json

  { "patioAreaM2": 1500, "motosNoPatio": 60, "percentualComBeacon": 0.8, "funcionariosBuscando": 2, "horaPico": 0 }
  ``
```

## Testes

- Os testes ficam em `ProjetoChallengeMottu.Tests` (xUnit), cobrindo repositórios (EF InMemory) e integração (middleware de API Key e endpoints de ML).
- Para executar:
  - Windows PowerShell:
    - `dotnet test .\ProjetoChallengeMottu.Tests\ProjetoChallengeMottu.Tests.csproj`
  - Dica: se ocorrer erro de cópia/lock de arquivo no Windows, rode `dotnet clean` na solução e tente novamente.
- Observações:
  - Os testes de integração usam uma fábrica customizada (`CustomWebApplicationFactory`) que sobrescreve o `ApiKey` para `TestKey123!` em memória.
  - Portanto, não é necessário alterar `appsettings.json` para os testes.
  - Banco de dados Oracle não é necessário para os testes de ML e boa parte dos testes de repositório (usam EF InMemory).

## Comandos úteis

### Entity Framework

```bash
# Adicionar nova migração
dotnet ef migrations add NomeDaMigracao

# Aplicar migrações ao banco
dotnet ef database update

# Reverter para migração específica
dotnet ef database update NomeDaMigracao

# Remover última migração (não aplicada)
dotnet ef migrations remove

# Ver status das migrações
dotnet ef migrations list
```

### Build e Execução

```bash
# Restaurar dependências
dotnet restore

# Compilar o projeto
dotnet build

# Executar em modo desenvolvimento
dotnet run

# Executar com profile específico
dotnet run --launch-profile https

# Compilar para produção
dotnet publish -c Release
```

## Testes via cURL

**Listar todas as motos:**
```bash
curl -X GET "http://localhost:5207/api/motos" -H "accept: application/json"
```

**Criar uma nova moto:**
```bash
curl -X POST "http://localhost:5207/api/motos" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d "{\"placa\":\"MOT-2025\",\"modelo\":\"Mottu Sport"}"
```

**Buscar moto por ID:**
```bash
curl -X GET "http://localhost:5207/api/motos/1" -H "accept: application/json"
```

## Modelo de Dados (resumo)

### Tabelas Criadas

1. **MOTOS**
   - Id (IDENTITY)
   - Placa (VARCHAR(10), REQUIRED)
   - Modelo (VARCHAR(100), REQUIRED)

2. **ECHOBEACON**
   - Id (IDENTITY)
   - NumeroIdentificacao (VARCHAR(10), REQUIRED)
   - DataRegistro (DATETIME, REQUIRED)
   - MotoId (FK para MOTOS, NULLABLE)

3. **LOCALIZACOES**
   - Id (IDENTITY)
   - MotoId (FK para MOTOS, REQUIRED)
   - EchoBeaconId (FK para ECHOBEACON, NULLABLE)
   - Setor (VARCHAR(50), REQUIRED)
   - Status (INT, REQUIRED)
   - DataHoraRegistro (DATETIME, REQUIRED)

## Configuração

### Variáveis de Ambiente

Você pode usar variáveis de ambiente para configurar a aplicação:

```bash
# String de conexão
export ConnectionStrings__DefaultConnection="User Id=hr;Password=oracle;Data Source=localhost:1521/XE;"

# Ambiente de execução
export ASPNETCORE_ENVIRONMENT=Development
```

### Configuração no appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "User Id=<ID>;Password=<PASSWORD>;Data Source=<HOST>/<SERVICE_NAME>;"
  }
}
```

## Solução de Problemas

### Erro de Conexão com Oracle

1. Verifique se o Oracle Database está rodando
2. Confirme os dados de conexão (host, porta, service name)
3. Teste a conectividade: `tnsping <service_name>`

### Erro nas Migrações

```bash
# Limpar e recriar migrações
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Porta já em uso

Altere as portas no arquivo `Properties/launchSettings.json`:

```json
"applicationUrl": "https://localhost:7262;http://localhost:5207"
```

Projeto acadêmico FIAP Challenge Mottu.

