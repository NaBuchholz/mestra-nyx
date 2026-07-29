# Mestra Nyx

Mestra Nyx é uma aplicação para gerenciamento e publicação de conteúdo de RPG.

## Execução local da API

### Pré-requisitos

- Docker em execução no host;
- projeto aberto no Dev Container;
- acesso ao Docker do host pelo Dev Container.

### 1. Configurar o PostgreSQL

Crie o arquivo local de variáveis de ambiente a partir do modelo:

```fish
cp .env.example .env
```

Substitua os valores `change_me` de `.env` por credenciais locais. O arquivo
`.env` não deve ser versionado.

Inicie o PostgreSQL:

```fish
docker compose up -d postgres
```

Confirme que o serviço alcançou o estado `healthy`:

```fish
docker compose ps postgres
```

O PostgreSQL é publicado pelo Docker Compose na porta `5432` do host. De dentro
do Dev Container, a API acessa o host pelo endereço `host.docker.internal`.

### 2. Configurar a connection string da API

A API lê `ConnectionStrings:DefaultConnection` dos provedores de configuração
do .NET e interrompe a inicialização quando o valor está ausente ou vazio. Em
desenvolvimento, armazene esse valor com .NET User Secrets.

Para evitar que a connection string seja gravada no histórico do Fish, leia-a
de forma oculta:

```fish
read --silent --prompt-str "Connection string: " MESTRA_NYX_CONNECTION_STRING
echo
```

Quando solicitado, informe uma connection string com este formato, substituindo
os placeholders pelos mesmos valores definidos no seu `.env`:

```text
Host=host.docker.internal;Port=5432;Database=<database>;Username=<username>;Password=<password>
```

Confirme que a variável temporária não está vazia, grave o User Secret e remova
a variável da sessão:

```fish
string match -qr '\S' -- "$MESTRA_NYX_CONNECTION_STRING"; and echo "VALUE=nonblank"; or echo "VALUE=blank"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "$MESTRA_NYX_CONNECTION_STRING" --project src/MestraNyx.API/MestraNyx.API.csproj
set -e MESTRA_NYX_CONNECTION_STRING
```

Não execute `dotnet user-secrets list` em logs, issues ou outros locais
compartilhados, pois o comando exibe o valor configurado.

### 3. Iniciar a API

Com o PostgreSQL saudável e o User Secret configurado, execute:

```fish
dotnet run --project src/MestraNyx.API/MestraNyx.API.csproj
```

O profile HTTP de desenvolvimento inicia a API em:

```text
http://localhost:5063
```

### Troubleshooting do volume de User Secrets

O Dev Container monta o volume persistente `mestra-nyx-usersecrets` em
`/home/vscode/.microsoft/usersecrets`. Se o comando `dotnet user-secrets set`
falhar por falta de permissão, verifique o mount e a capacidade de escrita:

```fish
findmnt /home/vscode/.microsoft/usersecrets
stat -c '%U:%G %a %n' /home/vscode/.microsoft/usersecrets
test -w /home/vscode/.microsoft/usersecrets; and echo "USER_SECRETS_WRITE=allowed"; or echo "USER_SECRETS_WRITE=blocked"
```

Se o volume estiver gravável, mas pertencer a `root`, ajuste sua propriedade
dentro do Dev Container:

```fish
sudo chown -R vscode:vscode /home/vscode/.microsoft/usersecrets
```

O volume preserva os User Secrets entre rebuilds do Dev Container. Ele não
substitui um gerenciador de segredos de produção.

## Migrations do banco de dados

As migrations do Entity Framework Core versionam a evolução do schema do banco.
Os arquivos gerados ficam em
`src/MestraNyx.Infrastructure/Persistence/Migrations` e devem ser revisados
antes de serem aplicados ou versionados.

O projeto usa uma ferramenta local para manter a versão do `dotnet-ef`
reproduzível. Após clonar o repositório ou reconstruir o Dev Container, restaure
as ferramentas declaradas no manifest:

```fish
dotnet tool restore
```

### Gerar uma migration

Depois de alterar o modelo ou seu mapeamento, gere uma migration com um nome que
descreva a mudança:

```fish
dotnet ef migrations add <MigrationName> \
  --project src/MestraNyx.Infrastructure/MestraNyx.Infrastructure.csproj \
  --startup-project src/MestraNyx.API/MestraNyx.API.csproj \
  --context ApplicationDbContext \
  --output-dir Persistence/Migrations
```

O projeto Infrastructure é o destino porque contém o `ApplicationDbContext` e
as migrations. A API é o startup project porque fornece a composição e a
configuração necessárias para o EF criar o contexto em design time.

Antes de aplicar uma migration, revise seus métodos `Up` e `Down`, o model
snapshot, os tipos das colunas, a nulabilidade, os limites, as chaves e os
índices gerados.

### Aplicar migrations pendentes

Com o PostgreSQL saudável e `DefaultConnection` configurada por User Secrets,
execute:

```fish
dotnet ef database update \
  --project src/MestraNyx.Infrastructure/MestraNyx.Infrastructure.csproj \
  --startup-project src/MestraNyx.API/MestraNyx.API.csproj \
  --context ApplicationDbContext
```

O EF registra as migrations aplicadas na tabela `__EFMigrationsHistory`.

### Rollback local

Para retornar o banco ao estado anterior à primeira migration:

```fish
dotnet ef database update 0 \
  --project src/MestraNyx.Infrastructure/MestraNyx.Infrastructure.csproj \
  --startup-project src/MestraNyx.API/MestraNyx.API.csproj \
  --context ApplicationDbContext
```

> **Atenção:** esse rollback executa o método `Down` da migration inicial,
> remove a tabela `campaigns` e apaga todos os dados armazenados nela. Não
> execute o comando em um banco compartilhado ou de produção sem backup,
> validação e um plano de recuperação.

## Segurança de configuração

- Não versione `.env`, connection strings, senhas ou arquivos de User Secrets.
- Não coloque credenciais em `appsettings.json`.
- Não registre connection strings em logs.
- Use credenciais próprias e de menor privilégio para cada ambiente.
