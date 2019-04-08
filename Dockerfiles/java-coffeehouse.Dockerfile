FROM openjdk:8-jdk
LABEL maintainer="Datadog Inc. <tyler@datadoghq.com>"

COPY java-coffeehouse/ /usr/src/java-coffeehouse
WORKDIR /usr/src/java-coffeehouse

# TODO: split into a dependency part and an install path to cache dependencies between builds
RUN ./gradlew installDist

CMD ./gradlew bootRun
