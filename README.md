# DetectDuplicates [![NuGet Version](http://img.shields.io/nuget/v/DetectDuplicates.svg?style=flat)](https://www.nuget.org/packages/DetectDuplicates)

A .NET Global tool that scans csproj files for duplicate package references

### Installation 

Execute this from your terminal `dotnet tool install --global DetectDuplicates`

### Running the tool

To run the tool you can simply call `detectduplicates` from your terminal. This will use the current directory to look for `*.csproj` files.

To specify a path to analyse `*.csproj` or `*.fsproj` files you can call `detectduplicates /path/to/csproj`
