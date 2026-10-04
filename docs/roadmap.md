# Roadmap — Renmember.io

## A regra que governa este roadmap

O ritmo de trabalho é **irregular**. Por isso o roadmap não tem datas: tem **fatias verticais**,
e cada uma termina com algo **utilizável** no dia a dia.

> **Se o projeto parar depois de qualquer fatia, o que existe continua funcionando e apresentável.**

Como o v1 é local, "pronto" significa:

1. Mergeado na `main` por PR, com **CI verde**
2. Versão gerada pelo release-please (tag + `CHANGELOG.md`)
3. Rodando em `docker compose up` numa máquina limpa
4. Usado de verdade pelo autor

**Não se começa a Fatia N+1 antes da Fatia N cumprir os quatro itens.**

Unidade de estimativa: **sessão** ≈ 3 horas de trabalho concentrado.

---

## Fatia 0 — Fundação · ~3 sessões
**Entregável: `docker compose up` sobe tudo e o CI barra código quebrado**

- [ ] Monorepo: `api/`, `web/`, `docs/`, `infra/`, `.github/`
- [ ] `.slnx` com os 4 projetos + 3 projetos de teste (o de `Application` nasce na Fatia 1, ADR 0015)
- [ ] `web/package.json` com pnpm fixado em `packageManager` (sem workspace na raiz)
- [ ] `docker compose up` sobe Postgres + API + Web
- [ ] API com `/health`, OpenAPI e Scalar
- [ ] Web com shell da aplicação (navegação entre Lista, Kanban, Calendário), usando as
      variáveis padrão do shadcn/ui como tokens provisórios
- [ ] Primeira migration: tabela `Users` + usuário semeado; `ICurrentUser` resolvendo esse usuário
- [ ] Teste de integração com Testcontainers rodando (smoke da API contra Postgres real)
- [ ] Teste de arquitetura (NetArchTest) verificando a regra de dependência
- [ ] Workflow de CI com todos os jobs de `docs/contribuindo.md`, mesmo que alguns ainda com pouco teste
- [ ] release-please configurado; primeira versão `v0.1.0` gerada
- [ ] Ruleset na `main`: PR obrigatório, check `ci` exigido, branch atualizada, squash only
- [ ] Template de PR e de issue

**Pronto quando:** um PR com um teste quebrado é **reprovado** pelo CI.

---

## Fatia 1 — Cadastro de atividades · ~4 sessões
**Entregável: as responsabilidades saem da cabeça e entram no sistema**

- [ ] `Task` com título, descrição, status, agendamento (sem data, dia inteiro ou com horário)
- [ ] `Reference` (link e nota) e `Category` com cor de token
- [ ] Global query filter por `OwnerId` + teste de isolamento entre dois usuários
- [ ] API: criar, editar, concluir, excluir, listar com filtros (status, categoria, período)
- [ ] `Renmember.Application.UnitTests` criado junto com o primeiro teste de caso de uso
- [ ] Erros em `ProblemDetails`; validação com FluentValidation
- [ ] Cliente TypeScript gerado do OpenAPI
- [ ] Tela `/atividades`: lista, filtros, formulário (React Hook Form + Zod) em painel lateral
- [ ] Tokens do design system documentados em `docs/design-system/`, substituindo os provisórios
- [ ] Estados vazios, de carregamento e de erro desenhados
- [ ] E2E: criar, editar e concluir uma atividade

**Pronto quando:** você cadastra as atividades reais da sua semana.

---

## Fatia 2 — Kanban · ~4 sessões
**Entregável: ver o que está parado e o que está andando**

- [ ] Decisão D2 registrada (recorrentes no kanban) — mesmo que a recorrência só chegue na Fatia 4
- [ ] `Position` com indexação fracionária + testes de borda
- [ ] Endpoint de mover: muda status e/ou posição numa operação só
- [ ] Tela `/kanban` com dnd-kit: arrastar entre colunas e reordenar
- [ ] Atualização otimista com rollback se a API falhar
- [ ] Arrastar por teclado e anúncio para leitor de tela
- [ ] E2E: mover um cartão e conferir que a ordem persiste após recarregar

