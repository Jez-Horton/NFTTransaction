IlluviumTest Project
This project is a C# .NET Core application designed to process and manage NFT transactions (Mint, Burn, Transfer) and maintain an updated state of ownership within a MySQL database. The application uses Docker and Docker Compose to manage dependencies, apply migrations, and ensure the database is correctly initialized and maintained.

Project Structure
The project is organized as follows:

IlluviumTest.sln: The main solution file.
IlluviumTest: Core project containing the main application code.
IlluviumTest.Test: Unit tests for the project.
Dockerfile: Builds the application as a Docker image.
docker-compose.yml: Sets up the application environment with MySQL and runs migrations.
wait-for-db.sh: Script to ensure MySQL is fully ready before running migrations.
Requirements
Docker and Docker Compose
.NET 8.0 SDK (for local development and testing)
MySQL (Docker-managed within this project)
Installation
Clone this repository and navigate to the project root.

bash
Copy code
git clone https://github.com/your-repository/IlluviumTest.git
cd IlluviumTest
Project Setup and Running the Application
To start the application, including the MySQL database and migration services, use:

docker-compose up --build
This command:

Builds the application Docker image.
Starts the MySQL database and waits for it to initialize.
Runs a migrate service to apply any pending database migrations. {In progress}
Configuration Details
Database: The MySQL database is configured in docker-compose.yml with environment variables:
MYSQL_ROOT_PASSWORD
MYSQL_DATABASE
MYSQL_USER
MYSQL_PASSWORD
Connection Strings: The application connects to the database using a connection string configured via environment variables in docker-compose.yml.
Running Migrations
The migrate service in docker-compose.yml ensures that dotnet ef database update runs to apply migrations before the application starts.

To handle MySQL readiness, the migrate service uses a custom script, wait-for-db.sh, which continuously checks if MySQL is ready before running migrations. This ensures a smooth startup without database connectivity issues.

Running the Application
The application service, as per the guidelines it's a single run line application with the following commands: 
Read Inline (--read-inline <json>)
Read File (--read-file <file>)
NFT Ownership (--nft <id>)
Wallet Ownership (--wallet <address>)
Reset (--reset)

Design Decisions and Key Features
Containerized Environment:

The use of Docker allows for a consistent environment across development and production, ensuring dependencies (e.g., MySQL) are correctly isolated and managed.
Database Migrations:

Migrations are handled by a separate migrate service in Docker Compose, which applies schema updates to the database.
The wait-for-db.sh script ensures that migrations run only when the MySQL server is fully ready, improving startup reliability.
NFT Transaction Management:

The application supports "Mint," "Burn," and "Transfer" transactions, with each transaction type stored as a separate entity (e.g., MintTransaction, BurnTransaction, TransferTransaction).
A Transaction table is used to store all transaction logs, while an NFT table maintains the current ownership state for each token.
Error Handling and Validations:

The application includes basic validations, such as ensuring a valid token format and ownership checks before transactions.
Custom exception handling is implemented in the service layer to ensure informative logging and feedback.
Unit Testing:

The IlluviumTest.Test project includes unit tests to validate core functionality (e.g., transaction handling).
Tests are designed to ensure that critical actions like Mint, Burn, and Transfer behave as expected in various scenarios.

Design Decisions
Use of an ORM (Entity Framework Core)
Choosing an ORM like Entity Framework Core (EF Core) provided several advantages for this project:

Simplified Data Access:

EF Core abstracts the complexity of raw SQL queries, allowing us to interact with the database using C# objects and LINQ expressions. This makes code more readable, maintainable, and less error-prone, especially for developers familiar with C# but not necessarily with SQL.
Automated Database Management with Migrations:

EF Core’s migration system allows schema changes to be automatically applied to the database in a versioned manner. This ensures the database schema remains consistent with code changes and avoids manual intervention in updating schemas across different environments.
Using EF Core migrations in a Dockerized environment also facilitates seamless database version control, as changes can be automatically applied when the application container starts.
Improved Maintainability:

EF Core provides a centralized data access layer, where business rules and database interactions are organized within the same service layer. This setup improves maintainability and makes it easier to scale or refactor as the application grows.
Type Safety and Model Validation:

EF Core’s strongly-typed models offer compile-time validation, reducing the likelihood of runtime errors that can arise from data mismatches. By leveraging attributes and Fluent API configurations, EF Core ensures data consistency and allows validation rules to be enforced at the model level.
Flexibility with Multi-Provider Support:

EF Core’s support for multiple database providers allows the application to adapt to different databases (e.g., MySQL, PostgreSQL, SQL Server) with minimal changes to the data access layer. This design decision allows flexibility in choosing a database provider, which can be advantageous for scaling or migrating in the future.
Use of Serilog for Structured Logging
Serilog was selected as the logging framework for the following reasons:

