## Project Structure 🗂️

The solution is organized into several projects and folders, following Domain-Driven Design methodology along with Clean/Hexagonal Architecture. Below is a description of the main function of each folder/project:

- **SeedWork** – Base classes, utilities, and components shared among different projects (for example, the base `Entity` class).

- **GraphT.Model** – Contains domain models, entities, value objects, domain services, and exceptions. Defines the core business logic and rules.

- **GraphT.UseCases** – Implements use cases and application logic. This is where the workflow between models and repositories is orchestrated.

- **GraphT.Controllers** – API controllers, responsible for delegating logic to use cases and decoupling them from frameworks.

- **GraphT.Presenters** – Responsible for presenting and transforming data between business logic and the presentation layer (DTOs, view models, etc.).

- **GraphT.EfCore.Repositories** – Repository implementations using Entity Framework Core for persistence in relational databases.

- **GraphT.IoC** – Dependency injection configuration and inversion of control containers.

- **GraphT.WebAPI** – Main RESTful API project built with ASP.NET Core. Exposes endpoints to interact with the system.

---

## Getting Started 🚀

### Prerequisites 📋

- [MS SQL Server](https://learn.microsoft.com/en-us/sql/database-engine/install-windows/install-sql-server?view=sql-server-ver17). You can use a [Docker image](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver17&tabs=cli&pivots=cs1-bash) _(Recommended for local testing)_
- [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet)

### Installation 🔧

1. _Clone the repository_

```bash
   git clone https://github.com/yourusername/GraphT.NET.git
   cd GraphT.NET
```

2. _Restore dependencies_

```bash
   dotnet restore
```

3. _Build the solution_

```bash
    dotnet build
```

4. _Configure your [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0)_: Modify the ConnectionString according to your Database

```bash
	dotnet user-secrets set "EfDb:ConnectionString" "Server=localhost;Database=YourDatabase;User Id=sa;Password=YourPassword;Encrypt=False"
```

_If you want to test with Frontend [(like the Remix.JS frontend)](https://github.com/eduardoxcruz/GraphT.Remix.Js) you need to add CORS too_

```bash
	dotnet user-secrets set "Cors:Origins:0" "{frontend url}"
	#Examples
	dotnet user-secrets set "Cors:Origins:0" "http://localhost:3000"
	dotnet user-secrets set "Cors:Origins:1" "http://localhost:4200"
	dotnet user-secrets set "Cors:Origins:2" "http://localhost:5173"
```

5. _Verify your User Secrets configuration_

```bash
    dotnet user-secrets list
```

6. _[Apply migrations]_

```bash
	cd GraphT.EfCore.Repositories
	dotnet ef migrations add InitialMigration
	dotnet ef database update
```

7. _Run the Web API_

```bash
	cd ..
    cd GraphT.WebAPI
   	dotnet run -v q
```

---

## Running Tests ⚙️

### Unit Tests

Projects ending with ```.Tests``` are included as unit tests. All projects with unit tests use ```xUnit``` and ```NSubstitute``` for mocks.

Some of these projects are:

- _Model.Tests_
- _UseCases.Tests_
- _EfCore.Repositories.Tests_: Be careful, every time you run a test the ```TestDatabaseFixture``` calls next lines. [More info](https://learn.microsoft.com/en-us/ef/core/managing-schemas/ensure-created)

```csharp
using (EfDbContext context = CreateContext()){
	context.Database.EnsureDeleted();
	context.Database.EnsureCreated();
}
```

You can run the unit tests with your favorite IDE or code editor, or with the command ```dotnet test```

### REST API

You can test the Web API endpoints using ```Swagger``` or ```OpenAPI File```

#### Swagger

_With the Web Api running, open ```http://{url:port}/index.html``` in your browser_
- ```url``` and ```port``` are those you get after running ```dotnet run -v q```.

- An example URL would be ```http://localhost:5000/index.html```

#### OpenAPI Specification File

You can import the schema into your preferred HTTP tool such as [Postman](https://www.postman.com/) or [Bruno](https://github.com/usebruno/bruno)

1. _With the WebApi running, open ```http://{url:port}/swagger/v1/swagger.json``` in your browser_
2. _Click on ```Save```_
3. _Import the ```JSON``` file into ```Postman``` or ```Bruno``` as an ```OpenAPI Specification```_

---

## Deployment 📦

The Web API project includes default support for Azure Key Vault, but you can modify these lines to include any other platform where you want to deploy the project for personal use.

```csharp
GraphT.WebApi.Program.cs:

using Azure.Identity;

if (builder.Environment.IsProduction())
{
	string? keyVaultUri = builder.Configuration["AzureKeyVault:Uri"];
	
	if (!string.IsNullOrEmpty(keyVaultUri))
	{
		builder.Configuration.AddAzureKeyVault(
			new Uri(keyVaultUri),
			new DefaultAzureCredential()
		);
	}
}
```

---

## Built With 🛠️

* [.NET 8 Hexagonal Architecture Template](https://github.com/eduardoxcruz/HexagonalArchitecture.NET) – .NET 8 template for multi-platform projects following Clean & Hexagonal Architecture principles.
* [xUnit](https://xunit.net/) – Unit testing.
* [nSubstitute](https://nsubstitute.github.io/) – Mocks for unit tests.
* [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) – .NET Object Relational Mapper

---

## Versioning 📌

We use [SemVer](http://semver.org/) for versioning. For all available versions, see the [tags in this repository](https://github.com/eduardoxcruz/GraphT.NET/tags).

## Contributing 🖇️

Contributions are welcome! Please open issues and submit pull requests for new features, bug fixes, or improvements.

- We follow the [GitFlow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow)
- Remember to [write meaningful commits](https://cbea.ms/git-commit/)

## Wiki 📖

You can find much more on how to use this project in our [Wiki](https://github.com/eduardoxcruz/GraphT.NET/wiki)

## License 📄

This project is licensed under the MIT License – see the [LICENSE.md](./LICENSE.md) file for details

---

## Authors ✒️

You can see the list of all [contributors](https://github.com/eduardoxcruz/GraphT.NET/contributors) who have participated in this project.

## Expressions of Gratitude 🎁

* Tell others about this project 📢
* Buy a beer 🍺 or coffee ☕ for someone on the team.
* Give public thanks 🤓