**Pronto quando:** você usa o kanban no lugar de lembrar de cabeça.

---

## Fatia 3 — Calendário mensal e semanal · ~4 sessões
**Entregável: a semana e o mês inteiros numa tela**

- [ ] Licença dos plugins do FullCalendar conferida e registrada em ADR
- [ ] Endpoint de calendário: atividades de um intervalo (`de`, `ate`)
- [ ] Tela `/calendario` com alternância mês ↔ semana e navegação entre períodos
- [ ] Dia inteiro e com horário exibidos de forma distinta; cor pela categoria
- [ ] Clicar num dia vazio cria atividade naquela data
- [ ] Arrastar uma atividade reagenda
- [ ] FullCalendar estilizado só com tokens do design system (claro e escuro)
- [ ] E2E: reagendar arrastando e conferir no kanban/lista

**Pronto quando:** você planeja a semana olhando o calendário.

---

## Fatia 4 — Recorrência · ~5 sessões
**Entregável: rotinas cadastradas uma vez só**

- [ ] `RecurrenceRule` no domínio, construída por TDD com tabela de casos:
      intervalos, dias da semana, dia 31, 29/02, término por data e por contagem
- [ ] Cálculo de ocorrências por intervalo, no fuso do usuário
- [ ] `OccurrenceException`: concluir, pular e remarcar uma ocorrência
- [ ] Editar "só esta" vs. "esta e as próximas" vs. "toda a série"
- [ ] Calendário e kanban exibindo ocorrências conforme a decisão D2
- [ ] Formulário de recorrência com resumo em linguagem natural ("A cada 2 semanas, seg e qua")

**Pronto quando:** suas contas mensais e rotinas diárias estão cadastradas como recorrência.

---

## Fatia 5 — Lembretes in-app · ~4 sessões
**Entregável: o sistema avisa antes de você esquecer**

- [ ] `Reminder` por antecedência; validação de que a atividade tem agendamento
- [ ] Cálculo de `ReminderAlert` vencidos, incluindo ocorrências recorrentes
- [ ] `GET /reminders/due`, dispensar e adiar
- [ ] Polling de 1 minuto no front; toast + contador na central de lembretes
- [ ] Notification API do navegador, com pedido de permissão explicado ao usuário
- [ ] Testes com `FakeTimeProvider` cobrindo vencimento, adiamento e recorrência
- [ ] ADR do polling e do caminho para e-mail/push

**Pronto quando:** um lembrete aparece sozinho e você age por causa dele.

---

## Fatia 6 — Acabamento · ~3 sessões
**Entregável: o projeto se apresenta sozinho**

- [ ] PWA instalável (manifest, ícones, service worker para o shell)
- [ ] Dark mode completo
- [ ] Atalhos de teclado (`N` nova atividade, `K` kanban, `C` calendário)
- [ ] Auditoria de acessibilidade (axe no Playwright) sem violações sérias
- [ ] README exemplar: problema, screenshots, arquitetura, como rodar
- [ ] ADRs completos em `docs/adr/`
- [ ] Diagramas C4 (contexto e container) em Mermaid

**Pronto quando:** alguém clona o repositório, roda `docker compose up` e entende o projeto sem você explicar.

---

## Caminho para online — pós-v1

Em ordem sugerida, cada item vira uma fatia quando chegar a hora:

1. **Autenticação**: ASP.NET Core Identity + JWT em cookie `httpOnly`; `ICurrentUser` passa a ler o claim
2. **Hospedagem**: Vercel (web) + Azure Container Apps (API) + Neon (banco) — reavaliar preços na época
3. **CD**: deploy no merge ou na tag; *migration bundle* como passo do deploy
4. **Lembretes fora do app**: agendador no servidor + Web Push e/ou e-mail (Resend)
5. **App**: empacotar o PWA com Capacitor

## Backlog — não priorizado

Anexos na referência · Subtarefas/checklist · Busca textual · Visão diária/agenda ·
Estatísticas de produtividade · Exportar/importar iCal · Colunas do kanban personalizáveis ·
Sincronização com Google Calendar

**Regra:** ideia nova entra aqui, nunca na fatia em andamento.
