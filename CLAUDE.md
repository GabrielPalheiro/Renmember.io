# CLAUDE.md — Regras do projeto Renmember.io

Lido automaticamente pelo Claude Code em toda sessão neste repositório.
É a **fonte da verdade** sobre como se trabalha aqui. Em conflito com qualquer outra orientação,
este arquivo vence.

---

## 1. O que é este projeto

**Renmember.io** — com **"nm"**, intencionalmente. Nunca corrigir para "Remember".

Gestor de atividades pessoais: cadastrar responsabilidades com referência, vê-las em **kanban**,
**calendário mensal** e **calendário semanal**, repeti-las por **recorrência** e ser avisado por
**lembretes dentro da aplicação**.

Produto da **GP.Tech**. Autor: Gabriel Palheiros.
Repositório (público): https://github.com/GabrielPalheiro/Renmember.io — a aplicação roda só localmente.

### Objetivos, nesta ordem

1. **Uso pessoal real** — um usuário só, o autor.
2. **Engenharia de nível sênior** — arquitetura, testes, versionamento e CI exemplares.
3. **Produto futuro** — pronto para virar online e multiusuário sem reescrita.

### Fora de escopo no v1 — não sugerir, não implementar

- Hospedagem, deploy ou qualquer serviço de nuvem (v1 é **somente local**)
- Login e autenticação (existe um usuário semeado; ver seção 3.6)
- E-mail, push, SMS, WhatsApp ou qualquer lembrete fora da aplicação
- Colaboração, compartilhamento ou multiusuário na interface
- App nativo (o fim do v1 é PWA)
- Integração com calendários externos

---

## 2. Stack

| Camada | Tecnologia | Observação |
|---|---|---|
| Backend | .NET 10 (LTS), Minimal APIs | Clean Architecture, 4 projetos |
| ORM | EF Core 10 + Npgsql | Migrations versionadas |
| Banco | PostgreSQL 17 | Container local via Docker Compose |
| Validação | FluentValidation | Erros em `ProblemDetails` |
| Front-end | Next.js (App Router) + React + TypeScript `strict` | |
| Estilo | Tailwind CSS + shadcn/ui | Só tokens do design system |
| Estado servidor | TanStack Query | Atualização otimista no kanban |
| Formulários | React Hook Form + Zod | |
| Kanban | dnd-kit | Com suporte a teclado |
| Calendário | FullCalendar (só plugins MIT) | Estilizado por tokens |
| Contrato | Cliente TS gerado do OpenAPI | Nunca escrever tipos da API à mão |
| Testes backend | xUnit, FluentAssertions, NSubstitute, Testcontainers, NetArchTest | |
| Testes front | Vitest, Testing Library, MSW, Playwright | |
| Versionamento | Conventional Commits + release-please | |
| CI | GitHub Actions | Ver `docs/contribuindo.md` |

**Não troque nenhum item sem registrar um ADR.** Biblioteca nova: pergunte antes de instalar.

---

## 3. Princípios inegociáveis

### 3.1 TDD
Nenhuma linha de produção sem um teste que falhe antes. **Red → Green → Refactor.**
A recorrência e os lembretes, em especial, nascem de tabelas de casos escritas antes do código.

### 3.2 Clean Architecture
`Domain` não conhece ninguém. `Application` conhece só `Domain`. `Infrastructure` e `Api`
implementam e compõem. Verificado por NetArchTest no CI.

### 3.3 Clean Code
Nomes que revelam intenção, funções pequenas, zero comentário óbvio, zero código morto.

### 3.4 Design System
Nenhuma cor, espaçamento, raio, sombra ou fonte literal. Só tokens — inclusive no tema do
FullCalendar e nas cores das categorias.

### 3.5 Linguagem ubíqua
O nome no código é o nome em `docs/dominio.md`. É `Task`, não `Todo` nem `Item`.
É `Category`, não `Tag`. É `Reminder` (configuração) e `ReminderAlert` (disparo).

### 3.6 Preparado para multiusuário
- Toda entidade raiz tem `OwnerId`
- O usuário atual vem **só** de `ICurrentUser` — nunca de corpo, query string ou rota
- Global query filter por `OwnerId` em todas as entidades do usuário
- No v1, `ICurrentUser` devolve o usuário semeado. Não criar atalhos que dependam disso

### 3.7 Tempo
- `TimeProvider` sempre injetado; `FakeTimeProvider` nos testes
- Dia inteiro é `DateOnly`; com horário é instante UTC (`DateTimeOffset`)
- Recorrência calculada no fuso do usuário
- Ocorrências **calculadas**, nunca materializadas; só exceções são persistidas

---

## 4. Como trabalhar aqui

### Fluxo de uma tarefa

1. **Entender** — leia `docs/dominio.md` e o ADR relacionado
2. **Planejar** — para qualquer coisa não trivial, apresente o plano e aguarde aprovação
3. **Testar** — escreva o teste que falha
4. **Implementar** — o mínimo para passar
5. **Refatorar** — com a rede verde
6. **Verificar** — suíte inteira, lint, typecheck, testes de arquitetura
7. **Documentar** — atualize o documento que ficou obsoleto, no mesmo PR

