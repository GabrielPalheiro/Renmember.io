# Contribuindo — Renmember.io

Como uma mudança sai da sua cabeça e chega na `main`: branch, commit, PR, CI, versão e migration.

---

## 1. O ciclo de uma atividade

```powershell
# 1. Partir sempre da main atualizada
git switch main
git pull

# 2. Branch da atividade
git switch -c feat/kanban-arrastar

# 3. Trabalhar em ciclos de TDD, commitando a cada ideia completa

# 4. Enviar e abrir o PR
git push -u origin feat/kanban-arrastar
gh pr create --fill

# 5. Acompanhar o CI
gh pr checks --watch

# 6. Mesclar (squash) e apagar a branch
gh pr merge --squash --delete-branch
```

---

## 2. Branches

Modelo **trunk-based**: a `main` é a única branch longa e está sempre verde.
Toda atividade nasce numa branch curta — idealmente mesclada em até poucos dias.

| Prefixo | Uso | Exemplo |
|---|---|---|
| `feat/` | Funcionalidade nova | `feat/recorrencia-semanal` |
| `fix/` | Correção de defeito | `fix/lembrete-fuso` |
| `refactor/` | Mudança interna sem alterar comportamento | `refactor/extrair-schedule` |
| `test/` | Só testes | `test/borda-dia-31` |
| `docs/` | Só documentação | `docs/adr-polling` |
| `chore/` | CI, dependências, configuração | `chore/cache-pnpm` |

Nome em português, minúsculo, com hífen, sem acento.

---

## 3. Commits

