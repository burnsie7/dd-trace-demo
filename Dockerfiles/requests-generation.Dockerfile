FROM ubuntu

WORKDIR /usr

RUN apt-get update && \
    apt-get install -y \
        build-essential \
        libssl-dev \
        git

RUN git clone https://github.com/wg/wrk.git wrk

WORKDIR /usr/wrk

RUN make

FROM renskiy/cron

WORKDIR /usr/cron

ARG CRONTAB_FILE="crontab"

COPY --from=0 /usr/wrk/wrk /usr/cron/wrk
COPY --from=0 /usr/wrk/scripts /usr/cron/wrk-scripts
COPY request-generation /usr/cron/request-generation

RUN ln -s /usr/cron/request-generation/${CRONTAB_FILE} /etc/cron.d/generate-requests
RUN chmod 0644 /etc/cron.d/generate-requests

ENV WRK /usr/cron/wrk
ENV JAVA_COFFEEHOUSE_ENDPOINT http://localhost:8080
ENV GO_COFFEEHOUSE_ENDPOINT http://localhost:8082
ENV DOTNET_COFFEEHOUSE_ENDPOINT http://localhost:8084
ENV DURATION 59s
