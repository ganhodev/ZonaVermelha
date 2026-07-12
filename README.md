# 🔴 Zona Vermelha

Aplicação backend de mapeamento colaborativo de áreas de risco, onde usuários podem registrar ocorrências criminais (assaltos, roubos, furtos) próximas à sua localização. Relatos próximos entre si formam **zonas vermelhas** no mapa, que aumentam de intensidade conforme novos relatos chegam e desaparecem automaticamente após 6 horas sem atividade.

> Projeto desenvolvido como primeiro projeto real, com foco em aprendizado prático de arquitetura em camadas, geolocalização, comunicação em tempo real e boas práticas de desenvolvimento backend em .NET 10.

---

## Funcionalidades

- Registro de relatos de crimes com descrição e localização (lat/lng)
- Agrupamento automático de relatos próximos em zonas (raio de 200m, via fórmula de Haversine)
- Intensidade da zona aumenta a cada novo relato associado
- Busca de zonas ativas próximas a uma coordenada (raio de 5km)
- Notificações em tempo real via SignalR quando zonas surgem ou se intensificam
- Expiração automática de zonas sem atividade há mais de 6 horas
- Histórico de relatos preservado mesmo após expiração da zona
- Tratamento de erros global com respostas JSON padronizadas

---

## Arquitetura

A solução segue o padrão de **arquitetura em camadas**, com 4 projetos separados por responsabilidade:

```
ZonaVermelha/
├── ZonaVermelha.API           → Controllers, Services, Middlewares, Hubs, BackgroundService
├── ZonaVermelha.Domain        → Entidades, regras de negócio, exceções customizadas
├── ZonaVermelha.Infrastructure → DbContext, EF Core, migrations, acesso a dados
└── ZonaVermelha.Communication → DTOs de request/response (contratos da API)
```

### Por que essa separação?

- **Domain** não depende de nenhuma biblioteca externa — as regras de negócio são puras e testáveis isoladamente
- **Infrastructure** conhece o Domain, mas o Domain não sabe que ela existe
- **Communication** define o contrato público da API sem expor entidades internas
- **API** orquestra tudo — é a única camada que conhece as outras três

Essa separação permite, por exemplo, trocar o SQLite por PostgreSQL alterando só o `Infrastructure`, sem tocar em nenhuma outra camada.

---

## Decisões técnicas

### Agrupamento de relatos por proximidade (Haversine)

