# Renmember.io — Blueprint do Produto

> **Projeto:** Renmember.io
> **Organização:** GP.Tech
> **Autor:** Gabriel Palheiros
> **Documento:** Blueprint v1 — escopo, arquitetura e roadmap
> **Data:** 04/10/2026
> **Status:** Em definição — decisões abertas na seção 9

---

## 1. Propósito do projeto

Renmember.io é um gestor de atividades pessoais: um lugar único para cadastrar responsabilidades,
vê-las em kanban ou em calendário e ser lembrado delas no momento certo.

**Objetivos, nesta ordem:**

1. **Uso pessoal real.** Resolver a gestão diária das responsabilidades do autor, do jeito que ele
   sempre quis. O primeiro e único usuário do v1 é o próprio autor.
2. **Engenharia de nível sênior.** Ser um projeto exemplar em arquitetura, testes, versionamento
   e CI — demonstrável como portfólio.
3. **Produto futuro.** Estar preparado para virar um serviço online multiusuário, e depois app,
   **sem reescrita**.

**Consequência prática desta ordem:** o v1 roda **somente local**, mas as decisões de modelagem
não podem fechar a porta para o multiusuário online. Preparar o terreno é barato; reescrever é caro.

### Nome

**Renmember.io** — grafado com **"nm"**, intencionalmente. Nunca "corrigir" para *Remember*
em código, documentação, namespace ou repositório.

