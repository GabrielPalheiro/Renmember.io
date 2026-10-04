# 0015 — Criar cada projeto de teste junto com o seu primeiro teste

- **Status:** Aceita
- **Data:** 2026-10-04
- **Fatia:** 0

## Contexto

O blueprint prevê quatro projetos de teste. Na Fatia 0, o `Renmember.Application` ainda não tem
nenhum caso de uso, então o `Renmember.Application.UnitTests` ficaria vazio.

No .NET 10, o `dotnet test` usa o Microsoft.Testing.Platform, que **falha quando nenhum teste é
executado** (código de saída 8). Restavam três caminhos: um teste de enfeite, um CI configurado
para aceitar zero testes, ou não criar o projeto ainda.

## Decisão

**Um projeto de teste só é criado junto com o seu primeiro teste real.** O CI **não** aceita
execução com zero testes.

Na Fatia 0 existem três projetos de teste: `Renmember.Domain.UnitTests`,
`Renmember.Api.IntegrationTests` e `Renmember.ArchitectureTests`. O
`Renmember.Application.UnitTests` nasce na Fatia 1, com o primeiro caso de uso.

## Alternativas consideradas

| Alternativa | Por que não |
|---|---|
| Teste de enfeite no projeto vazio | Código morto que passa a impressão de cobertura inexistente |
| CI aceitando zero testes (`--ignore-exit-code 8`) | Mascara o defeito real de um projeto que perdeu seus testes ou de um filtro errado |
| Projeto vazio fora da solução | A estrutura deixaria de refletir o que de fato é compilado e testado |

## Consequências

- **Positivas:** todo projeto de teste presente na solução executa pelo menos um teste; zero
  testes executados sempre é sinal de problema.
- **Negativas e riscos:** a árvore de `api/tests/` da Fatia 0 difere da árvore final do
  blueprint até a Fatia 1.
- **O que muda:** roadmap e blueprint registram o momento de criação do
  `Renmember.Application.UnitTests`.