Coordenadas geográficas não podem ser comparadas por subtração simples — a Terra é uma esfera. A [fórmula de Haversine](https://en.wikipedia.org/wiki/Haversine_formula) calcula a distância real em metros entre dois pontos dados por latitude e longitude, considerando a curvatura terrestre.

Implementada em `ZonaVermelha.Domain/CalculadoraDistancia.cs` como classe estática (função matemática pura, sem estado).

```
Fluxo de um novo relato:
1. Busca todas as zonas ativas no banco
2. Calcula a distância Haversine entre o relato e cada zona
3. Se alguma zona estiver a menos de 200m → associa o relato e incrementa intensidade
4. Se nenhuma zona estiver próxima → cria uma nova zona naquela coordenada
5. Salva no banco e notifica clientes via SignalR
```

### Expiração automática de zonas

Um `BackgroundService` (`ZonaExpiracaoService`) roda a cada 5 minutos e verifica zonas cuja `UltimaAtividade` foi há mais de 6 horas. Ao expirar uma zona:

- Os **relatos históricos são preservados** (`ZonaId` é setado para `null`, não deletado)
- A zona é removida do banco
- Clientes conectados são notificados via SignalR com o evento `ZonaExpirada`

### Tratamento de erros

Um middleware global (`ErrorHandlingMiddleware`) intercepta todas as exceções e retorna respostas JSON padronizadas:

| Exceção | Status HTTP | Quando é lançada |
|---|---|---|
| `ValidacaoException` | 400 Bad Request | Dados de entrada inválidos |
| `NotFoundException` | 404 Not Found | Recurso não encontrado no banco |
| `Exception` (genérica) | 500 Internal Server Error | Erros inesperados |

### Tempo de vida dos serviços

| Serviço | Lifetime | Motivo |
|---|---|---|
| `ZonaVermelhaDbContext` | Scoped | Uma instância por requisição HTTP |
| `RelatoService`, `ZonaService`, `UsuarioService` | Scoped | Dependem do DbContext |
| `ZonaExpiracaoService` | Singleton (Hosted) | BackgroundService — vive o tempo todo |

O `BackgroundService` usa `IServiceProvider` para criar escopos manualmente a cada iteração, evitando o problema de injetar um serviço `Scoped` num `Singleton`.

---

## Stack técnica

| Tecnologia | Uso |
|---|---|
| .NET 10 | Plataforma |
| ASP.NET Core Web API | Endpoints REST |
| Entity Framework Core 10 | ORM e migrations |
| SQLite | Banco de dados |
| SignalR | Comunicação em tempo real (WebSocket) |
| BackgroundService | Expiração automática de zonas |

---

## Endpoints da API

### Usuários

```
POST /api/usuarios
```
```json
{
  "nome": "string",
  "email": "string"
}
```

### Relatos

```
POST /api/relatos
```
```json
{
  "descricao": "string",
  "latitude": -20.2900,
  "longitude": -40.2925,
  "usuarioId": "guid"
}
```

Validações:
- `descricao` não pode ser vazia
- `latitude` deve estar entre -90 e 90
- `longitude` deve estar entre -180 e 180
- `usuarioId` deve ser um GUID válido e existente

### Zonas

```
GET /api/zonas?latitude=-20.2900&longitude=-40.2925
```

Retorna todas as zonas ativas dentro de 5km da coordenada informada.

---

## SignalR

O Hub está disponível em `/hubs/zonas`.

### Eventos disponíveis

| Evento | Direção | Payload | Quando |
|---|---|---|---|
| `EntrarNaRegiao(regiaoId)` | Cliente → Servidor | string | Cliente entra num grupo de zona |
| `ZonaAtualizada` | Servidor → Cliente | dados da zona | Nova zona criada ou intensificada |
| `ZonaExpirada` | Servidor → Cliente | IdZona (Guid) | Zona removida por inatividade |

---

## Como rodar localmente

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022+ ou VS Code

### Passos

1. Clone o repositório:
```bash
git clone https://github.com/ganhodev/ZonaVermelha.git
cd ZonaVermelha
```

2. Aplique as migrations (cria o banco SQLite automaticamente):
```bash
cd ZonaVermelha.API
dotnet ef database update --project ../ZonaVermelha.Infrastructure
```

3. Rode a aplicação:
```bash
dotnet run
```

A API estará disponível em `http://localhost:5083`.

### Testando com o arquivo `.http`

O projeto inclui `ZonaVermelha.API.http` com exemplos de todas as requisições. No Visual Studio, abra o arquivo e clique em **Enviar solicitação** acima de cada request.

Fluxo recomendado para teste:
1. `POST /api/usuarios` — crie um usuário e copie o `id` retornado
2. `POST /api/relatos` — crie um relato usando o `id` do passo anterior
3. `POST /api/relatos` — crie outro relato com coordenadas próximas (deve retornar o mesmo `zonaId`)
4. `GET /api/zonas?latitude=...&longitude=...` — busque as zonas ativas

---

## Estrutura de pastas (ZonaVermelha.API)

```
ZonaVermelha.API/
├── BackgroundServices/
│   └── ZonaExpiracaoService.cs   → Expiração automática de zonas (a cada 5min)
├── Controllers/
│   ├── RelatosController.cs      → POST /api/relatos
│   ├── UsuariosController.cs     → POST /api/usuarios
│   └── ZonasController.cs        → GET /api/zonas
├── Hubs/
│   └── ZonasHub.cs               → Hub SignalR para notificações em tempo real
├── Middlewares/
│   └── ErrorHandlingMiddleware.cs → Tratamento global de exceções
└── Services/
    ├── RelatoService.cs           → Lógica de criação de relatos e agrupamento
    ├── UsuarioService.cs          → Lógica de criação de usuários
    └── ZonaService.cs             → Lógica de busca de zonas por proximidade
```

---

## Próximos passos

- [ ] Autenticação e autorização (JWT)
- [ ] Frontend web (Angular) consumindo a API e o SignalR
- [ ] Deploy (API + banco + frontend)
- [ ] Testes unitários dos serviços

---

Desenvolvido por [ganhodev](https://github.com/ganhodev) como projeto de estudo em .NET/C#.