### Fatias

O trabalho segue `docs/roadmap.md`. **Nunca comece a Fatia N+1 antes da Fatia N estar pronta**:
mergeada com CI verde, versionada, rodando em `docker compose up` e em uso.

### Branches, commits, PRs e migrations

Tudo em `docs/contribuindo.md`. O essencial:

- Uma branch por atividade (`feat/`, `fix/`, `refactor/`, `test/`, `docs/`, `chore/`)
- Conventional Commits em português; o título do PR também
- Squash merge; nenhum push direto na `main`
- Migration no mesmo PR da feature; migration mesclada **nunca** é editada

## 4.1 Modo de aprendizado

O autor quer aprender construindo. Cada passo do plano é marcado com um de três modos:

| Modo | Quem escreve | Papel do Claude |
|---|---|---|
| **Manual** | O autor | Explicar o conceito e o porquê, indicar o comando ou a API a pesquisar, revisar depois. **Não escrever o código nem rodar o comando** |
| **Par** | O autor, guiado | Mostrar o trecho em partes pequenas, explicando cada uma, para o autor digitar e adaptar |
| **Delegado** | O Claude | Implementar e, ao final, explicar em poucas linhas o que foi feito e o que vale ler no código |

Regras:
- Na dúvida, pergunte o modo antes de começar o passo.
- Ao revisar código do autor, aponte problemas e explique, mas não reescreva o arquivo inteiro: sugira a mudança e deixe o autor aplicar.
- Prefira perguntas que levem o autor à resposta ("o que acontece com o filtro de `OwnerId` se...?") a respostas prontas, quando o modo for Manual.

---

## 5. O que nunca fazer

- ❌ Código de produção antes do teste
- ❌ `Domain` ou `Application` referenciando EF Core, ASP.NET ou qualquer SDK
- ❌ `DateTime.Now` / `DateTime.UtcNow` direto
- ❌ Materializar ocorrências de recorrência no banco
- ❌ Cor, espaçamento ou fonte literal em componente
- ❌ Tipos da API escritos à mão no front
- ❌ Classe terminada em `Manager`, `Helper`, `Util`, `Processor` ou `Service` genérico
- ❌ `OwnerId` vindo da requisição
- ❌ Editar migration já mesclada
- ❌ Teste de integração contra SQLite ou InMemory — use Testcontainers
- ❌ `any` ou `@ts-ignore` no TypeScript
- ❌ Código comentado "para depois"
- ❌ Commitar segredo, connection string real ou `.env`
- ❌ Adicionar dependência sem perguntar
- ❌ Sugerir hospedagem, autenticação ou notificação externa antes do pós-v1

---

## 6. Ambiente

| Item | Valor |
|---|---|
| Sistema | Windows 11 nativo |
| Terminal | PowerShell 7 no Windows Terminal |
| Editor | VS Code |
| Raiz do repositório | `D:\Renmember.io` (pasta do clone; qualquer caminho curto serve) |
| Docker | Docker Desktop (aberto antes de rodar testes de integração) |

- Comandos em **PowerShell**: `$env:NOME`, caminhos com `\`, scripts `.ps1`
- Sem `export`, `chmod`, `sudo`, `rm -rf`, `cat`, `ls`
- Fim de linha **LF**, forçado pelo `.gitattributes`

## 7. Comandos

> Funcionam a partir da Fatia 0.

```powershell
# Stack completa
docker compose -f infra\docker\docker-compose.yml up

# Backend
dotnet test api\
dotnet test api\tests\Renmember.Domain.UnitTests
dotnet format api\ --verify-no-changes

# Nova migration
dotnet ef migrations add NomeDaMudanca `
  --project api\src\Renmember.Infrastructure `
  --startup-project api\src\Renmember.Api

# Frontend
Set-Location web
pnpm dev
pnpm test
pnpm test:e2e
pnpm lint
pnpm typecheck
pnpm generate:api   # regenera o cliente a partir do OpenAPI
Set-Location ..
```

---

## 8. Mapa dos documentos

| Preciso de… | Leia |
|---|---|
| Visão geral, arquitetura, decisões | `docs/blueprint.md` |
| O que fazer agora | `docs/roadmap.md` |
| Como nomear as coisas | `docs/dominio.md` |
| Branch, commit, PR, CI, versão, migration | `docs/contribuindo.md` |
| Setup do zero: Windows, ferramentas, GitHub | `docs/primeiros-passos.md` |
| Por que uma decisão foi tomada | `docs/adr/` |
| Cores, espaçamentos, componentes | `docs/design-system/` |

## 9. Estado atual

**Fatia atual: 0 — Fundação.** Nenhum código existe ainda; só a documentação.

Decisões abertas (ver blueprint, seção 9), a resolver até a Fatia 2:
**D2** recorrentes no kanban · **D3** colunas do kanban fixas ou personalizáveis.
