# Controle de Gastos da Casa

Sistema completo para gerenciamento de gastos domésticos. A aplicação permite o controle de transações financeiras, categorização de gastos e gerenciamento de pessoas, oferecendo uma visão clara e organizada das finanças domésticas.

## Principais Tecnologias Utilizadas

### Backend (API)

- **.NET 9.0** - Framework principal da aplicação
- **ASP.NET Core 9.0** - Framework web para construção da API REST
- **PostgreSQL 16** - Banco de dados relacional

### Frontend

- **React 19.2.0** - Biblioteca para construção da interface
- **TypeScript 5.9.3** - Superset JavaScript com tipagem estática

### Infraestrutura

- **Docker** - Containerização da aplicação
- **Docker Compose** - Orquestração de containers

## Estrutura do Projeto

O projeto segue uma arquitetura em camadas (Clean Architecture), separando as responsabilidades em diferentes projetos:

```
Controle-Gastos-Casa/
├── api/                                    # Backend .NET
│   ├── ControleGastosCasa.Api/            # Camada de apresentação (Controllers)
│   ├── ControleGastosCasa.Application/    # Camada de aplicação (Services, DTOs)
│   ├── ControleGastosCasa.Domain/         # Camada de domínio (Entities, Enums)
│   ├── ControleGastosCasa.Infrastructure/ # Camada de infraestrutura (Repositories, DbContext)
│   └── ControleGastosCasa.Tests/          # Testes unitários
├── frontend/                               # Frontend React
│   ├── src/
│   │   ├── components/                     # Componentes reutilizáveis
│   │   ├── pages/                          # Páginas da aplicação
│   │   ├── services/                       # Serviços de API
│   │   ├── hooks/                          # Custom hooks
│   │   ├── helpers/                        # Funções auxiliares
│   │   └── types/                          # Tipagens do TypeScript
│   └── public/                             # Arquivos estáticos
└── docker-compose.yml                      # Configuração Docker Compose
```

## Pré-requisitos

### Para execução com Docker

- Docker Desktop ou Docker Engine instalado
- Docker Compose instalado

### Para execução sem Docker

**Backend:**
- .NET SDK 9.0
- PostgreSQL 16 instalado e em execução

**Frontend:**
- Node.js 20 ou superior

## Como Executar

### Opção 1: Usando Docker (Recomendado)

Esta é a forma mais simples de executar o projeto, pois não requer instalação manual de dependências.

1. Após clonar o repositório e estar na pasta raiz, execute o Docker Compose:
```bash
docker-compose up --build
```

2. Aguarde alguns instantes para que todos os containers sejam iniciados. O sistema estará disponível em:
   - **Frontend**: http://localhost:5173
   - **API**: http://localhost:5027
   - **Swagger**: http://localhost:5027/swagger

### Opção 2: Execução Manual (Sem Docker)

#### Configuração do Banco de Dados

1. Instale e inicie o PostgreSQL 16
2. Crie um banco de dados:
```sql
CREATE DATABASE controle_gastos;
CREATE USER controle WITH PASSWORD 'controle';
GRANT ALL PRIVILEGES ON DATABASE controle_gastos TO controle;
```

#### Executando a API

1. Navegue até a pasta da API:
```bash
cd api
```

2. Configure a string de conexão no arquivo `ControleGastosCasa.Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=controle_gastos;Username=controle;Password=controle;Include Error Detail=true"
  }
}
```

3. Restaure as dependências e execute as migrações:
```bash
dotnet restore
dotnet ef database update --project ControleGastosCasa.Infrastructure --startup-project ControleGastosCasa.Api
```

4. Execute a API:
```bash
cd ControleGastosCasa.Api
dotnet run
```

A API estará disponível em http://localhost:5027 e o Swagger em http://localhost:5027/swagger

#### Executando o Frontend

1. Navegue até a pasta do frontend:
```bash
cd frontend
```

2. Instale as dependências:
```bash
npm install
```

3. Configure a URL da API no arquivo `.env` ou `.env.local`:
```env
VITE_API_URL=http://localhost:5027
```

4. Execute o servidor de desenvolvimento:
```bash
npm run dev
```

O frontend estará disponível em http://localhost:5173

## Aplicação publicada via Render para maior facilidade  
#### * Por ser Free Tier pode levar algum tempo pra conseguir acessar, pois as aplicações entram em modo sono por inatividade e retornam após receber requisições, pode levar de 3 a 5 minutos.

A aplicação está disponível em produção nos seguintes endereços:

- **Frontend**: https://controle-gastos-casa-frontend.onrender.com/
- **API com Swagger**: https://controle-gastos-casa-api.onrender.com/swagger/index.html

## Funcionalidades

### Gestão de Pessoas
- Cadastro, edição e listagem de pessoas
- Visualização de totais gerais
- Busca e paginação

### Gestão de Categorias
- Cadastro, edição e listagem de categorias
- Categorização por finalidade (Receita ou Despesa)
- Visualização de totais gerais
- Busca e paginação

### Gestão de Transações
- Cadastro, edição e listagem de transações
- Associação com pessoas e categorias
- Filtros por tipo (Receita ou Despesa)
- Visualização de totais gerais
- Busca e paginação

## Estrutura da API

A API RESTful oferece os seguintes endpoints:

### Pessoas
- `GET /api/Pessoa` - Lista todas as pessoas (com paginação)
- `GET /api/Pessoa/{id}` - Obtém uma pessoa específica
- `POST /api/Pessoa` - Cria uma nova pessoa
- `PUT /api/Pessoa/{id}` - Atualiza uma pessoa
- `DELETE /api/Pessoa/{id}` - Remove uma pessoa

### Categorias
- `GET /api/Categoria` - Lista todas as categorias (com paginação)
- `GET /api/Categoria/{id}` - Obtém uma categoria específica
- `POST /api/Categoria` - Cria uma nova categoria
- `PUT /api/Categoria/{id}` - Atualiza uma categoria
- `DELETE /api/Categoria/{id}` - Remove uma categoria

### Transações
- `GET /api/Transacao` - Lista todas as transações (com paginação e filtros)
- `GET /api/Transacao/{id}` - Obtém uma transação específica
- `POST /api/Transacao` - Cria uma nova transação
- `PUT /api/Transacao/{id}` - Atualiza uma transação
- `DELETE /api/Transacao/{id}` - Remove uma transação
- `GET /api/Transacao/totais` - Obtém totais gerais

A documentação completa da API está disponível através do Swagger quando a aplicação estiver em execução.

## Testes

### Frontend

Para executar os testes do frontend:
```bash
cd frontend
npm test
```

Para executar com interface visual:
```bash
npm run test:ui
```

### Backend

Para executar os testes do backend:
```bash
cd api
dotnet test
```

