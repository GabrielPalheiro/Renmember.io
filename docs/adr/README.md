# Registros de decisão arquitetural (ADR)

Cada decisão que muda a stack, a arquitetura ou o processo vira um arquivo aqui, a partir de
[`0000-template.md`](0000-template.md).

- Nome do arquivo: `NNNN-titulo-em-portugues-com-hifen.md`, numeração sequencial
- Um ADR aceito **não é editado** para mudar a decisão: um novo ADR o substitui e o antigo
  passa a `Substituída por NNNN`
- Todo ADR novo entra nesta tabela e na seção 8 do [blueprint](../blueprint.md)

As decisões 001 a 012 estão resumidas no blueprint e ganham arquivo próprio na Fatia 6.

| # | Decisão | Status |
|---|---|---|
| [0013](0013-usar-awesomeassertions.md) | Usar AwesomeAssertions no lugar de FluentAssertions | Aceita |
| [0014](0014-release-please-com-token-dedicado.md) | Rodar o release-please com um token dedicado | Aceita |
| [0015](0015-projeto-de-teste-nasce-com-o-primeiro-teste.md) | Criar cada projeto de teste junto com o seu primeiro teste | Aceita |
| [0016](0016-gerar-cliente-com-openapi-typescript.md) | Gerar o cliente da API com openapi-typescript e openapi-fetch | Aceita |
