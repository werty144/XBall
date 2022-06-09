FROM gradle:7-jdk11 AS build
COPY --chown=gradle:gradle . /home/gradle/src
WORKDIR /home/gradle/src
RUN gradle shadowJar --no-daemon

FROM openjdk:11
EXPOSE 8080:8080
RUN mkdir /app
COPY --from=build /home/gradle/src/build/libs/example-0.0.1-all.jar /app/ktor-docker-sample.jar
COPY SteamworksWebApiKey /app
ENTRYPOINT ["java","-jar","/app/ktor-docker-sample.jar"]