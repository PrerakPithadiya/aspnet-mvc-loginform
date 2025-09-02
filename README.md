# ASP.NET MVC Login Form with MySQL Database

This project is a simple ASP.NET Core MVC web application that demonstrates user authentication (login, registration, password reset) using a MySQL database. It is designed as a learning resource for students and developers who want to understand how to connect ASP.NET Core to MySQL and implement basic authentication flows.

## Features

- User registration with full name, username, and password
- Secure login form
- Forgot password and reset password functionality
- Entity Framework Core for database access
- MySQL as the backend database
- MVC architecture with controllers, models, and views
- Password visibility toggle (JavaScript)

## Project Structure

```
loginform-with-database.sln
loginform-with-database/
  Controllers/
    AccountController.cs
    HomeController.cs
  Data/
    ApplicationDbContext.cs
  Models/
    User.cs
    ForgotPasswordModel.cs
  Migrations/
  Views/
    Account/
      Login.cshtml
      Register.cshtml
      ForgotPassword.cshtml
      ResetPassword.cshtml
    Home/
      Index.cshtml
    Shared/
      _Layout.cshtml
      _ValidationScriptsPartial.cshtml
  wwwroot/
    js/
      password-toggle.js
      site.js
  appsettings.json
  appsettings.Development.json
  Program.cs
```

## Database

- MySQL database with a `Users` table:
  - `Id` (int, PK)
  - `FullName` (varchar)
  - `Username` (varchar, unique)
  - `Password` (varchar)
- Entity Framework Core migrations are included for schema management.

## Getting Started

1. **Clone the repository**
2. **Configure MySQL connection**
   - Update `appsettings.json` with your MySQL connection string.
3. **Apply migrations**
   - Run `dotnet ef database update` to create the database and tables.

# aspnet-mvc-loginform

An ASP.NET Core MVC sample that implements a simple login and registration flow using MySQL and Entity Framework Core. This repository is intended as an educational example showing how to wire up MVC views, controllers, and EF Core for basic account flows.

## Short description (suggested GitHub repo description)

A simple ASP.NET Core MVC login form with MySQL (EF Core) — registration, login, and password reset example.

## Highlights

- ASP.NET Core MVC app (net9.0)
- Entity Framework Core with MySQL (DbContext + migrations included)
- Registration, Login, and Forgot/Reset Password flows
- Minimal client JS (password visibility toggle)

## Project structure

```
loginform-with-database.sln
loginform-with-database/
  Controllers/
    AccountController.cs
    HomeController.cs
  Data/
    ApplicationDbContext.cs
  Migrations/
  Models/
    User.cs
    ForgotPasswordModel.cs
  Views/
    Account/
      Login.cshtml
      Register.cshtml
      ForgotPassword.cshtml
      ResetPassword.cshtml
    Home/
      Index.cshtml
    Shared/
      _Layout.cshtml
      _ValidationScriptsPartial.cshtml
  wwwroot/
    js/
      password-toggle.js
      site.js
  Program.cs
  appsettings.json
  appsettings.Development.json
```

## How the code works (quick)

- `Program.cs` registers `ApplicationDbContext` using the `DefaultConnection` connection string and calls `Database.EnsureCreated()` at startup so the DB/tables exist when running locally.
- `ApplicationDbContext` exposes `DbSet<User> Users` and configures column sizes and a unique index on `Username`.
- `AccountController` implements registration, login, and password reset logic (see source).

## Important security note

This sample stores passwords in plain text to keep the example minimal. Do NOT store plaintext passwords in production. Use ASP.NET Core Identity or a secure hashing algorithm (PBKDF2/Argon2) and proper password reset token flows for real applications.

## Preparing the project for GitHub

1. Remove or sanitize secrets in `appsettings.json` before pushing. Replace real credentials with placeholders:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=LoginFormDb;User=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;"
}
```

2. Keep `appsettings.Development.json` and other local-only files out of version control by adding them to `.gitignore`.

3. Optionally use the Secret Manager for local secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
```

## Run locally

Prerequisites:

- .NET 9.0 SDK or later
- MySQL Server

Commands:

```powershell
# apply migrations (optional, the app calls EnsureCreated at startup)
dotnet ef database update

# build and run
dotnet build
dotnet run
```

Open the URL shown in the console (usually `https://localhost:5001`).

## Notes about the `Users` table

- Columns: `Id` (int, PK), `FullName` (varchar(100)), `Username` (varchar(50), unique), `Password` (varchar(100)).
- A migration was added that runs SQL to reorder the `Users` columns (keeps `Id` first, then `FullName`, `Username`, `Password`). Back up your data before running schema changes.

## Push to GitHub (project root)

Example commands to create a new remote repo and push the current project to the repository root:

```powershell
cd 'E:\Noble Sem 5\ASP.NET\loginform-with-database'

git init
git add .
git commit -m "Initial commit: ASP.NET MVC login form"

git remote add origin https://github.com/<your-username>/aspnet-mvc-loginform.git
git branch -M main
git push -u origin main
```

Replace `<your-username>` with your GitHub username. If you prefer the GitHub CLI you can create the repo and push in one step with `gh repo create`.

## Suggested next steps I can take for you

- Add a `.gitignore` tailored for .NET/Visual Studio
- Add an `appsettings.json.example` (sanitized) with placeholders
- Add a concise `RUNNING.md` with step-by-step instructions

## License

This repository is provided for educational purposes and can be used and modified freely.
