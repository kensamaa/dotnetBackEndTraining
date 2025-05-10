# Create the solution folder
mkdir StudentCleanArch && cd StudentCleanArch

# Create a solution
dotnet new sln -n StudentCleanArch

# 1. Core library
dotnet new classlib -n Core
# 2. Infrastructure library
dotnet new classlib -n Infrastructure
# 3. Service library
dotnet new classlib -n Service
# 4. API project
dotnet new webapi -n Api

# Add projects to solution
dotnet sln add Core/Core.csproj
dotnet sln add Infrastructure/Infrastructure.csproj
dotnet sln add Service/Service.csproj
dotnet sln add Api/Api.csproj

# Infrastructure depends on Core
cd Infrastructure && dotnet add reference ../Core/Core.csproj && cd ..
# Service depends on Core and Infrastructure
cd Service \
  && dotnet add reference ../Core/Core.csproj \
  && dotnet add reference ../Infrastructure/Infrastructure.csproj \
  && cd ..
# API depends on Service
cd Api && dotnet add reference ../Service/Service.csproj && cd ..