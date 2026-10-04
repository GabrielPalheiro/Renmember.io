# Renmember.io

> Gestor de atividades pessoais: cadastre suas responsabilidades, veja tudo em **kanban** ou
> **calendário** e seja lembrado na hora certa.

**Status:** em construção — Fatia 0 (Fundação). Veja o [roadmap](docs/roadmap.md).

Um projeto da **GP.Tech** · por Gabriel Palheiros

---

## O que é

| Visão | Para quê |
|---|---|
| **Lista** | Cadastrar atividades com referência (link e nota) e categoria |
| **Kanban** | Ver o que está a fazer, em andamento e feito — arrastando cartões |
| **Calendário mensal e semanal** | Planejar a semana e o mês numa tela só |
| **Recorrência** | Rotinas e contas cadastradas uma vez, repetidas automaticamente |
| **Lembretes** | Avisos dentro da aplicação, com antecedência configurável |

O v1 roda **localmente**, para um único usuário. O código é aberto e a arquitetura já está
preparada para virar um serviço online multiusuário.

## Stack

**Backend:** .NET 10 · Minimal APIs · EF Core 10 · PostgreSQL · Clean Architecture
**Frontend:** Next.js · React · TypeScript · Tailwind CSS · shadcn/ui · TanStack Query · dnd-kit · FullCalendar
**Qualidade:** xUnit · Testcontainers · NetArchTest · Vitest · Playwright · GitHub Actions · release-please

## Como rodar

> Disponível a partir da Fatia 0.

Pré-requisito: [Docker Desktop](https://www.docker.com/products/docker-desktop/).

```powershell
git clone https://github.com/GabrielPalheiro/Renmember.io.git
cd Renmember.io
docker compose -f infra\docker\docker-compose.yml up
```

| Serviço | Endereço |
|---|---|
| Aplicação | http://localhost:3000 |
| API + documentação (Scalar) | http://localhost:8080/scalar |

## Documentação

| Documento | Conteúdo |
|---|---|
| [Blueprint](docs/blueprint.md) | Propósito, arquitetura e decisões |
| [Domínio](docs/dominio.md) | Linguagem ubíqua — o nome de cada conceito |
| [Roadmap](docs/roadmap.md) | Fatias de entrega |
| [Contribuindo](docs/contribuindo.md) | Branches, commits, PRs, CI, versões e migrations |
| [Primeiros passos](docs/primeiros-passos.md) | Setup do ambiente do zero |

## Licença

[MIT](LICENSE)
