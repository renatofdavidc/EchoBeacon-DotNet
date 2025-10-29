# EchoBeacon-DotNet

## Integrantes

- Gustavo Lopes Santos da Silva - RM: 556859  
- Renato de Freitas David Campiteli - RM: 555627  
- Gabriel Santos Jablonski - RM: 555452  

## Objetivo

API RESTful para gerenciamento de motos, dispositivos EchoBeacons e registros de localizações (rastreamento de frota simplificado). O sistema permite que funcionários localizem motos no pátio através da ativação de LED e som nos dispositivos EchoBeacon acoplados às motos.

## ✨ Novas Funcionalidades Implementadas

### 🔐 Segurança com API Key
- Autenticação via API Key em todos os endpoints (exceto Swagger e Health Checks)
- Header necessário: `X-Api-Key: EchoBeacon2025SecureKey!@#`
- Middleware customizado para validação de autenticação

### 🔄 Versionamento de API
- Implementado versionamento de API usando `Microsoft.AspNetCore.Mvc.Versioning`
- Versão atual: **v1**
- Endpoints com rota: `/api/v1/{controller}`
- Suporte para múltiplas versões futuras

### 🏥 Health Checks
- Endpoint `/health` para verificação do status da API
- Verifica conectividade com o banco de dados Oracle
- Retorna JSON com status detalhado de cada componente
- Não requer API Key

### 📝 Documentação Swagger Aprimorada
- Interface Swagger com suporte para API Key
- Documentação XML dos endpoints
- Versionamento integrado ao Swagger UI

## Tecnologias

