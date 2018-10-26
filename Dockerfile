FROM microsoft/dotnet:2.1-sdk AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production

COPY *.sln ./
COPY DecisionWebApi/*.csproj DecisionWebApi/
COPY DecisionWebApi.Test/*.csproj DecisionWebApi.Test/
RUN dotnet restore
COPY . .
WORKDIR /app/DecisionWebApi
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=base /app/DecisionWebApi/out .
ENTRYPOINT ["dotnet", "DecisionWebApi.dll"]