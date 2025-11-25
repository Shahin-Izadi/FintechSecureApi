\# FintechSecureApi

\### A production-ready banking backend built with .NET 8 Clean Architecture



!\[.NET 8](https://img.shields.io/badge/.NET-8.0-5C2D91?logo=.net\&logoColor=white)

!\[C#](https://img.shields.io/badge/Language-C%23-blue)

!\[JWT Auth](https://img.shields.io/badge/Auth-JWT-000000?logo=json-web-tokens)

!\[Entity Framework Core](https://img.shields.io/badge/ORM-EF%20Core%208-512BD4)

!\[SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927?logo=microsoft-sql-server)

!\[Azure](https://img.shields.io/badge/Azure-0078D4?logo=microsoft-azure\&logoColor=white)

!\[Swagger](https://img.shields.io/badge/Docs-Swagger-brightgreen)

!\[License](https://img.shields.io/badge/License-MIT-yellow)



\*\*Live Demo:\*\* https://fintechsecureapi.azurewebsites.net (Open → interactive Swagger UI)



A complete, production-ready banking backend that allows users to register, log in, open accounts, perform transactions (deposit, withdraw, transfer with overdraft protection), view history, and includes admin features – all built with Clean Architecture, JWT authentication, and deployed to Azure.



\## Features

\- User registration \& login with JWT Bearer authentication

\- Role-based authorization (User/Admin)

\- Account management: Create and retrieve accounts

\- Transaction operations: Deposit, withdrawal, transfer (with business logic like overdraft protection)

\- Full transaction history for users

\- Admin dashboard endpoints: View all users, accounts, and transactions

\- Integration with Azure SQL Database for production storage

\- In-memory database option for local development

\- Swagger / OpenAPI documentation with authorization support

\- Entity Framework Core Code-First migrations



\## Project Structure

FintechSecureApi/  

├── .github/             # GitHub workflows  

├── src/  

│   ├── Api/             # Presentation layer: Controllers, Program.cs, appsettings, Swagger config  

│   ├── Application/     # Application layer: MediatR commands/queries, DTOs, Validators, Mappers  

│   ├── Domain/          # Domain layer: Entities (User, Account, Transaction), Interfaces  

│   ├── Infrastructure/  # Infrastructure layer: Repositories, DbContext, Services (e.g., TokenService)  

│   └── Tests.Unit/      # Unit tests for application logic  

├── .gitignore  

├── FintechSecureApi.sln  

├── README.md  

└── tree.txt  



\## API Endpoints (Swagger UI → `/swagger`)



| Method | Endpoint                              | Description                        | Auth   |

|--------|---------------------------------------|------------------------------------|--------|

| POST   | `/api/auth/register`                  | Register a new user                | –      |

| POST   | `/api/auth/login`                     | Login and get authentication token | –      |

| POST   | `/api/Accounts`                       | Create a new account               | Yes    |

| GET    | `/api/Accounts`                       | Retrieve all accounts              | Yes    |

| POST   | `/api/Transactions`                   | Create a new transaction           | Yes    |

| GET    | `/api/Transactions/my`                | Retrieve current user's transactions | Yes  |

| GET    | `/api/admin/users`                    | Retrieve users (admin)             | Yes (Admin) |

| GET    | `/api/admin/accounts`                 | Retrieve accounts (admin)          | Yes (Admin) |

| GET    | `/api/admin/transactions`             | Retrieve transactions (admin)      | Yes (Admin) |



\## Tech Stack

\- ASP.NET Core 8 Web API

\- Entity Framework Core 8 (Code First)

\- SQL Server / In-Memory DB (easy to switch)

\- JWT Authentication

\- MediatR for CQRS

\- FluentValidation

\- Swashbuckle (Swagger)

\- Azure App Service + Azure SQL Database



\## How to Run Locally

1\. Clone the repo  

&nbsp;  ```bash  

&nbsp;  git clone https://github.com/yourusername/FintechSecureApi.git  

&nbsp;  cd FintechSecureApi/src/Api  


2. Add your own appsettings.json with:

&nbsp;    Azure SQL connection string (or local SQL Server/In-Memory)

&nbsp;    JWT Secret (at least 32 characters)
3.Run migrations: dotnet ef migrations add Initial then dotnet ef database update

&nbsp;   Run the app: dotnet run

&nbsp;   Open Swagger at https://localhost:port/swagger



For more details, watch Teddy Smith's YouTube course: https://youtu.be/SIQhe-yt3mA?si=xFzKLm-tHP2DW5jl



Credit

This project was built by following Teddy Smith’s excellent .NET 8 Clean Architecture tutorial series on YouTube.



License

MIT © \[Shahin Izadi] – happy to connect on LinkedIn: https://www.linkedin.com/in/shahin-izadi/ – feel free to star if you like it!

