# 0017 — Semear o usuário do v1 com UseAsyncSeeding

- **Status:** Aceita
- **Data:** 2026-10-04
- **Fatia:** 0

## Contexto

O v1 tem um único usuário, sem login, com `Id` (Guid v7), `Name` e `TimeZone` (IANA, validado no
domínio). O fuso precisa ser configurável pelo `.env` (`USER_TIMEZONE`).

A regra original do `contribuindo.md` mandava semear dados fixos com `HasData`. Mas o `HasData`
grava os valores **dentro da migration** no momento em que ela é gerada: um fuso lido do ambiente
ficaria congelado com o valor da máquina de quem gerou a migration, e o modelo mudaria a cada
ambiente, quebrando o `has-pending-model-changes` do CI. Além disso, o `HasData` contorna a
fábrica do domínio (`User.Criar`) e a validação do fuso.

## Decisão

- O usuário é semeado em tempo de execução com **`UseAsyncSeeding`** do EF Core, chamado pelo
  `MigrateAsync` no startup da API em desenvolvimento
- O seed é idempotente: só insere se não existir usuário com o `Id` configurado
- O usuário é criado por `User.Criar`, passando pelas validações do domínio
- Os valores vêm da seção `SeededUser` da configuração:
  - `Id`: Guid v7 fixo em `appsettings.json` (o mesmo em todas as máquinas)
  - `Name`: `appsettings.json`
  - `TimeZone`: `appsettings.json`, sobrescrito por `USER_TIMEZONE` no Docker Compose
- `ICurrentUser` (implementação `SeededCurrentUser`) devolve o mesmo `Id` configurado. Nenhum
  outro código conhece esse valor
- `HasData` continua sendo a forma de semear **dados de referência fixos** que não dependem de
  ambiente

## Alternativas consideradas

| Alternativa | Por que não |
|---|---|
| `HasData` com o fuso lido do ambiente | Valor congelado na migration; modelo diferente por máquina |
| `HasData` com fuso fixo e `.env` só para conferência | `USER_TIMEZONE` deixaria de ter efeito; contorna a validação do domínio |
| Script SQL separado | Fora do fluxo do EF; fácil de esquecer de rodar |
| Seed num `IHostedService` próprio | Mais código para o mesmo efeito; o EF já oferece o gancho |

## Consequências

- **Positivas:** o domínio valida o usuário semeado; o fuso é configurável sem gerar migration;
  migrations ficam só com esquema.
- **Negativas e riscos:** o seed só roda pelo `MigrateAsync`/`EnsureCreatedAsync` assíncronos. O
  `dotnet ef database update` (síncrono) aplica o esquema mas **não** semeia; o usuário aparece no
  próximo startup da API. Mudar o `TimeZone` depois do primeiro seed não altera o usuário já
  gravado.
- **O que muda:** `docs/contribuindo.md` §7 passa a distinguir dados de referência (`HasData`) de
  dados dependentes de ambiente (`UseAsyncSeeding`).
