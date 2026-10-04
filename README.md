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

Pré-requisito: [Docker Desktop](https://www.docker.com/products/docker-desktop/).

```powershell
git clone https://github.com/GabrielPalheiro/Renmember.io.git
cd Renmember.io
docker compose -f infra\docker\docker-compose.yml up --build
```

| Serviço | Endereço |
|---|---|
| Aplicação | http://localhost:3000 |
| API + documentação (Scalar) | http://localhost:8080/scalar |
| Saúde da API e do banco | http://localhost:8080/health |

O compose já tem valores padrão de desenvolvimento. Para mudar portas, senha do banco ou o fuso do
usuário, copie o `.env.example` e passe o arquivo explicitamente (o compose procura o `.env` na
pasta do `docker-compose.yml`, não na raiz):

```powershell
Copy-Item .env.example .env
docker compose -f infra\docker\docker-compose.yml --env-file .env up --build
```

Na primeira subida, a API aplica as migrations e cria o usuário do v1.

### Desenvolvimento sem containers para API e Web

Requer .NET SDK 10 e Node 24 com pnpm (ver [primeiros passos](docs/primeiros-passos.md)).

```powershell
docker compose -f infra\docker\docker-compose.yml up -d db   # só o Postgres
dotnet tool restore
dotnet run --project api\src\Renmember.Api                    # http://localhost:8080
Set-Location web; pnpm install; pnpm dev                      # http://localhost:3000
```

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
