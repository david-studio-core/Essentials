# DavidStudio.Core.Essentials

A productivity library that accelerates .NET project scaffolding by providing ready-to-use features for
common application needs. Includes data I/O helpers and abstractions, authentication utilities,
OpenAPI/Swagger integration features, standardized result/response types, pagination helpers, and general-purpose
utilities.

> ⚠️ Alpha Release:
This package is currently in alpha. Development is ongoing, and contributions are welcome!

---

## 🚀 Getting Started

### Install NuGet Package

Using the .NET CLI:
```bash
dotnet add package DavidStudio.Core.Essentials
```

Or via the Package Manager Console:
```bash
Install-Package DavidStudio.Core.Essentials
```

### How to use it?

Feel free to explore the [samples](https://github.com/david-studio-core/Essentials/tree/main/samples) to find practical examples for each feature.
New samples are added continuously as more features are developed.

## 📦 Key Features

* [DataIO](https://github.com/david-studio-core/Essentials/tree/main/src/DavidStudio.Core.DataIO) - A comprehensive toolkit that streamlines data operations with Entity Framework Core, ADO.NET, and providers for Elasticsearch, RabbitMQ, Azure ServiceBus, Redis, and RedLock. Includes multiple design patterns and abstractions that reduce boilerplate and simplify your daily development workflow.  

* [Auth](https://github.com/david-studio-core/Essentials/tree/main/src/DavidStudio.Core.Auth) - Essential Authentication and Authorization utilities for ASP.NET Core, featuring powerful permission-based authorization for JWT, seamless Bearer token support, Swagger authentication configuration, convenient claim helpers, and user-session tools. 

* [Swagger](https://github.com/david-studio-core/Essentials/tree/main/src/DavidStudio.Core.Swagger) - Plug-and-play extensions for adding Swagger with sensible defaults. Includes built-in support for StronglyTypedIds, Bearer authentication, API versioning, and controller ordering.

* [Results](https://github.com/david-studio-core/Essentials/tree/main/src/DavidStudio.Core.Results) - Clean, standardized result and response models for APIs and services. Helps enforce consistent response patterns and reduce repetitive code.

* [Pagination](https://github.com/david-studio-core/Essentials/tree/main/src/DavidStudio.Core.Pagination) - Robust cursor-based and offset-based pagination models and utilities designed for high-performance, scalable APIs.

* [Utilities](https://github.com/david-studio-core/Essentials/tree/main/src/DavidStudio.Core.Utilities) - A growing collection of handy helpers and extensions to speed up everyday .NET development.

## 🚀 What I'm Most Proud Of

One of the standout features of *DavidStudio.Core.Essentials* is its high-performance dynamic ordering query builder from string for LINQ and Entity Framework Core.
It delivers over **2× faster** execution and cuts memory allocations in half compared to *System.Linq.Dynamic.Core*, making it ideal for high-load APIs and advanced pagination scenarios.

```
BenchmarkDotNet v0.15.6, macOS 26.1 (25B78) [Darwin 25.1.0]
Apple M4 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
```
| Method                                      | Mean      | Error     | StdDev    | Rank | Gen0   | Gen1   | Allocated |
|---------------------------------------------|----------:|----------:|----------:|-----:|-------:|-------:|----------:|
| Essentials_DynamicOrdering_CursorPagination |  5.193 μs | 0.0522 μs | 0.0463 μs |    2 | 1.3428 |      - |  10.99 KB |
| Essentials_DynamicOrdering_OffsetPagination |  9.278 μs | 0.0665 μs | 0.0589 μs |    4 | 2.1973 |      - |  18.29 KB |
| LINQ_DynamicOrdering_CursorPagination       |  9.847 μs | 0.0692 μs | 0.0613 μs |    5 | 3.1738 | 0.0610 |  26.06 KB |
| LINQ_DynamicOrdering_OffsetPagination       | 13.725 μs | 0.0969 μs | 0.0906 μs |    6 | 3.9063 |      - |  32.73 KB |


## 🤝 Contributing

Found a bug? Have an idea? Want to contribute?

* Submit an issue:
https://github.com/david-studio-core/Essentials/issues
* Create a pull request:
https://github.com/david-studio-core/Essentials/pulls

Contributions of any size are appreciated!

## 📝 License

Distributed under the **MIT license**. See [License](https://github.com/david-studio-core/Essentials/blob/main/LICENSE.txt) for more information.

Copyright © 2025 David Khachatryan (David Studio)