| Uso | Forma |
|---|---|
| Nome do produto | Renmember.io |
| Repositório | [`GabrielPalheiro/Renmember.io`](https://github.com/GabrielPalheiro/Renmember.io) (público) |
| Pasta local | `D:\Renmember.io` (pasta do clone; qualquer caminho curto serve) |
| Namespace .NET | `Renmember.*` |
| Pacote web | `renmember-web` |

### Não-objetivos do v1

- Não é hospedado. Roda em `docker compose up` na máquina do autor.
- Não é multiusuário. Um único usuário (ver seção 4, "Preparado para multiusuário").
- Não envia e-mail, push ou mensagem. **Lembretes são exibidos dentro da aplicação.**
- Não tem app nativo. O front é responsivo e, no fim do v1, instalável como PWA.
- Não tem colaboração, compartilhamento de listas ou integração com calendários externos.

---

## 2. O problema

| Dor | Como o Renmember.io resolve |
|---|---|
| Responsabilidades espalhadas entre cabeça, bloco de notas e alarmes do celular | Cadastro único de atividades, com referência (link e nota) |
| Não enxergar o que está em andamento e o que está parado | Visão **kanban** por status |
| Não enxergar a semana ou o mês como um todo | Visões de **calendário mensal e semanal** |
| Coisas que se repetem (contas, remédios, rotinas) precisando ser recriadas | **Recorrência**: uma regra gera as ocorrências |
| Esquecer no momento em que importa | **Lembretes in-app** com antecedência configurável |

---

## 3. Atores

| Ator | v1 | Futuro |
|---|---|---|
| **Usuário** | Único, sem login. Identidade fixa resolvida pela infraestrutura | Vários usuários, cada um com seus dados, autenticação obrigatória |

---

## 4. Modelo de domínio

Esboço — os nomes oficiais estão em `docs/dominio.md`.

```
User
 └── Tasks (Atividades)
      ├── Reference          (link e/ou nota de apoio)
      ├── Category           (etiqueta: Casa, Saúde, Finanças…)
      ├── Schedule           (data, ou data + hora, ou sem data)
      ├── RecurrenceRule     (opcional: diária, semanal, mensal, anual)
      │    └── OccurrenceExceptions (ocorrência concluída, pulada ou remarcada)
      └── Reminders          (antecedência: 15 min, 1 h, 1 dia…)
```

### Três visões, um modelo

Kanban, calendário mensal e calendário semanal são **projeções da mesma `Task`**. O kanban lê o
status e a posição; os calendários leem o agendamento. Nenhuma visão tem dado próprio.

### Recorrência: calcular, não materializar

Uma atividade recorrente **não** gera milhares de linhas no banco. As ocorrências de um intervalo
(ex.: o mês exibido) são **calculadas sob demanda** a partir da regra. Só vira linha no banco o que
**diverge** da regra: uma ocorrência concluída, pulada ou remarcada (`OccurrenceException`).

É o padrão usado por calendários maduros: banco enxuto, edição da série inteira trivial, e a
lógica fica no domínio — onde é testável por unidade.

### Tempo

- Atividade **de dia inteiro** usa `DateOnly`. Uma conta que vence dia 10 vence dia 10, sem fuso.
- Atividade **com horário** é armazenada como instante UTC (`timestamptz`) + fuso do usuário
  (`America/Sao_Paulo` no v1, configurável depois).
- Recorrência é calculada **no fuso do usuário**, não em UTC — senão "todo dia às 8h" desliza no
  horário de verão de outros fusos.
- `TimeProvider` injetado em todo lugar. Nunca `DateTime.Now`.

### Preparado para multiusuário

O v1 é de um usuário só, mas:

- Toda entidade raiz tem `OwnerId` desde a primeira migration.
- O acesso ao "usuário atual" passa por `ICurrentUser`. No v1, a implementação devolve o usuário
  semeado; no futuro, lê o claim do JWT. **Nenhuma outra linha de código muda.**
- Global query filter por `OwnerId` já ativo, com teste de integração provando o isolamento.

Custo hoje: quase zero. Custo de não fazer: revisar toda consulta do sistema quando for ao ar.

---

## 5. Arquitetura e stack

### Visão geral (v1, local)

```
┌───────────────────────────────┐
│  Next.js (App Router) · React │   http://localhost:3000
│  Kanban · Calendário · Lista  │
└───────────────┬───────────────┘
                │ HTTP/JSON
┌───────────────▼───────────────┐
│  API .NET 10 · Minimal APIs   │   http://localhost:8080
└───────────────┬───────────────┘
                │
┌───────────────▼───────────────┐
│  PostgreSQL 17 (container)    │
└───────────────────────────────┘

        Tudo sobe com: docker compose up
```

### Backend — .NET 10

| Item | Escolha | Por quê |
|---|---|---|
| Runtime | .NET 10 (LTS) | Suporte longo |
| Estrutura | Clean Architecture em 4 projetos, features organizadas por pasta dentro de `Application` | Regra de dependência verificável por teste; navegação por feature |
| API | Minimal APIs + endpoint groups | Menos cerimônia |
| ORM | EF Core 10 + Npgsql | Migrations versionadas, global query filters |
| Banco | PostgreSQL | Mesmo motor local e, no futuro, em produção (Neon) |
| Validação | FluentValidation | Padrão de mercado |
| Erros | `ProblemDetails` (RFC 9457) | Contrato de erro único para o front |
| Recorrência | Implementação própria no `Domain` | Escopo limitado (4 frequências), 100% testável por TDD, sem dependência |
| Logs | Serilog (console estruturado) | OpenTelemetry entra quando houver para onde enviar |
| Testes | xUnit, FluentAssertions, NSubstitute, Testcontainers, NetArchTest | Postgres real nos testes de integração |
| Docs da API | OpenAPI nativo + Scalar | API autodocumentada |

```
api/
  src/
    Renmember.Domain/          entidades, value objects, regras puras (recorrência mora aqui)
    Renmember.Application/     casos de uso por feature: Tasks/, Kanban/, Calendar/, Reminders/
    Renmember.Infrastructure/  EF Core, migrations, ICurrentUser, relógio
    Renmember.Api/             endpoints, composição, ProblemDetails
  tests/
    Renmember.Domain.UnitTests/
    Renmember.Application.UnitTests/
    Renmember.Api.IntegrationTests/     Testcontainers + WebApplicationFactory
    Renmember.ArchitectureTests/
```

### Frontend — Next.js + React

| Item | Escolha | Por quê |
|---|---|---|
| Framework | Next.js (App Router) | Mesma base do Alugarme; prepara o caminho para hospedagem |
| Linguagem | TypeScript `strict` | Não negociável |
| Estilo | Tailwind CSS + shadcn/ui | Só tokens do design system |
| Estado servidor | TanStack Query | Cache, atualização otimista no arrastar do kanban |
| Formulários | React Hook Form + Zod | Validação no client espelhando a da API |
| Kanban | dnd-kit | Drag-and-drop com suporte a teclado |
| Calendário | FullCalendar (plugins de licença MIT: mês, semana, interação) | Lib pronta, decisão do autor. Conferir licença dos plugins na Fatia 3 |
| Contrato da API | Cliente TypeScript gerado do OpenAPI | Front e back não divergem em silêncio |
| Testes | Vitest, Testing Library, MSW, Playwright | Unidade, componente com API simulada, E2E |

**Renderização:** a aplicação inteira é interativa e de um usuário — Client Components com
TanStack Query. Server Components ficam para o shell e páginas estáticas.

### Lembretes in-app (v1)

Sem push e sem e-mail. O fluxo:

1. O usuário configura lembretes na atividade (ex.: 1 dia antes, 15 minutos antes).
2. A API expõe `GET /reminders/due` com os lembretes vencidos e ainda não dispensados.
   O cálculo considera as ocorrências de atividades recorrentes.
3. O front consulta esse endpoint periodicamente (TanStack Query, intervalo de 1 minuto) e exibe:
   - um aviso (toast) dentro da aplicação;
   - um contador na central de lembretes;
   - uma notificação do navegador (Notification API), se o usuário permitir — funciona enquanto
     a aba estiver aberta, mesmo em segundo plano.
4. O usuário **dispensa** ou **adia** o lembrete.

**Por que polling e não SignalR no v1:** um usuário, precisão de minuto, zero infraestrutura extra
e trivial de testar. O endpoint `due` é o mesmo que um agendador vai consumir quando entrarem
e-mail ou push — a troca do canal não muda o domínio. Registrar em ADR.

### Ordenação no kanban

Cada cartão tem uma `Position` em **indexação fracionária** (chaves textuais ordenáveis). Mover um
cartão altera **uma** linha, não a coluna inteira. Testes cobrem inserção entre vizinhos, no topo,
no fim e em coluna vazia.

---

## 6. Ambiente e DevOps

### Ambiente

Só desenvolvimento, local: **Windows 11 · PowerShell 7 · VS Code · Docker Desktop**.

`docker compose up` sobe Postgres, API e Web. Qualquer pessoa com Docker roda o projeto.

### Controle de versão

- Repositório **público** no GitHub: [`GabrielPalheiro/Renmember.io`](https://github.com/GabrielPalheiro/Renmember.io). O código é visível; a aplicação roda só localmente
- `main` protegida por ruleset (PR obrigatório, check `ci` verde, squash only)
- Secret scanning com push protection e Dependabot ativos (gratuitos em repositório público)
- **Trunk-based**: `main` sempre verde; toda atividade em branch curta
- **Conventional Commits** em português
- **Squash merge** como único jeito de mesclar
- **release-please** gera versão semântica, tag, `CHANGELOG.md` e GitHub Release a partir dos commits

Detalhes operacionais em `docs/contribuindo.md`.

### CI — GitHub Actions

Roda em todo PR para `main`. Nenhum merge com CI vermelho.

| Job | O que valida |
|---|---|
| `api-quality` | `dotnet format --verify-no-changes`, build com warnings como erro |
| `api-tests` | Unidade, arquitetura e integração (Testcontainers com Postgres real) |
| `db-migrations` | Todas as migrations aplicadas num banco vazio; nenhuma mudança de modelo sem migration; script SQL idempotente publicado como artefato do PR |
| `web-quality` | ESLint, `typecheck`, Prettier |
| `web-tests` | Vitest + Testing Library |
| `contract` | Cliente TypeScript regenerado do OpenAPI é idêntico ao commitado |
| `e2e` | Playwright contra a stack completa em `docker compose` |
| `ci` | Job agregador — é o único check exigido pela proteção da `main` |

### CD

**Não há deploy no v1.** O "entregável" de cada fatia é: mergeado na `main`, CI verde, versão
gerada pelo release-please e rodando em `docker compose up`.

O pipeline já produz os artefatos que o deploy futuro vai usar: imagem Docker da API, imagem da
web e *migration bundle* (`efbundle`). Quando o projeto for ao ar, o CD é um workflow novo, não
uma reestruturação.

### Migrations

- Versionadas no repositório, commitadas **no mesmo PR** da feature que as exige
- Em desenvolvimento: aplicadas no startup da API, para conveniência
- Em qualquer ambiente futuro: aplicadas por *migration bundle* como passo de deploy, **nunca** no startup
- Mudança destrutiva segue **expand → migrate → contract** em releases separadas
- Uma migration mesclada na `main` **nunca é editada** — corrige-se com uma nova

---

## 7. Roadmap

Detalhado em `docs/roadmap.md`. Resumo:

| Fatia | Entrega | Sessões |
|---|---|---|
| 0 | Fundação: monorepo, compose, CI, release-please, primeira migration | ~3 |
| 1 | Cadastro de atividades, com referência e categoria | ~4 |
| 2 | Kanban com arrastar e soltar | ~4 |
| 3 | Calendário mensal e semanal | ~4 |
| 4 | Recorrência | ~5 |
| 5 | Lembretes in-app | ~4 |
| 6 | Acabamento: PWA, dark mode, acessibilidade, README e ADRs | ~3 |

**Caminho para online (pós-v1):** autenticação, hospedagem, CD, e-mail ou push, app via Capacitor.

---

## 8. Decisões arquiteturais registradas

| # | Decisão | Alternativa descartada | Justificativa |
|---|---|---|---|
| 001 | v1 somente local, ambiente único de desenvolvimento | Hospedar desde o início | Foco em deixar o produto como o autor quer antes de pagar e operar infraestrutura |
| 002 | Modelo preparado para multiusuário (`OwnerId`, `ICurrentUser`) | Ignorar usuário no v1 | Custo quase zero hoje; evita reescrita ao ir para online |
| 003 | Clean Architecture em 4 projetos | Projeto único com vertical slices | Regra de dependência verificável por NetArchTest; objetivo de portfólio sênior |
| 004 | PostgreSQL | SQL Server | Mesmo motor local e no futuro free tier gerenciado (Neon) |
| 005 | Recorrência calculada sob demanda + exceções | Materializar todas as ocorrências | Banco enxuto, edição de série trivial, regra testável no domínio |
| 006 | Recorrência implementada no domínio | Biblioteca RRULE (Ical.Net) | Escopo limitado a 4 frequências; TDD puro; uma dependência a menos |
| 007 | Lembretes in-app por polling | SignalR, push, e-mail | Um usuário, precisão de minuto basta; o endpoint serve aos canais futuros |
| 008 | FullCalendar | Calendário próprio | Velocidade; decisão do autor |
| 009 | Indexação fracionária no kanban | Inteiro sequencial | Mover cartão altera uma linha só |
| 010 | Next.js mesmo sem SEO | Vite + React SPA | Prepara a hospedagem futura; mesma base do Alugarme |
| 011 | PWA antes de app nativo | React Native, MAUI | Instalável e reaproveita 100% do front; Capacitor depois |
| 012 | Repositório público, aplicação só local | Repositório privado (com ou sem GitHub Pro) | No GitHub Free, rulesets só valem em repositório público; também habilita secret scanning gratuito e serve de portfólio |

---

## 9. Decisões abertas

| # | Questão | Opções |
|---|---|---|
| ~~D1~~ | ~~Visibilidade do repositório~~ | **Resolvida em 04/10/2026:** público (ADR 012) |
| D2 | Atividade recorrente no kanban | (a) cartão representa a série; (b) cartão representa a próxima ocorrência; (c) só ocorrências de hoje/semana aparecem como cartões |
| D3 | Estados do kanban | Fixos (`ToDo`, `Doing`, `Done`) ou colunas personalizáveis |

---

## 10. Riscos

| Risco | Probabilidade | Mitigação |
|---|---|---|
| Abandono por ritmo irregular | Alta | Fatias que sempre terminam mergeadas e utilizáveis |
| Scope creep ("já que estou aqui…") | Alta | Backlog pós-v1 explícito; ideia nova entra lá |
| Recorrência com bordas difíceis (dia 31, 29/02, mudança de fuso) | Média | TDD com tabela de casos antes da implementação |
| Fuso e horário de verão | Média | Regras de tempo da seção 4; testes com `FakeTimeProvider` |
| Segredo exposto no repositório público | Baixa/Crítica | `.gitignore` bloqueando `.env`; secret scanning com push protection; nenhum segredo real no v1 local |
| CI lento travando o ritmo | Média | Cache de NuGet/pnpm, jobs em paralelo, filtros de caminho |
| Decisões do v1 travando o online | Baixa | `OwnerId` e `ICurrentUser` desde a Fatia 0; artefatos de deploy já gerados no CI |