[Conventional Commits](https://www.conventionalcommits.org/) em português. O tipo decide a versão:

| Commit | Efeito na versão |
|---|---|
| `fix(escopo): ...` | patch — `0.4.1` → `0.4.2` |
| `feat(escopo): ...` | minor — `0.4.1` → `0.5.0` |
| `feat(escopo)!: ...` ou rodapé `BREAKING CHANGE:` | major |
| `refactor`, `test`, `docs`, `chore`, `ci` | não gera versão |

Escopos válidos: `tasks`, `kanban`, `calendar`, `recurrence`, `reminders`, `categories`,
`domain`, `api`, `web`, `db`, `infra`, `ci`, `deps`, `docs`.

| Escopo | Para |
|---|---|
| `infra` | Docker, Docker Compose e demais arquivos de `infra/` |
| `deps` | Atualização de dependências (usado pelo Dependabot) |

```
feat(kanban): permitir reordenar atividades por teclado
fix(recurrence): cair no ultimo dia do mes quando o dia 31 nao existe
test(reminders): cobrir adiamento de alerta recorrente
chore(ci): cachear pacotes do pnpm
chore(deps): atualizar npgsql para 10.0.4
```

Um commit = uma ideia. Como o merge é **squash**, o título do PR vira o commit na `main` —
ele também precisa seguir o formato. O CI valida isso.

---

## 4. Pull requests

### Regras da `main`

| Regra | Como |
|---|---|
| Nenhum push direto | Ruleset do GitHub na `main` |
| CI verde obrigatório | Check `ci` exigido |
| Branch atualizada com a `main` | Exigido antes do merge |
| Um jeito só de mesclar | Só squash habilitado |
| Branch apagada após merge | Automático |

O repositório é público ([`GabrielPalheiro/Renmember.io`](https://github.com/GabrielPalheiro/Renmember.io)),
então o ruleset é aplicado pelo GitHub. A configuração passo a passo está em `docs/primeiros-passos.md`.

### Checklist do template de PR

- [ ] Título no formato Conventional Commits
- [ ] Testes escritos **antes** da implementação
- [ ] Migration incluída, se o modelo mudou — e o script SQL do artefato revisado
- [ ] Nomes conferidos com `docs/dominio.md`
- [ ] Documento afetado atualizado no mesmo PR
- [ ] Testado manualmente em `docker compose up`

---

## 5. O CI

Arquivo: `.github/workflows/ci.yml`. Roda em todo PR para `main` e em todo push na `main`.

```
            ┌─ api-quality ──┐
            ├─ api-tests ────┤
PR ────────►├─ db-migrations ┼──► e2e ──► ci  (check exigido)
            ├─ web-quality ──┤
            ├─ web-tests ────┤
            └─ contract ─────┘
```

| Job | Comandos principais | Reprova se |
|---|---|---|
| `api-quality` | `dotnet format --verify-no-changes`, `dotnet build -warnaserror` | Formatação ou warning |
| `api-tests` | `dotnet test` com cobertura | Teste falha; cobertura do `Domain` < 90%; violação de arquitetura |
| `db-migrations` | Aplica todas as migrations num Postgres vazio; `dotnet ef migrations has-pending-model-changes`; gera script idempotente | Migration falha; modelo mudou sem migration |
| `web-quality` | `pnpm lint`, `pnpm typecheck`, `pnpm format:check` | Erro de lint, de tipo ou de formatação |
| `web-tests` | `pnpm test` | Teste falha |
| `contract` | Regenera o cliente TypeScript do OpenAPI e compara | Cliente commitado divergiu da API |
| `e2e` | Sobe a stack com `docker compose`, roda `pnpm test:e2e` | Fluxo E2E falha |
| `ci` | Agrega os anteriores | Qualquer job acima falhou |
| `pr-title` | Valida o título do PR | Título fora do Conventional Commits |

**Velocidade do CI:**

- Cache de NuGet e do store do pnpm
- Jobs independentes em paralelo
- Filtros de caminho: mudança só em `docs/` não roda testes
- `concurrency` cancela a execução anterior quando chega um push novo no mesmo PR

---

## 6. Versionamento e releases

**release-please** cuida de tudo. Você nunca edita número de versão à mão.

1. A cada merge na `main`, o release-please atualiza um **PR de release** aberto, acumulando
   os `feat` e `fix` no `CHANGELOG.md`
2. Quando você quiser fechar uma versão, mescla o PR de release
3. Ele cria a tag (`v0.5.0`), a GitHub Release com as notas e atualiza o `CHANGELOG.md`

O release-please roda com o token do secret `RELEASE_PLEASE_TOKEN`, para que o PR de release
dispare o CI como qualquer outro ([ADR 0014](adr/0014-release-please-com-token-dedicado.md)).
O manifesto começa em `0.0.0` e a primeira versão é a `v0.1.0`.

Regra prática: **feche uma versão ao final de cada fatia**. A versão `v1.0.0` é o fim da Fatia 6.

Durante `0.x`, `feat!` sobe minor (configuração `bump-minor-pre-major`) — mudança incompatível
é esperada antes da `1.0`.

---

## 7. Migrations

### Criar

```powershell
dotnet ef migrations add AdicionarRecorrencia `
  --project api\src\Renmember.Infrastructure `
  --startup-project api\src\Renmember.Api
```

Nome em português, no imperativo, descrevendo a mudança: `AdicionarRecorrencia`,
`CriarTabelaDeLembretes`, `RemoverColunaPrioridade`.

### Regras

| Regra | Por quê |
|---|---|
| Migration vai no **mesmo PR** da feature | Código e esquema sempre na mesma versão |
| Revise o script SQL publicado como artefato do PR | O SQL é o que roda de verdade, não o C# |
| **Nunca edite** migration já mesclada na `main` | Ela já foi aplicada em algum banco; corrija com uma nova |
| Mudança destrutiva em duas etapas (**expand → contract**) | A versão anterior continua funcionando durante a transição |
| Seeds de dados fixos via `HasData`; dados de exemplo por script separado | Seed de exemplo não pode vazar para produção futura |

### Expand → contract, na prática

Renomear `Notes` para `ReferenceNote`:

1. **PR 1 (expand):** cria `ReferenceNote`, copia os dados, o código passa a escrever nas duas e ler da nova
2. **PR 2 (contract), numa release seguinte:** remove `Notes` e a escrita dupla

No v1 local isso parece excesso. É treino para quando houver banco de produção — e é exatamente
o que se pergunta em entrevista.

### Aplicar

| Ambiente | Como |
|---|---|
| Desenvolvimento | Automático no startup da API |
| CI | Job `db-migrations` + Testcontainers nos testes de integração |
| Futuro (online) | *Migration bundle* (`efbundle`) como passo do deploy, nunca no startup |

---

## 8. Quando algo der errado

| Situação | O que fazer |
|---|---|
| CI vermelho no PR | Corrija na mesma branch e dê push; o CI roda de novo |
| Bug descoberto na `main` | Branch `fix/`, teste que reproduz o bug primeiro, depois a correção |
| Migration errada já mesclada | Nova migration corrigindo — nunca editar a antiga |
| Segredo commitado | Rotacione a credencial **primeiro**, limpe o histórico depois |
