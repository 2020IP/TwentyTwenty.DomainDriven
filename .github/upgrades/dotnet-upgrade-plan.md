# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade src\TwentyTwenty.DomainDriven.MassTransit\TwentyTwenty.DomainDriven.MassTransit.csproj
4. Upgrade src\TwentyTwenty.DomainDriven.Marten\TwentyTwenty.DomainDriven.Marten.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

No projects are excluded from this upgrade.

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### src\TwentyTwenty.DomainDriven.MassTransit\TwentyTwenty.DomainDriven.MassTransit.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - All NuGet packages will be evaluated for .NET 10.0 compatibility and updated as needed during the upgrade process.

#### src\TwentyTwenty.DomainDriven.Marten\TwentyTwenty.DomainDriven.Marten.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - All NuGet packages will be evaluated for .NET 10.0 compatibility and updated as needed during the upgrade process.
