# Week09QualityLab

ENSE707 Week 9 Lab — TDD and QA in CI/CD.

```
Week09QualityLab.sln
Week09QualityLab.Core/     Class library (production code)
    DiscountCalculator.cs
    ParkingFeeCalculator.cs
Week09QualityLab.Tests/    MSTest project (automated tests)
    DiscountCalculatorTests.cs
    ParkingFeeCalculatorTests.cs
PipelineQualityNotes.md    Part 5 (CI/CD quality notes)
Week09Reflection.md        Reflection
.github/workflows/ci.yml   Example CI pipeline
```

## Run the tests
- **Visual Studio:** open `Week09QualityLab.sln`, then *Test > Run All Tests* (Test Explorer).
- **Command line:** `dotnet test`

Targets .NET 10 (Visual Studio 2026). MSTest is pinned to 3.6.4 because `Assert.ThrowsException`
(used in the lab) was removed in MSTest 4.
