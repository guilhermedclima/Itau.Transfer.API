# Projeto de Transferência Bancária em C#

Este projeto é uma aplicação C# que lê informações de um arquivo Docker Compose para obter contas e saldos e realizar transferências bancárias.

 
## Descrição

Esta aplicação foi desenvolvida para gerenciar contas bancárias e realizar transferências entre elas. Utiliza Entity Framework Core para interagir com um banco de dados SQL Server, e Docker Compose para gerenciar o ambiente de contêineres.

## Requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

## Instalação

1. Clone este repositório:
    ```sh
    git clone https://github.com/guilhermedclima/Itau.Transfer.API
    cd Itau.Transfer.API
    ```

## Configuração

1. Configure a string de conexão no arquivo `appsettings.json`:
    ```json
    {
      "ConnectionStrings": {
         "SqlServer": "Server=localhost\\;Database=Transferencias;user id=sa;password=12345qwert@;MultipleActiveResultSets=true;Encrypt=false"
      }
    }
    ```


## Uso

1. Suba os contêineres do Docker Compose:
    ```sh
    docker-compose up --build
    ```
2. Clone o projeto https://github.com/mllcarvalho/DesafioItau
3. cd DesafioItau
4. docker-compose up --build -d
2. Acesse a aplicação em `http://localhost:8080/swagger/index.html`.
