FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY NuGet.Config /root/.nuget/NuGet/
ARG PAT=localhost
RUN sed -i "s|</configuration>|<packageSourceCredentials><Salzburgdev><add key=\"Username\" value=\"PAT\" /><add key=\"ClearTextPassword\" value=\"${PAT}\" /></Salzburgdev></packageSourceCredentials></configuration>|" /root/.nuget/NuGet/NuGet.Config
COPY . .
WORKDIR "/src/Osp/Backend/Ianitor.Osp.Backend.Identity"
RUN dotnet build "Ianitor.Osp.Backend.Identity.csproj" -c Release

FROM build AS publish
RUN dotnet publish "Ianitor.Osp.Backend.Identity.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ianitor.Osp.Backend.Identity.dll"]