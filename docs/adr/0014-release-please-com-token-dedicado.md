# 0014 — Rodar o release-please com um token dedicado

- **Status:** Aceita
- **Data:** 2026-10-04
- **Fatia:** 0

## Contexto

O release-please abre e atualiza o PR de release a cada merge na `main`. A `main` exige o check
`ci` verde para mesclar.

Eventos criados com o `GITHUB_TOKEN` padrão do GitHub Actions **não disparam outros workflows**
(proteção contra execução recursiva). Um PR de release aberto com esse token nunca roda o CI, o
check `ci` nunca reporta e o ruleset bloqueia o merge para sempre.

## Decisão

O release-please roda com um **Personal Access Token fine-grained**, guardado no secret
`RELEASE_PLEASE_TOKEN`:

- Acesso restrito ao repositório `GabrielPalheiro/Renmember.io`
- Permissões: **Contents** e **Pull requests** em leitura e escrita (Metadata em leitura é
  automática)
- Validade de no máximo um ano, com renovação anotada na agenda

Versionamento:

- Manifesto (`.release-please-manifest.json`) começando em `0.0.0`
- `bump-minor-pre-major: true`: durante `0.x`, mudança incompatível sobe minor
- A primeira versão gerada é a **`v0.1.0`**, no fim da Fatia 0

O passo a passo de criação do token e do secret está em `docs/primeiros-passos.md`, seção 3.5.

## Alternativas consideradas

| Alternativa | Por que não |
|---|---|
| `GITHUB_TOKEN` padrão | O PR de release não dispara o CI e fica bloqueado pelo ruleset |
| Autor na lista de bypass do ruleset | Abre exceção justamente no PR que gera a versão; o CI deixaria de validar o release |
| GitHub App próprio | Token de curta duração e sem dono humano, mas exige criar e manter um app para um projeto de um usuário só. Revisitar se o projeto tiver mais mantenedores |

## Consequências

- **Positivas:** o PR de release passa pelo mesmo CI que qualquer outro PR; nenhuma exceção no
  ruleset.
- **Negativas e riscos:** o token expira e precisa ser renovado, senão o release-please falha
  (falha visível no Actions, sem efeito na `main`). Os commits e PRs de release aparecem em nome
  do autor.
- **O que muda:** o workflow do release-please usa `token: ${{ secrets.RELEASE_PLEASE_TOKEN }}`.
  A configuração "Allow GitHub Actions to create and approve pull requests" **não** é necessária
  e fica desligada, porque o `GITHUB_TOKEN` não precisa criar PRs.
