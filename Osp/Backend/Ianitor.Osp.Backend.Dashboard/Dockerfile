FROM salzburgdev.azurecr.io/dotnet_aspnetcore-runtime-node-3.1.0:latest AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM salzburgdev.azurecr.io/dotnet_sdk-node-3.1:latest AS build
WORKDIR /src
COPY NuGet.Config /root/.nuget/NuGet/
ARG PAT=localhost
RUN sed -i "s|</configuration>|<packageSourceCredentials><Salzburgdev><add key=\"Username\" value=\"PAT\" /><add key=\"ClearTextPassword\" value=\"${PAT}\" /></Salzburgdev></packageSourceCredentials></configuration>|" /root/.nuget/NuGet/NuGet.Config
COPY . .
WORKDIR "/src/Osp/Backend/Ianitor.Osp.Backend.Dashboard"
RUN dotnet build "Ianitor.Osp.Backend.Dashboard.csproj" -c Release

FROM build AS publish
RUN dotnet publish "Ianitor.Osp.Backend.Dashboard.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ianitor.Osp.Backend.Dashboard.dll"]