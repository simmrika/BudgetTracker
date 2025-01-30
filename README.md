Overview
The Budget Tracker Application is designed to help users manage their personal finances by tracking income, expenses, and categorized transactions. The system integrates with bank APIs, supports manual cash entries, and provides budget limit notifications. The architecture follows a modular design, ensuring scalability, maintainability, and security.

System Architecture
The application follows a three-tier architecture, comprising:

Presentation Layer (Frontend)

Technology: ASP.NET Core Razor Pages
Purpose: Provides a user-friendly interface for managing financial transactions, tracking expenses, and generating reports.
Features:
Responsive UI with dynamic content rendering.
Secure authentication using cookie-based authentication.
Dashboard visualization with income vs. expenses charts.


Business Logic Layer (Backend)

Technology: .NET Core with Entity Framework Core (EF Core)
Purpose: Manages core functionalities like data processing, business rules, and API communications.
Features:
Budget tracking logic (income, expenses, budget limits).
Transaction categorization using predefined and custom categories.
API request handling for bank account integration.
Data Layer (Database)

Database: SQL Server
Purpose: Stores user accounts, transactions, categories, and linked bank accounts.
Features:
Structured storage for financial data.
Secure user authentication with JWT tokens.
Relationship management between user accounts, transactions, and spending categories.