- [.NET 8 (ASP.NET Core Web API)](https://learn.microsoft.com/pt-br/aspnet/core/introduction-to-aspnet-core)
- [Entity Framework Core 9.0.5](https://learn.microsoft.com/pt-br/ef/core/)
- [Oracle Database](https://www.oracle.com/database/) com Oracle.EntityFrameworkCore 9.23.80
- [Swagger / Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) para documentação da API
- Microsoft.AspNetCore.Mvc.Versioning 5.1.0
- Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore 8.0.0
- Visual Studio ou VS Code
- Arquitetura em camadas (Repository Pattern)

## Estrutura (pastas principais)

```
Controllers/      # Controllers da API (Motos, EchoBeacons, Localizações)
Models/           # Modelos de domínio (Moto, EchoBeacon, Localizacao)
DTOs/             # Data Transfer Objects para requests e responses
Repositories/     # Implementação do Repository Pattern
Interfaces/       # Interfaces para repositórios
Filters/          # Filtros para consultas
Data/             # Contexto do Entity Framework
Migrations/       # Migrações do banco de dados
Middleware/       # Middleware customizado (API Key Authentication)
```

## Funcionalidades

- **Motos**: CRUD completo, filtro por placa/modelo, paginação, associação 1:1 com EchoBeacon
- **EchoBeacons**: CRUD completo, filtro por número/data, vínculo opcional à moto
- **Localizações**: Registro histórico (moto + opcional beacon), filtros por setor/status/data, paginação
- **Relacionamentos**: 
  - Moto ↔ EchoBeacon (1:1 opcional)
  - Moto → Localizações (1:N)
  - EchoBeacon → Localizações (1:N opcional)

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Oracle Database (local ou na nuvem)
- Visual Studio 2022 ou VS Code
- Entity Framework Core Tools

## Execução

### 1. Clone o repositório

```bash
git clone https://github.com/renatofdavidc/EchoBeacon-DotNet.git
cd EchoBeacon-DotNet
```

### 2. Instale as dependências

```bash
dotnet restore
```

### 3. Configure o banco de dados

Edite o arquivo `appsettings.json` com sua string de conexão do Oracle:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_HOST/SEU_SERVICE_NAME;"
  },
  "ApiKey": "EchoBeacon2025SecureKey!@#"
}
```

**Exemplo de configuração:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=hr;Password=oracle;Data Source=localhost:1521/XE;"
  },
  "ApiKey": "EchoBeacon2025SecureKey!@#"
}
```

### 4. Instale o Entity Framework Core Tools (se necessário)

```bash
dotnet tool install --global dotnet-ef
```

### 5. Execute as migrações para criar o banco

```bash
dotnet ef database update
```

> ⚠️ **Importante**: Certifique-se de que o Oracle Database está rodando e acessível antes de executar as migrações.

### 6. Execute o projeto

```bash
dotnet run
```

### 7. Acesse a API

- **API Base**: `http://localhost:5207` (HTTP) / `https://localhost:7262` (HTTPS)
- **Swagger UI**: `http://localhost:5207/swagger`
- **Health Check**: `http://localhost:5207/health`

## 🔐 Autenticação

Todos os endpoints (exceto `/swagger` e `/health`) requerem autenticação via API Key.

### Como usar a API Key

**No Swagger UI:**
1. Acesse `/swagger`
2. Clique no botão "Authorize" (cadeado verde no topo)
3. Insira a API Key: `EchoBeacon2025SecureKey!@#`
4. Clique em "Authorize"

**Em requisições HTTP:**
```bash
curl -H "X-Api-Key: EchoBeacon2025SecureKey!@#" http://localhost:5207/api/v1/motos
```

**No Postman/Insomnia:**
- Adicione um header:
  - **Key**: `X-Api-Key`
  - **Value**: `EchoBeacon2025SecureKey!@#`

## Endpoints

### Health Check - `/health`

| Método | Endpoint  | Descrição                              | Autenticação |
|--------|-----------|----------------------------------------|--------------|
| GET    | `/health` | Verifica status da API e banco de dados | ❌ Não      |

**Resposta exemplo:**
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "description": null,
      "duration": 45.2
    },
    {
      "name": "api",
      "status": "Healthy",
      "description": "API está funcionando",
      "duration": 0.1
    }
  ],
  "totalDuration": 45.3
}
```

### Motos - `/api/v1/motos`

| Método | Endpoint                | Descrição                              | Parâmetros                    |
|--------|-------------------------|----------------------------------------|-------------------------------|
| GET    | `/api/v1/motos`         | Lista motos com filtros e paginação   | `modelo`, `placa`, `page`, `size` |
| GET    | `/api/v1/motos/{id}`    | Busca moto por ID                      | `id` (path parameter)         |
| POST   | `/api/v1/motos`         | Cria nova moto                         | Request body (JSON)           |
| PUT    | `/api/v1/motos/{id}`    | Atualiza moto existente                | `id` + Request body (JSON)    |
| DELETE | `/api/v1/motos/{id}`    | Remove moto                            | `id` (path parameter)         |

**Request Body para POST/PUT:**
```json
{
  "placa": "ABC1234",
  "modelo": "Honda CG 160",
  "echoBeaconId": 1
}
```

### EchoBeacons - `/api/v1/echobeacons`

| Método | Endpoint                     | Descrição                              | Parâmetros                    |
|--------|------------------------------|----------------------------------------|-------------------------------|
| GET    | `/api/v1/echobeacons`        | Lista EchoBeacons com filtros          | `numeroIdentificacao`, `dataRegistro`, `page`, `size` |
| GET    | `/api/v1/echobeacons/{id}`   | Busca EchoBeacon por ID                | `id` (path parameter)         |
| POST   | `/api/v1/echobeacons`        | Cria novo EchoBeacon                   | Request body (JSON)           |
| PUT    | `/api/v1/echobeacons/{id}`   | Atualiza EchoBeacon existente          | `id` + Request body (JSON)    |
| DELETE | `/api/v1/echobeacons/{id}`   | Remove EchoBeacon                      | `id` (path parameter)         |

**Request Body para POST/PUT:**
```json
{
  "numeroIdentificacao": "ECH001",
  "dataRegistro": "2025-01-15T10:30:00",
  "motoId": 1
}
```

### Localizações - `/api/v1/localizacoes`

| Método | Endpoint                       | Descrição                              | Parâmetros                    |
|--------|--------------------------------|----------------------------------------|-------------------------------|
| GET    | `/api/v1/localizacoes`         | Lista localizações com filtros        | `motoId`, `setor`, `status`, `dataInicio`, `dataFim`, `page`, `size` |
| GET    | `/api/v1/localizacoes/{id}`    | Busca localização por ID              | `id` (path parameter)         |
| POST   | `/api/v1/localizacoes`         | Registra nova localização             | Request body (JSON)           |
| PUT    | `/api/v1/localizacoes/{id}`    | Atualiza localização existente        | `id` + Request body (JSON)    |
| DELETE | `/api/v1/localizacoes/{id}`    | Remove localização                     | `id` (path parameter)         |

**Request Body para POST/PUT:**
```json
{
  "motoId": 1,
  "echoBeaconId": 1,
  "setor": "Patio",
  "status": 1
}
```

**Status (enum `LocalizacaoStatus`):**
- `0` - Recebida
- `1` - Patio
- `2` - EmReparo
- `3` - Finalizada

## Exemplos de Uso

### Verificar Health da API (Sem API Key)
```bash
GET http://localhost:5207/health
```

### Criar uma Moto (Com API Key)
```bash
POST http://localhost:5207/api/v1/motos
X-Api-Key: EchoBeacon2025SecureKey!@#
Content-Type: application/json

