# 0016 — Gerar o cliente da API com openapi-typescript e openapi-fetch

- **Status:** Aceita
- **Data:** 2026-10-04
- **Fatia:** 0

## Contexto

A regra do projeto é nunca escrever tipos da API à mão no front: o cliente TypeScript é gerado do
documento OpenAPI publicado pela API, e o job `contract` do CI reprova o PR se o cliente
commitado divergir da API. O job existe desde a Fatia 0, então a ferramenta precisa ser escolhida
agora, embora o primeiro uso real seja na Fatia 1.

## Decisão

- **openapi-typescript** gera, a partir do `openapi.json`, um único arquivo `.d.ts` com os tipos
  de todas as rotas, corpos e respostas
- **openapi-fetch** faz as chamadas tipadas com esses tipos (`client.GET("/health")`), sem código
  gerado em tempo de execução
- O script `pnpm generate:api` regenera os tipos; o arquivo gerado é commitado e comparado pelo
  job `contract`
- Os hooks do TanStack Query são escritos no projeto, por feature, chamando o openapi-fetch

## Alternativas consideradas

| Alternativa | Por que não |
|---|---|
| `orval` | Gera hooks do TanStack Query prontos, mas acopla o formato dos hooks ao gerador e produz muito código para revisar a cada mudança da API |
| `@hey-api/openapi-ts` | Ainda em versão `0.x`, com mudanças incompatíveis frequentes |
| Tipos escritos à mão | Proibido pelo `CLAUDE.md`: front e back divergiriam em silêncio |

## Consequências

- **Positivas:** saída gerada pequena e legível (só tipos), diff do contrato fácil de revisar no
  PR, cliente de poucos KB, controle total sobre os hooks do TanStack Query.
- **Negativas e riscos:** os hooks de consulta e mutação são escritos à mão (com os tipos
  gerados); duas dependências em vez de uma.
- **O que muda:** dependências `openapi-typescript` (desenvolvimento) e `openapi-fetch` em
  `web/package.json`; script `generate:api`; job `contract` no CI.