Structured Logging:
Unlike traditional logging frameworks, Serilog allows for structured logging, where logs are output in JSON or other structured formats. This structure makes logs easier to analyze, filter, and search, especially when integrated with logging solutions like the ELK Stack (Elasticsearch, Logstash, and Kibana) or Seq.
Seamless Integration with .NET Core:
Serilog integrates smoothly with ASP.NET Core’s built-in logging providers, making it easy to replace the default logging without extensive reconfiguration. This integration supports dependency injection, making it straightforward to log from any part of the application.
Enhanced Log Context:
Serilog supports adding custom context information to log entries, such as user ID, transaction ID, or other metadata. This can be extremely helpful for tracking the flow of specific transactions or debugging issues by providing additional context.
Flexible Sinks:
Serilog supports a variety of output “sinks,” including console, file, and remote logging services like Azure Application Insights and Seq. This flexibility allows logs to be directed to different outputs depending on the environment (e.g., console logs for development, and structured JSON for production).
Error and Exception Handling:
With Serilog’s detailed logging capabilities, error and exception data can be captured in a structured way, including stack traces and custom fields. This helps in identifying and resolving issues quickly and improves overall reliability by providing comprehensive insights into application behavior.
Conclusion on Design Choices
Using EF Core and Serilog as foundational components of the application’s architecture provides a strong balance of simplicity, scalability, and maintainability:

EF Core enhances data consistency and schema management through its ORM features and migrations, making it a valuable choice for database-driven applications.
Serilog ensures structured, detailed logging that’s crucial for monitoring and debugging, particularly in a production environment.
These decisions align well with the application's needs for data consistency, robust error handling, and extensibility, contributing to a reliable, maintainable codebase suitable for both development and production use cases.
Testing
To run unit tests, make sure the .NET 8.0 SDK is installed locally. Then, use:

bash
dotnet test

Known Issues and Troubleshooting
Database Connection Issues: If dotnet ef database update fails to connect, ensure that the database is fully ready by checking the Docker logs. You can also increase the health check interval or retries in docker-compose.yml.
Path Errors: Ensure the docker-compose.yml context and Dockerfile paths align with your project directory structure. Using relative paths correctly in Dockerfile COPY commands is essential for successful builds.

Future Improvements
1. Enhanced Security
Secure Connections (SSL): Enable SSL/TLS for MySQL connections and any API integrations to ensure data-in-transit security. This would involve updating connection strings and possibly adding SSL certificates, especially if deploying in a production environment.
Environment-Specific Configuration: Use environment-specific configurations for sensitive data, storing production secrets (e.g., database credentials, API keys) securely through a service like AWS Secrets Manager, Azure Key Vault, or HashiCorp Vault.
Application Secrets Management: Consider using Docker secrets or Kubernetes secrets to manage sensitive information. This provides a more secure alternative to passing environment variables directly in Docker Compose for production deployments.
User Access Control: For future development, implementing user access control and role-based permissions could further secure sensitive parts of the application and better align with industry-standard security practices.
2. Advanced Error Handling
Comprehensive Logging: Integrate structured logging throughout the application to capture transaction details, errors, and performance metrics. Structured logging with Serilog or NLog (already partially set up with Serilog) can output logs in JSON format, which is ideal for centralized log aggregation.
Logging Service Integration: Deploy a logging and monitoring solution, such as the ELK Stack (Elasticsearch, Logstash, and Kibana) or Grafana and Loki. This would enable centralized log storage, real-time error monitoring, and more detailed insight into transaction processing.
Retry Policies for Transient Errors: Implement retry policies, particularly for database operations, using EnableRetryOnFailure for MySQL. This could reduce the impact of temporary connectivity issues, especially in production or cloud environments.
Alerting and Monitoring: Configure real-time alerting (e.g., with Prometheus and Alertmanager if using Grafana) for critical errors and unexpected behavior. This setup could notify the development team immediately if the service encounters issues or if the database goes offline.
3. Blockchain Integration
Blockchain API Integration: Since the application deals with NFTs, integrating with a blockchain API (e.g., Ethereum, Polygon) could enable real-time validation of NFT transactions against blockchain records. This would improve data integrity and could allow the system to operate as an off-chain storage and management solution for on-chain assets.
Smart Contract Interaction: Consider adding support for smart contract interactions, where the application could trigger on-chain events for operations like minting and transferring NFTs. Web3.js or Nethereum could be used to facilitate interaction with smart contracts, extending the application’s scope to fully decentralized operations.
Blockchain-Based Authentication: Future iterations could implement blockchain-based user authentication, allowing users to interact with the application using wallets. This would enhance security by using decentralized identity management.
4. Scalability and Performance Enhancements
Database Optimization: Use database indexing and query optimization to handle increasing transaction volume efficiently, particularly for complex NFT ownership queries.
Horizontal Scaling: For increased workload support, consider deploying the application in a container orchestration platform like Kubernetes or Docker Swarm. This would allow for horizontal scaling to handle more transactions concurrently.
Load Balancing: To improve response times and handle more requests, incorporate load balancing (e.g., NGINX or HAProxy) if deploying in a clustered environment.
Caching: Introduce caching (e.g., with Redis or Memcached) to store frequently accessed data, such as ownership states, reducing the load on the MySQL database and improving response times for read-heavy operations.
5. User-Friendly Enhancements
API Documentation: Add API documentation using Swagger or NSwag to simplify integration for external developers and provide a clear interface for interacting with the application’s endpoints.
Admin Interface: For ease of monitoring and management, consider developing a web-based admin interface to visualize NFT ownership and transaction history. This could provide real-time insights into system operations and transaction states.
Automated CI/CD Pipeline: Set up a continuous integration and continuous deployment pipeline (e.g., with GitHub Actions, Azure DevOps, or Jenkins) to automate testing, build, and deployment. This would improve deployment speed and consistency, especially in a production environment.
License
This project is licensed under the MIT License.

This README.md provides all necessary instructions for setting up and running the project, with insights into the design choices and setup decisions for an effective Dockerized environment. Let me know if you'd like further adjustments!






