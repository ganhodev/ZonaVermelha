Zona Vermelha

Aplicação de mapeamento colaborativo de áreas de risco, permitindo que usuários denunciem ocorrências (assaltos, roubos, etc.) próximas à sua região. Cada denúncia gera ou intensifica uma "zona vermelha" no mapa, que desaparece automaticamente após 6 horas sem novos relatos na área.


🚧 Projeto em desenvolvimento — primeiro projeto real do autor, com foco em aprendizado prático de arquitetura em camadas, EF Core, geolocalização e comunicação em tempo real.



Visão geral


Usuários podem registrar relatos de crimes com características e localização
Relatos próximos entre si se agrupam em uma única zona, aumentando seu nível de intensidade
Zonas sem atividade recente (6h) expiram e somem do mapa automaticamente
Atualizações de zonas são propagadas em tempo real para usuários próximos


Arquitetura

Solução organizada em camadas, seguindo o padrão de projetos separados por responsabilidade:

ZonaVermelha/
├── ZonaVermelha.API              → Controllers, configuração, endpoints REST e SignalR
├── ZonaVermelha.Domain           → Entidades e regras de negócio (sem dependências externas)
├── ZonaVermelha.Infrastructure   → DbContext, EF Core, acesso a dados (SQLite)
└── ZonaVermelha.Communication    → DTOs de request/response

Stack técnica


.NET 10
ASP.NET Core Web API — endpoints REST
SignalR — comunicação em tempo real (zonas surgindo/expirando no mapa)
Entity Framework Core + SQLite — persistência de dados
FluentValidation — validação de regras de negócio (planejado)


Como o sistema decide entre uma zona nova ou existente


Um novo relato chega com sua coordenada (lat/lng)
O sistema verifica se existe uma zona ativa dentro de um raio próximo
Se não existe → cria uma nova zona associada ao relato
Se existe → associa o relato à zona existente e aumenta seu nível de intensidade
Um serviço em segundo plano monitora zonas sem atividade há mais de 6h e as remove, notificando os clientes conectados


Status do desenvolvimento


 Estrutura da solution em camadas
 Modelagem das entidades (Relato, Zona, Usuário)
 Configuração do EF Core + migrations
 Endpoints REST de relatos e zonas
 Hub SignalR para atualizações em tempo real
 Serviço de expiração automática de zonas
 Frontend
 Deploy


Roadmap futuro


Frontend web ou mobile para visualização do mapa
Deploy da API, banco de dados e frontend



Desenvolvido por ganhodev como projeto de estudo em .NET/C#.