# Linguagem ubíqua — Renmember.io

> **Um conceito tem um nome só.** O mesmo nome no código, na tabela, na rota da API, na tela e
> na conversa. Consulte este glossário **antes** de nomear qualquer coisa.
>
> Conceito novo só nasce depois de ser adicionado aqui.

## Convenção de idioma

| O quê | Idioma | Exemplo |
|---|---|---|
| Nome do conceito (classe, tabela, rota) | Inglês | `Task`, `RecurrenceRule`, `Reminder` |
| Comportamento e regra (método, variável) | Português | `CalcularOcorrencias()`, `estaVencido` |
| Documentação, commit, ADR, nome de teste | Português | `feat(tasks): adicionar referencia por link` |
| Texto visível ao usuário | Português (pt-BR) | "Nova atividade" |

---

## Atores

| Termo | Definição | **Não** chamar de |
|---|---|---|
| **Usuário** (`User`) | Dono das atividades. No v1 existe um só; o modelo já suporta vários | Cliente, Conta, Owner (como classe) |

| Campo | Regra |
|---|---|
| `Id` | `Guid` versão 7 |
| `Name` | Obrigatório, até 100 caracteres |
| `TimeZone` | Identificador **IANA** (ex.: `America/Sao_Paulo`), validado no domínio. É o fuso do usuário |

> `OwnerId` é o **nome da coluna** que liga uma entidade ao `User`. O conceito continua sendo `User`.

---

## Conceitos centrais

### Task — Atividade
Algo que o usuário precisa fazer. Tem título, descrição opcional, status, posição no kanban,
agendamento opcional, categoria opcional, referência opcional, recorrência opcional e lembretes.

> ❌ Não chame de `Todo`, `Item`, `Card`, `Job`, `Activity` nem `Tarefa` no código.
> "Cartão" é como a atividade **aparece** no kanban, não o que ela **é**.
> Atenção: `Task` colide com `System.Threading.Tasks.Task` no C#. Use o alias
> `using TaskEntity = Renmember.Domain.Tasks.Task;` só onde houver conflito, nunca renomeie o conceito.

### TaskStatus — Status
A etapa em que a atividade está. Define a coluna do kanban.

| Valor | Rótulo na tela |
|---|---|
| `ToDo` | A fazer |
| `Doing` | Fazendo |
| `Done` | Feito |

> Colunas personalizáveis são decisão aberta (D3 no blueprint). Até lá, os três valores acima.

### Position — Posição
Ordem da atividade dentro da sua coluna, em **indexação fracionária** (texto ordenável).
Mover uma atividade altera só a posição dela.

### Schedule — Agendamento
Quando a atividade acontece. Três formas, mutuamente exclusivas:

| Forma | Exemplo | Tipo |
|---|---|---|
| Sem data | "Ler o livro X" | — |
| Dia inteiro | "Pagar o IPTU no dia 10" | `DateOnly` |
| Com horário | "Consulta às 14h30, 1 hora" | Instante UTC + duração opcional |

Atividade **sem data** aparece no kanban, mas não no calendário.

### Reference — Referência
Material de apoio para executar a atividade: um **link** (URL) e/ou uma **nota** em texto.
Anexos de arquivo ficam fora do v1.

> ❌ Não chame de `Attachment`, `Link` (sozinho) nem `Note` (sozinho). A referência pode ter os dois.

### Category — Categoria
Etiqueta escolhida pelo usuário para agrupar atividades: Casa, Saúde, Finanças, Trabalho.
Tem nome e uma cor **do conjunto de tokens** do design system — nunca cor livre.

> ❌ Não chame de `Tag`, `Label`, `Group` nem `Project`.

### RecurrenceRule — Recorrência
Regra que faz uma atividade se repetir. Composição:

| Campo | Valores |
|---|---|
| `Frequency` | `Daily` · `Weekly` · `Monthly` · `Yearly` |
| `Interval` | A cada N (ex.: a cada 2 semanas) |
| `DaysOfWeek` | Só para `Weekly` (ex.: seg, qua, sex) |
| Término | Nunca · em uma data · após N ocorrências |

**Regra do dia inexistente:** recorrência mensal no dia 31 cai no **último dia** dos meses
mais curtos. Recorrência anual em 29/02 cai em 28/02 nos anos não bissextos.

### Occurrence — Ocorrência
Uma instância de uma atividade recorrente numa data específica. **Não é persistida**: é calculada
a partir da `RecurrenceRule` para o intervalo pedido.

> Uma atividade **não recorrente** tem exatamente uma ocorrência implícita: ela mesma.

### OccurrenceException — Exceção de ocorrência
O único registro persistido sobre uma ocorrência: quando ela **diverge** da regra.

| Tipo | Significado |
|---|---|
| `Completed` | Esta ocorrência foi feita |
| `Skipped` | Esta ocorrência não vai acontecer |
| `Rescheduled` | Esta ocorrência foi movida para outra data/hora |

### Reminder — Lembrete
Aviso configurado numa atividade, definido por **antecedência** em relação ao agendamento
(ex.: 15 minutos antes, 1 dia antes). Atividade sem data não pode ter lembrete.

### ReminderAlert — Alerta
O lembrete **disparado** para uma ocorrência específica — o que aparece na tela.

| Estado | Significado |
|---|---|
| `Due` | Venceu e está visível |
| `Snoozed` | Adiado pelo usuário até um novo instante |
| `Dismissed` | Dispensado. Não aparece mais |

> `Reminder` é a configuração ("avise 1 dia antes"). `ReminderAlert` é o acontecimento
> ("aviso de hoje, 09h, da conta de luz"). Não misture os dois.

---

## Vocabulário de tempo

| Termo | Definição |
|---|---|
| **Data** | Data de calendário, sem hora e sem fuso. `DateOnly` |
| **Instante** | Momento absoluto, armazenado em UTC. `DateTimeOffset` / `timestamptz` |
| **Fuso do usuário** | `America/Sao_Paulo` no v1. Recorrências são calculadas nele |
| **Relógio** | `TimeProvider`, sempre injetado. Nunca `DateTime.Now` |

---

## Visões

As visões **não são entidades** — são formas de ler as mesmas atividades.

| Visão | Lê | Rota no front |
|---|---|---|
| Lista | Todas as atividades, com filtros | `/atividades` |
| Kanban | `Status` + `Position` | `/kanban` |
| Calendário mensal | `Schedule` + ocorrências do mês | `/calendario?modo=mes` |
| Calendário semanal | `Schedule` + ocorrências da semana | `/calendario?modo=semana` |

---

## Termos proibidos

| Proibido | Use |
|---|---|
| `Todo`, `Item`, `Card`, `Job`, `Activity` | `Task` |
| `Tag`, `Label`, `Group`, `Project` | `Category` |
| `Attachment`, `Link`/`Note` isolados | `Reference` |
| `Repeat`, `Cycle`, `Routine` | `RecurrenceRule` |
| `Instance` | `Occurrence` |
| `Notification`, `Alarm` | `Reminder` (configuração) ou `ReminderAlert` (disparo) |
| `DueDate` genérico | `Schedule` |
| `Manager`, `Helper`, `Util`, `Service` genérico | O nome do conceito que falta |
| **Remember** (no nome do produto) | **Renmember** — com "nm", sempre |
