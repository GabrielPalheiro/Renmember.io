# 0013 — Usar AwesomeAssertions no lugar de FluentAssertions

- **Status:** Aceita
- **Data:** 2026-10-04
- **Fatia:** 0

## Contexto

A stack original listava FluentAssertions para as asserções dos testes do backend. A partir da
versão 8, o FluentAssertions passou a ter licença comercial (Xceed): gratuito apenas para uso não
comercial. O objetivo 3 do projeto é virar um produto, então depender de uma licença que pode
exigir pagamento quando isso acontecer é um risco que não precisa existir.

## Decisão

Usar **AwesomeAssertions**, fork comunitário do FluentAssertions com licença Apache 2.0 e API
praticamente idêntica (`resultado.Should().Be(...)`).

## Alternativas consideradas

| Alternativa | Por que não |
|---|---|
| FluentAssertions 8+ | Licença comercial para uso comercial |
| FluentAssertions 7.x fixado | Apache 2.0, mas sem evolução nem correções futuras |
| Shouldly | Licença livre, mas API diferente da que o autor já conhece; ganho nenhum que justifique |
| Só `Assert` do xUnit | Mensagens de falha menos legíveis em asserções de coleção e de objeto |

## Consequências

- **Positivas:** licença compatível com produto comercial; quem conhece FluentAssertions lê os
  testes sem esforço.
- **Negativas e riscos:** projeto comunitário mais novo, com comunidade menor. Se for abandonado, a
  migração para outro pacote é mecânica, porque a API é a mesma.
- **O que muda:** `CLAUDE.md` e blueprint passam a citar AwesomeAssertions. Os projetos de teste
  referenciam o pacote `AwesomeAssertions`.
