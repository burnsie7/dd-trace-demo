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

FROM ubuntu

RUN apt-get update && apt-get install -y curl

WORKDIR /usr/testrand

COPY --from=0 /usr/wrk/wrk wrk
COPY --from=0 /usr/wrk/scripts scripts

COPY request-generation/ request-generation

ENV JAVA_COFFEEHOUSE_ENDPOINT http://localhost:8080
ENV WRK /usr/testrand/wrk

CMD ["./request-generation/testrand.sh"]
