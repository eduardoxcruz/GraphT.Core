## Project Structure 🗂️

The solution is organized into several projects and folders, following Domain-Driven Design methodology along with 
Domain-Driven Design & Hexagonal Architecture. Below is a description of the main function of each folder/project:

- **SeedWork** – Base classes, utilities, and components shared among different projects (for example, 
the base `PagedList` class).

- **GraphT.Model** – Contains domain models, entities, value objects, domain services, and exceptions. Defines the core 
business logic and rules.

- **GraphT.UseCases** – Implements use cases and application logic. This is where the workflow between models and 
repositories is orchestrated.

---

## Getting Started 🚀

### Prerequisites 📋

- [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet)

### Installation 🔧

1. _Clone the repository_

```bash
   git clone https://github.com/eduardoxcruz/GraphT.Core.git
   cd GraphT.Core
```

2. _Restore dependencies_

```bash
   dotnet restore
```

3. _Build the solution_

```bash
    dotnet build
```

---

## Running Tests ⚙️

### Unit Tests

Projects ending with ```.Tests``` are included as unit tests. All projects with unit tests use ```xUnit``` and 
```NSubstitute``` for mocks.

Some of these projects are:

- _Model.Tests_
- _UseCases.Tests_

You can run the unit tests with your favorite IDE or code editor, or with the command ```dotnet test```

---

## Built With 🛠️

* [.NET 8 Hexagonal Architecture Template](https://github.com/eduardoxcruz/HexagonalArchitecture.NET) – .NET 8 template for multi-platform projects following Domain-Driven 
Desing & Hexagonal Architecture principles.
* [xUnit](https://xunit.net/) – Unit testing.
* [nSubstitute](https://nsubstitute.github.io/) – Mocks for unit tests.

---

## Versioning 📌

We use [SemVer](http://semver.org/) for versioning. For all available versions, see the [tags in this repository](https://github.com/eduardoxcruz/GraphT.Core/tags).

## Contributing 🖇️

Contributions are welcome! Please open issues and submit pull requests for new features, bug fixes, or improvements.

- We follow the [GitFlow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow)
- Remember to [write meaningful commits](https://cbea.ms/git-commit/)

## Wiki 📖

You can find much more on how to use this project in our [Wiki](https://github.com/eduardoxcruz/GraphT.Core/wiki)

## License 📄

This project is licensed under the MIT License – see the [LICENSE.md](../LICENSE.md) file for details

---

## Authors ✒️

You can see the list of all [contributors](https://github.com/eduardoxcruz/GraphT.Core/contributors) who have participated in this project.

## Expressions of Gratitude 🎁

* Tell others about this project 📢
* Buy a beer 🍺 or coffee ☕ for someone on the team.
* Give public thanks 🤓
