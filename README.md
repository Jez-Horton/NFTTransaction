# IlluviumTest Project

This project is a C# .NET Core application designed to process and manage NFT transactions (Mint, Burn, Transfer) and maintain an updated state of ownership within a MySQL database. The application uses Docker and Docker Compose to manage dependencies, apply migrations, and ensure the database is correctly initialized and maintained.

## Project Structure

The project is organized as follows:

- **IlluviumTest.sln**: The main solution file.
- **IlluviumTest**: Core project containing the main application code.
- **IlluviumTest.Test**: Unit tests for the project.
- **Dockerfile**: Builds the application as a Docker image.
- **docker-compose.yml**: Sets up the application environment with MySQL and runs migrations.
- **wait-for-db.sh**: Script to ensure MySQL is fully ready before running migrations.

## Requirements

- Docker and Docker Compose
- .NET 8.0 SDK (for local development and testing)
- MySQL (Docker-managed within this project)

## Installation

Clone this repository and navigate to the project root.

```bash
git clone https://github.com/Jez-Horton/NFTTransaction
cd IlluviumTest
```

## Project Setup and Running the Application

To start the application, including the MySQL database and migration services, use:

```bash
docker-compose up --build
```

This command:

- Builds the application Docker image.
- Starts the MySQL database and waits for it to initialize.
- Runs a migrate service to apply any pending database migrations. *(In progress) - While this isn't working, will need to run dotnet ef database update once the container has started*

## Configuration Details

- **Database**: The MySQL database is configured in `docker-compose.yml` with environment variables:
  - `MYSQL_ROOT_PASSWORD`
  - `MYSQL_DATABASE`
  - `MYSQL_USER`
  - `MYSQL_PASSWORD`
- **Connection Strings**: The application connects to the database using a connection string configured via environment variables in `docker-compose.yml`.

## Running Migrations

The migrate service in `docker-compose.yml` ensures that `dotnet ef database update` runs to apply migrations before the application starts.

To handle MySQL readiness, the migrate service uses a custom script, `wait-for-db.sh`, which continuously checks if MySQL is ready before running migrations. This ensures a smooth startup without database connectivity issues.

## Running the Application

The application service can be run with the following commands:

- **Read Inline**: `--read-inline <json>`
- **Read File**: `--read-file <file>`
- **NFT Ownership**: `--nft <id>`
- **Wallet Ownership**: `--wallet <address>`
- **Reset**: `--reset`

## Design Decisions and Key Features

### Containerized Environment

- The use of Docker allows for a consistent environment across development and production, ensuring dependencies (e.g., MySQL) are correctly isolated and managed.

### Database Migrations

- Migrations are handled by a separate migrate service in Docker Compose, which applies schema updates to the database.
- The `wait-for-db.sh` script ensures that migrations run only when the MySQL server is fully ready, improving startup reliability.

### NFT Transaction Management

- The application supports "Mint," "Burn," and "Transfer" transactions, with each transaction type stored as a separate entity (e.g., `MintTransaction`, `BurnTransaction`, `TransferTransaction`).
- A `Transaction` table is used to store all transaction logs, while an `NFT` table maintains the current ownership state for each token.

### Error Handling and Validations

- The application includes basic validations, such as ensuring a valid token format and ownership checks before transactions.
- Custom exception handling is implemented in the service layer to ensure informative logging and feedback.

### Unit Testing

- The `IlluviumTest.Test` project includes unit tests to validate core functionality (e.g., transaction handling).
- Tests are designed to ensure that critical actions like Mint, Burn, and Transfer behave as expected in various scenarios.

## Design Decisions

### Use of an ORM (Entity Framework Core)

- **Simplified Data Access**: EF Core abstracts the complexity of raw SQL queries, allowing interaction with the database using C# objects and LINQ expressions.
- **Automated Database Management with Migrations**: EF Core's migration system keeps the database schema consistent with code changes.
- **Improved Maintainability**: EF Core provides a centralized data access layer for easier scaling or refactoring.
- **Type Safety and Model Validation**: Strongly-typed models offer compile-time validation, reducing the likelihood of runtime errors.
- **Flexibility with Multi-Provider Support**: EF Core’s support for multiple database providers allows flexibility in choosing a database provider.

### Use of Serilog for Structured Logging

- **Structured Logging**: Serilog allows for structured logging, making logs easier to analyze, filter, and search.
- **Seamless Integration with .NET Core**: Serilog integrates smoothly with ASP.NET Core’s built-in logging providers.
- **Enhanced Log Context**: Serilog supports adding custom context information to log entries.
- **Flexible Sinks**: Serilog supports a variety of output sinks, such as console, file, and remote logging services.

## Conclusion on Design Choices

Using EF Core and Serilog as foundational components provides a strong balance of simplicity, scalability, and maintainability:

- EF Core enhances data consistency and schema management through its ORM features and migrations.
- Serilog ensures structured, detailed logging that’s crucial for monitoring and debugging.

## Testing

To run unit tests, make sure the .NET 8.0 SDK is installed locally. Then, use:

```bash
dotnet test
```

## Known Issues and Troubleshooting

- **Database Connection Issues**: If `dotnet ef database update` fails to connect, ensure that the database is fully ready by checking the Docker logs.
- **Path Errors**: Ensure the `docker-compose.yml` context and Dockerfile paths align with your project directory structure.

## Future Improvements

1. **Enhanced Security**

   - Secure Connections (SSL)
   - Environment-Specific Configuration
   - Application Secrets Management
   - User Access Control

2. **Advanced Error Handling**

   - Comprehensive Logging
   - Logging Service Integration
   - Retry Policies for Transient Errors
   - Alerting and Monitoring

3. **Blockchain Integration**

   - Blockchain API Integration
   - Smart Contract Interaction
   - Blockchain-Based Authentication

4. **Scalability and Performance Enhancements**

   - Database Optimization
   - Horizontal Scaling
   - Load Balancing
   - Caching

5. **User-Friendly Enhancements**

   - API Documentation
   - Admin Interface
   - Automated CI/CD Pipeline

## License

This project is licensed under the MIT License.

---

This README.md provides all necessary instructions for setting up and running the project, with insights into the design choices and setup decisions for an effective Dockerized environment. Let me know if you'd like further adjustments!

