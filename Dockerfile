FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

RUN apt-get update && apt-get upgrade -y

COPY *.sln .
COPY NotesAPI/*.csproj .
COPY UI/*.csproj .

COPY . .

RUN dotnet restore *.sln

WORKDIR /src/NotesAPI

RUN dotnet build -c Release -o /app/build

WORKDIR /src/UI

RUN dotnet build -c Release -o /app/build



FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

RUN apt-get update && apt-get upgrade -y

COPY --from=build /app/build .

EXPOSE 7165 5230 7202 5087

ENTRYPOINT ["dotnet", "NotesAPI.dll", "&&", "dotnet", "UI.dll"]
