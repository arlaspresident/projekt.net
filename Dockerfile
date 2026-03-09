FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Courses.Web/Courses.Web.csproj", "Courses.Web/"]
RUN dotnet restore "Courses.Web/Courses.Web.csproj"
COPY . .
WORKDIR "/src/Courses.Web"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Courses.Web.dll"]
