FROM microsoft/dotnet:sdk AS build-env

RUN apt-get update && apt-get install -y curl wget

WORKDIR /usr/src/

RUN wget -q https://github.com/DataDog/dd-trace-dotnet/releases/download/v1.0.0/datadog-dotnet-apm_1.0.0_amd64.deb -O ddnativeapm.deb \
  && dpkg -i ./ddnativeapm.deb

WORKDIR /usr/src/dotnet-coffeehouse

COPY dotnet-coffeehouse/ ./

RUN dotnet restore
RUN dotnet publish -c Release -o publish

FROM microsoft/dotnet:aspnetcore-runtime

RUN mkdir -p /var/log/datadog/

WORKDIR /usr/src/dotnet-coffeehouse

COPY --from=build-env /usr/src/dotnet-coffeehouse/Datadog.Coffeehouse.Api/publish .
COPY --from=build-env /opt/datadog /opt/datadog

EXPOSE 8084

ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_INTEGRATIONS=/opt/datadog/integrations.json
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DEMO_DD_API_KEY=${DEMO_DD_API_KEY}
ENV DD_AGENT_HOST=agent

ENTRYPOINT ["dotnet", "Datadog.Coffeehouse.Api.dll"]
