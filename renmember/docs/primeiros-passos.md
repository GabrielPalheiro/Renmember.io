# Primeiros passos — Renmember.io

> Do zero até o repositório no GitHub, protegido e pronto para a Fatia 0.
> **Ambiente: Windows 11 nativo · PowerShell 7 · VS Code · Docker Desktop.**
> Repositório: https://github.com/GabrielPalheiro/Renmember.io

---

## Fase 0 — Ambiente

> **Já configurou o ambiente para o Alugarme.io?** Pule para a Fase 1 — é o mesmo setup.

### 0.1 Ajustes do Windows

```powershell
winget install Microsoft.PowerShell
winget install Microsoft.WindowsTerminal
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

Como **Administrador**, habilite caminhos longos e (opcional) exclua `C:\dev` do Defender:

```powershell
New-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem" `
  -Name "LongPathsEnabled" -Value 1 -PropertyType DWORD -Force
Add-MpPreference -ExclusionPath "C:\dev"
```

### 0.2 Ferramentas

Como **Administrador**:

```powershell
winget install Microsoft.DotNet.SDK.10
winget install OpenJS.NodeJS.LTS
winget install Git.Git
winget install GitHub.cli
winget install Microsoft.VisualStudioCode
winget install Docker.DockerDesktop
```

Feche e reabra o terminal, depois:

```powershell
dotnet --version
node --version
git --version
gh --version
docker --version
corepack enable
corepack prepare pnpm@latest --activate
pnpm --version
dotnet tool install --global dotnet-ef
```

### 0.3 Git

```powershell
git config --global user.name "Gabriel Palheiros"
git config --global user.email "seu-email@exemplo.com"
git config --global init.defaultBranch main
git config --global pull.rebase true
git config --global core.longpaths true
git config --global core.autocrlf false
gh auth login
```

> **`core.autocrlf false`** porque o `.gitattributes` do projeto já controla o fim de linha.
> Os dois juntos brigam e fazem arquivos aparecerem modificados sem você ter mexido.

---

## Fase 1 — Colocar os arquivos no lugar

O pacote `renmember-estrutura-inicial.zip` está em `Downloads`:

```powershell
New-Item -ItemType Directory -Path C:\dev -Force
Expand-Archive -Path "$HOME\Downloads\renmember-estrutura-inicial.zip" -DestinationPath C:\dev
Set-Location C:\dev\renmember
Get-ChildItem -Force
```

Você deve ver: `.github`, `docs`, `.editorconfig`, `.env.example`, `.gitattributes`, `.gitignore`,
`CLAUDE.md`, `LICENSE`, `README.md`.

> Use `Expand-Archive`, não o "Extrair tudo" do Explorer — ele pode esconder as pastas que
> começam com ponto.

---

## Fase 2 — Conectar ao repositório e subir

### 2.1 Conferir se o repositório no GitHub está vazio

```powershell
gh repo view GabrielPalheiro/Renmember.io --json isEmpty
```

### 2.2a Se `"isEmpty": true`

```powershell
git init
git add .
git commit -m "docs: documentacao inicial do projeto e arquivos base do repositorio"
git remote add origin https://github.com/GabrielPalheiro/Renmember.io.git
git push -u origin main
```

### 2.2b Se `"isEmpty": false` (o GitHub criou README, licença ou `.gitignore`)

Os arquivos deste pacote substituem os gerados pelo GitHub:

```powershell
git init
git remote add origin https://github.com/GabrielPalheiro/Renmember.io.git
git fetch origin
git reset --soft origin/main
git add .
git commit -m "docs: documentacao inicial do projeto e arquivos base do repositorio"
git push -u origin main
```

> `reset --soft` adota o histórico do GitHub sem tocar nos seus arquivos locais — o commit
> seguinte grava a versão deste pacote por cima. Se o GitHub tiver criado a branch com outro
> nome, troque `main` pelo nome mostrado em `git branch -r`.

Confira no navegador:

```powershell
gh repo view --web
```

> Este é o **único push direto na `main`** da vida do projeto. A partir da Fase 3, tudo entra por PR.

---

## Fase 3 — Configurar o GitHub

### 3.1 Configurações gerais

```powershell
gh repo edit GabrielPalheiro/Renmember.io `
  --description "Gestor de atividades pessoais com kanban, calendario e lembretes. .NET 10 + Next.js" `
  --enable-squash-merge `
  --enable-merge-commit=false `
  --enable-rebase-merge=false `
  --delete-branch-on-merge `
  --enable-issues `
  --enable-wiki=false
```

### 3.2 Segurança (gratuito em repositório público)

`Settings → Code security`:

- ☑️ Dependabot alerts
- ☑️ Dependabot security updates
- ☑️ Secret scanning + **Push protection**

### 3.3 Proteger a `main`

`Settings → Rules → Rulesets → New branch ruleset`

| Campo | Valor |
|---|---|
| Nome | `proteger-main` |
| Enforcement | Active |
| Target | Default branch |
| ☑️ Restrict deletions | |
| ☑️ Block force pushes | |
| ☑️ Require a pull request before merging | Approvals: **0** · Allowed merge methods: **Squash** |
| ☑️ Require status checks to pass | Check: `ci` · ☑️ Require branches to be up to date |

> **O check `ci` só aparece na lista depois que o workflow rodar pela primeira vez** (Fatia 0).
> Crie o ruleset agora só com PR obrigatório e bloqueio de force push, e volte para adicionar o
> status check quando o CI existir.

### 3.4 Labels usados pelos templates

```powershell
gh label create feature --color 0E8A16 --description "Funcionalidade nova" --force
gh label create bug --color D73A4A --description "Defeito" --force
```

---

## Fase 4 — Conferir o Claude Code

```powershell
Set-Location C:\dev\renmember
claude
```

Pergunte:

```
Qual e o nome do projeto, quais visoes ele tem e o que esta fora de escopo no v1?
```

Se ele responder **Renmember.io (com "nm")**, citar **lista, kanban, calendário mensal e semanal**
e listar **hospedagem, login e notificações externas** como fora de escopo, o `CLAUDE.md` foi lido.

---

## Fase 5 — Começar a Fatia 0

O primeiro trabalho já segue o fluxo oficial:

```powershell
git switch main
git pull
git switch -c chore/fundacao
claude
```

E dentro do Claude Code: peça o plano da Fatia 0 a partir de `docs/roadmap.md`, revise e aprove.

---

## Erros comuns

| Sintoma | Correção |
|---|---|
| `pnpm` bloqueado por execution policy | Fase 0.1 |
| Arquivos todos modificados sem mexer | `git config --global core.autocrlf false` |
| `path too long` | Caminhos longos (Fase 0.1) + pasta em `C:\dev\renmember` |
| `failed to push some refs` no primeiro push | O repositório não estava vazio — use a Fase 2.2b |
| Push bloqueado por secret scanning | **Rotacione a chave**, só depois limpe o histórico |
| Testcontainers falha | Abrir o Docker Desktop antes dos testes |
| Claude Code ignora as regras | Abrir o `claude` dentro de `C:\dev\renmember` |
