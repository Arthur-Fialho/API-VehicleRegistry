# API de Registo de Veículos - ASP.NET Core Minimal API

![C#](https://img.shields.io/badge/C%23-11-blue?logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-9.0-purple?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-58A2D9?style=for-the-badge&logo=xunit&logoColor=white)

> Status: Concluído ✅

Uma API RESTful moderna construída com **C#** e **ASP.NET Core Minimal APIs** para gerir o registo de veículos. A aplicação implementa um sistema de autenticação seguro com JWT e autorização baseada em perfis (Roles) para controlar o acesso às operações de CRUD.

Este projeto foi desenvolvido para demonstrar competências no ecossistema .NET, incluindo acesso a dados com Entity Framework Core, implementação de segurança e escrita de testes automatizados.

---

## Tabela de Conteúdos
1. [Funcionalidades Principais](#-funcionalidades-principais)
2. [Tecnologias Utilizadas](#-tecnologias-utilizadas)
3. [Como Executar Localmente](#-como-executar-localmente)
4. [Documentação dos Endpoints](#-documentação-dos-endpoints)
5. [Testes](#-testes)

---

## ✨ Funcionalidades Principais

- **Arquitetura Minimal API:** Endpoints construídos de forma concisa e performática, utilizando a abordagem mais recente do ASP.NET Core.
- **Persistência com Entity Framework Core:** Abordagem "Code-First", onde o esquema da base de dados é gerado e gerido através de *Migrations* a partir das classes de modelo C#.
- **Autenticação com JWT:** Sistema de login que gera um JSON Web Token (JWT) para autenticar requisições subsequentes.
- **Autorização Baseada em Perfis (Roles):** Controlo de acesso granular.
    - **Editor:** Pode criar e ler veículos.
    - **Administrador:** Tem acesso total, incluindo atualizar e apagar veículos.
- **Testes Automatizados:** Cobertura de testes de unidade e de persistência para a camada de serviço, utilizando xUnit, Moq e uma base de dados em memória.
- **Documentação Interativa:** Geração automática de uma UI do Swagger para visualização e teste dos endpoints.

## 🛠️ Tecnologias Utilizadas

| Categoria | Tecnologia |
| :--- | :--- |
| **Framework & Core**| .NET 9, ASP.NET Core |
| **Persistência de Dados** | Entity Framework Core, Npgsql (Provider PostgreSQL) |
| **Segurança** | `Microsoft.AspNetCore.Authentication.JwtBearer` |
| **Testes** | xUnit, Moq, EF Core In-Memory Database |
| **API & Documentação**| Minimal APIs, Swashbuckle (Swagger) |
| **Hashing de Senhas** | BCrypt.Net-Next |

## ⚙️ Como Executar Localmente

### Pré-requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Entity Framework Core Tools](https://docs.microsoft.com/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`)
- PostgreSQL instalado e a rodar.

### Passos
1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/Arthur-Fialho/API-VehicleRegistry
    cd VehicleRegistryAPI
    ```

2.  **Configure a Base de Dados:**
    - Crie uma base de dados no PostgreSQL chamada `vehicleregistry`.
    - No ficheiro `appsettings.json`, atualize a `DefaultConnection` com as suas credenciais do PostgreSQL e defina um segredo forte para o JWT:
      ```json
      {
        "ConnectionStrings": {
          "DefaultConnection": "Host=localhost;Port=5432;Database=vehicleregistry;Username=<seu-usuario>;Password=<sua-senha>"
        },
        "Jwt": {
          "Key": "<SEU_SEGREDO_SUPER_SECRETO_GERADO_AQUI>"
        }
      }
      ```

3.  **Aplique as Migrations:**
    Este comando irá criar as tabelas `Vehicles` e `Users` na sua base de dados.
    ```bash
    dotnet ef database update
    ```

4.  **Execute o projeto:**
    ```bash
    dotnet run
    ```
    A API estará disponível num endereço local (ex: `http://localhost:5167`). A UI do Swagger estará disponível em `/swagger`.

---

## 📖 Documentação dos Endpoints

Todos os endpoints, exceto `/login`, são protegidos e exigem um token JWT no cabeçalho `Authorization: Bearer <seu-token>`.

<details>
<summary>Clique para expandir a documentação da API</summary>

### Autenticação

* `POST /login`
    - Realiza o login e retorna um token JWT. Utilizadores padrão são criados na primeira inicialização (`editor`/`senha123` e `admin`/`senhaforte`).

### Veículos

* `GET /vehicles`: Lista todos os veículos. (Requer `Editor` ou `Administrator`)
* `GET /vehicles/{id}`: Busca um veículo por ID. (Requer `Editor` ou `Administrator`)
* `POST /vehicles`: Cria um novo veículo. (Requer `Editor` ou `Administrator`)
* `PUT /vehicles/{id}`: Atualiza um veículo. (**Requer `Administrator`**)
* `DELETE /vehicles/{id}`: Deleta um veículo. (**Requer `Administrator`**)

**Exemplo de corpo para `POST` ou `PUT` de Veículo:**
```json
{
  "make": "Toyota",
  "model": "Corolla",
  "year": 2025,
  "licensePlate": "BRA2E19"
}
```

</details>

---

## ✅ Testes

O projeto inclui uma suíte de testes automatizados. Para os executar:
```bash
dotnet test
```

---

## Autor

**Arthur Fialho**
* [GitHub](https://github.com/Arthur-Fialho)
* [LinkedIn](https://www.linkedin.com/in/arthurfialho/)