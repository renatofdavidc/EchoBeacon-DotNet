# EchoBeacon-DotNet

## Integrantes

- Gustavo Lopes Santos da Silva - RM: 556859  
- Renato de Freitas David Campiteli - RM: 555627  
- Gabriel Santos Jablonski - RM: 555452  

## Objetivo

API RESTful para gerenciamento de motos, dispositivos EchoBeacons e registros de localizações (rastreamento de frota simplificado).

## Tecnologias

- [.NET 8 (ASP.NET Core Web API)](https://learn.microsoft.com/pt-br/aspnet/core/introduction-to-aspnet-core)
- [Entity Framework Core 9.0.5](https://learn.microsoft.com/pt-br/ef/core/)
- [Oracle Database](https://www.oracle.com/database/) com Oracle.EntityFrameworkCore 9.23.80
- [Swagger / Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) para documentação da API
- Visual Studio ou VS Code
- Arquitetura em camadas (Repository Pattern)

## Estrutura (pastas principais)

```
Controllers/  Models/  DTOs/  Repositories/  Interfaces/  Filters/  Data/  Migrations/
```

## Funcionalidades

Motos: CRUD, filtro por placa/modelo, paginação, associação 1:1 com EchoBeacon.
EchoBeacons: CRUD, filtro por número/data, vínculo opcional à moto.
Localizações: registro histórico (moto + opcional beacon), filtros, paginação.
Relacionamentos: Moto- EchoBeacon (1:1 opcional); Moto → Localizações (1:N); EchoBeacon → Localizações (1:N opcional).

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
  }
}
```

**Exemplo de configuração:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=hr;Password=oracle;Data Source=localhost:1521/XE;"
  }
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

### 7. Endpoints base

API: `http://localhost:5207` (HTTP) / `https://localhost:7262` (HTTPS)
Swagger: `/swagger`

## Teste rápido

1. Swagger UI em `/swagger`.
2. Arquivo `ProjetoChallengeMottu.http` (VS Code) pode ser usado.
3. Base URL: `http://localhost:5207`.

## Endpoints

### Motos - `/api/motos`

| Método | Endpoint           | Descrição                              | Parâmetros                    |
|--------|--------------------|----------------------------------------|-------------------------------|
| GET    | `/api/motos`       | Lista motos com filtros e paginação   | `modelo`, `placa`, `page`, `size` |
| GET    | `/api/motos/{id}`  | Busca moto por ID                      | `id` (path parameter)         |
| POST   | `/api/motos`       | Cria nova moto                         | Request body (JSON)           |
| PUT    | `/api/motos/{id}`  | Atualiza moto existente                | `id` + Request body (JSON)    |
| DELETE | `/api/motos/{id}`  | Remove moto                            | `id` (path parameter)         |

**Request Body para POST/PUT:**
```json
{
  "placa": "ABC-1234",
  "modelo": "Honda CG 160",
  "echoBeaconId": 1
}
```

### EchoBeacons - `/api/echobeacons`

| Método | Endpoint                | Descrição                              | Parâmetros                    |
|--------|-------------------------|----------------------------------------|-------------------------------|
| GET    | `/api/echobeacons`      | Lista EchoBeacons com filtros          | `numeroIdentificacao`, `dataRegistro`, `page`, `size` |
| GET    | `/api/echobeacons/{id}` | Busca EchoBeacon por ID                | `id` (path parameter)         |
| POST   | `/api/echobeacons`      | Cria novo EchoBeacon                   | Request body (JSON)           |
| PUT    | `/api/echobeacons/{id}` | Atualiza EchoBeacon existente          | `id` + Request body (JSON)    |
| DELETE | `/api/echobeacons/{id}` | Remove EchoBeacon                      | `id` (path parameter)         |

**Request Body para POST/PUT:**
```json
{
  "numeroIdentificacao": "ECH001",
  "dataRegistro": "2025-01-15T10:30:00",
  "motoId": 1
}
```

### Localizações - `/api/localizacoes`

| Método | Endpoint                  | Descrição                              | Parâmetros                    |
|--------|---------------------------|----------------------------------------|-------------------------------|
| GET    | `/api/localizacoes`       | Lista localizações com filtros        | `motoId`, `setor`, `status`, `dataInicio`, `dataFim`, `page`, `size` |
| GET    | `/api/localizacoes/{id}`  | Busca localização por ID              | `id` (path parameter)         |
| POST   | `/api/localizacoes`       | Registra nova localização             | Request body (JSON)           |
| PUT    | `/api/localizacoes/{id}`  | Atualiza localização existente        | `id` + Request body (JSON)    |
| DELETE | `/api/localizacoes/{id}`  | Remove localização                     | `id` (path parameter)         |

**Request Body para POST/PUT:**
```json
{
  "motoId": 1,
  "echoBeaconId": 1,
  "setor": "Garagem",
  "status": "Entregue"
}
```

Status (enum `LocalizacaoStatus`):
`Recebida` (0), `Patio` (1), `EmReparo` (2), `Finalizada` (3)

## Exemplos

### Criar uma Moto
```bash
POST /api/motos
Content-Type: application/json

{
  "placa": "MOT-2025",
  "modelo": "Mottu Sport"
}
```

### Listar Motos com Filtros
```bash
GET /api/motos?modelo=Mottu%20%sport&page=1&size=5
```

### Criar um EchoBeacon
```bash
POST /api/echobeacons
Content-Type: application/json

{
  "numeroIdentificacao": "ECHO2025",
  "dataRegistro": "2025-01-15T08:00:00"
}
```

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

