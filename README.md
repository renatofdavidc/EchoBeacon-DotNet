# Projeto Challenge Mottu

## Finalidade

Este projeto foi desenvolvido como parte de um desafio técnico proposto pela Mottu. O objetivo é construir uma API RESTful para gerenciamento de motos e dispositivos EchoBeacons,
possibilitando o cadastro, atualização, listagem e exclusão de ambos os recursos, com relacionamento entre eles.

## Tecnologias e ferramentas utilizadas

- [.NET 8 (ASP.NET Core)](https://learn.microsoft.com/pt-br/aspnet/core/introduction-to-aspnet-core)
- [Entity Framework Core](https://learn.microsoft.com/pt-br/ef/core/)
- [Swagger / Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server)
- [Docker (opcional)](https://www.docker.com/)
- Visual Studio ou VS Code

## Funcionalidades

### Motos
- Cadastro de motos
- Atualização de dados
- Listagem com filtros e paginação
- Associação com um EchoBeacon
- Exclusão

### EchoBeacons
- Cadastro de dispositivos
- Atualização de dados
- Listagem com filtros e paginação
- Associação com uma Moto
- Exclusão

> O relacionamento é **1:1** entre Moto e EchoBeacon.

## Como rodar o projeto localmente

### 1. Clone o repositório

```bash
git clone https://github.com/renatofdavidc/EchoBeacon-DotNet.git
cd EchoBeacon-DotNet
```

### 2. Configure o banco de dados

- Edite o arquivo `appsettings.json` com sua string de conexão do Oracle:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MottuDb;User Id=usuario;Password=senha;"
}
```

### 3. Execute as migrações

```bash
dotnet ef database update
```

> Instale `dotnet-ef`:
>
> ```bash
> dotnet tool install --global dotnet-ef
> ```

### 4. Execute o projeto

```bash
dotnet run
```

### 5. Acesse o endereço http da aplicação para testar

## 🧪 Testando com Swagger

O Swagger está configurado para ler as informações da rota. Basta acessar:

```
https://localhost:5001/swagger
```

Você poderá visualizar e testar todas as rotas da API diretamente pelo navegador.

## Endpoints da API

### Motos

| Método | Rota               | Descrição                                | Corpo / Query Params |
|--------|--------------------|------------------------------------------|-----------------------|
| GET    | `/api/motos`       | Lista todas as motos com filtros e paginação | `modelo`, `placa`, `page`, `size` |
| GET    | `/api/motos/{id}`  | Retorna uma moto específica por ID       | —                     |
| POST   | `/api/motos`       | Cria uma nova moto                       | `{ placa, modelo, echoBeaconId? }` |
| PUT    | `/api/motos/{id}`  | Atualiza uma moto existente              | `{ placa, modelo, echoBeaconId? }` |
| DELETE | `/api/motos/{id}`  | Remove uma moto                          | —                     |

### EchoBeacons

| Método | Rota                    | Descrição                                     | Corpo / Query Params |
|--------|-------------------------|-----------------------------------------------|-----------------------|
| GET    | `/api/echobeacons`      | Lista todos os EchoBeacons com filtros        | `numeroIdentificacao`, `dataRegistro`, `page`, `size` |
| GET    | `/api/echobeacons/{id}` | Retorna um EchoBeacon específico por ID       | —                     |
| POST   | `/api/echobeacons`      | Cria um novo EchoBeacon                       | `{ numeroIdentificacao, dataRegistro, motoId? }` |
| PUT    | `/api/echobeacons/{id}` | Atualiza um EchoBeacon existente              | `{ numeroIdentificacao, dataRegistro, motoId? }` |
| DELETE | `/api/echobeacons/{id}` | Remove um EchoBeacon                          | —                     |

