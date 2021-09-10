#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["microservices-task-2/microservices-task-2.csproj", "microservices-task-2/"]
RUN dotnet restore "microservices-task-2/microservices-task-2.csproj"
COPY . .
WORKDIR "/src/microservices-task-2"
RUN dotnet build "microservices-task-2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "microservices-task-2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "microservices-task-2.dll"]