{
  "placa": "MOT2025",
  "modelo": "Honda CG 160"
}
```

### Listar Motos com Filtros (Com API Key)
```bash
GET http://localhost:5207/api/v1/motos?modelo=Honda&page=1&size=10
X-Api-Key: EchoBeacon2025SecureKey!@#
```

### Criar um EchoBeacon (Com API Key)
```bash
POST http://localhost:5207/api/v1/echobeacons
X-Api-Key: EchoBeacon2025SecureKey!@#
Content-Type: application/json

{
  "numeroIdentificacao": "ECHO2025",
  "dataRegistro": "2025-01-15T08:00:00"
}
```

### Registrar Localização (Com API Key)
```bash
POST http://localhost:5207/api/v1/localizacoes
X-Api-Key: EchoBeacon2025SecureKey!@#
Content-Type: application/json

{
  "motoId": 1,
  "echoBeaconId": 1,
  "setor": "Patio",
  "status": 1
}
```

## 🧪 Testes

O projeto está preparado para testes unitários e de integração com xUnit.

### Estrutura de Testes (a ser implementada)
```
ProjetoChallengeMottu.Tests/
├── Repositories/          # Testes unitários dos repositórios
├── Integration/           # Testes de integração da API
└── ProjetoChallengeMottu.Tests.csproj
```

### Executar Testes
```bash
# Na pasta do projeto de testes
cd ProjetoChallengeMottu.Tests
dotnet test
```

## 📊 Padrões Implementados

- **Repository Pattern**: Separação da lógica de acesso a dados
- **DTO Pattern**: Transferência de dados entre camadas
- **Dependency Injection**: Injeção de dependências nativa do .NET
- **RESTful Design**: Seguindo princípios REST
- **HATEOAS**: Links de navegação nos responses paginados

## 🔄 Versionamento da API

A API está versionada e pronta para evoluções futuras:
- **Versão atual**: v1
- **Formato da rota**: `/api/v{version}/{controller}`
- **Headers de versão**: `api-supported-versions` indica versões disponíveis

## 🚀 Próximos Passos

- Implementação completa de testes unitários e de integração
- Implementação de Machine Learning com ML.NET para previsões
- Docker/Docker Compose para containerização
- CI/CD com GitHub Actions
- Logging estruturado com Serilog
- Cache com Redis
- Rate Limiting

## 📝 Licença

Este projeto foi desenvolvido como parte de um desafio acadêmico da FIAP.

## 👥 Contribuidores

- Gustavo Lopes Santos da Silva
- Renato de Freitas David Campiteli  
- Gabriel Santos Jablonski

### Registrar Localização
```bash
POST /api/localizacoes
Content-Type: application/json

{
  "motoId": 1,
  "echoBeaconId": 1,
  "setor": "Centro de Distribuição",
  "status": "EmTransito"
}
```

## Paginação

Todos os endpoints de listagem suportam paginação:

- `page`: Número da página (default: 1)
- `size`: Itens por página (default: 10)

**Exemplo de resposta paginada:**
```json
{
  "data": [...],
  "page": 1,
  "size": 10,
  "total": 25,
  "totalPages": 3,
  "links": [
    {
      "rel": "self",
      "href": "/api/motos?page=1&size=10"
    },
    {
      "rel": "next", 
      "href": "/api/motos?page=2&size=10"
    }
  ]
}
```

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

