# Demo app for APM -- aka coffeehouse

Generate traces, system metrics & logs for a demo account.


## Organization

The repository contains modules in several languages, that sometimes interact together, to
demonstrate Datadog's monitoring capabilities (distributed traces, ...).

For now you can find:
- The java app in [java-coffeehouse](java-coffeehouse/)
- The node app in [js-coffeehouse](js-coffeehouse/)
- The go app in [go-coffeehouse](go-coffeehouse/)
- The dotnet app in [dotnet-coffeehouse](dotnet-coffeehouse/)
- The php app in [go-coffeehouse](php-coffeehouse/)

The dockerfiles for all apps are in the [Dockerfiles](Dockerfiles/) directory.

Test scripts to generate traffic are located in [request-generation](request-generation/).

A docker-compose file contains the different services that need to be run:
- `*-coffeehouse-*` (`java-coffeehouse`, `js-coffeehouse-auth`, ...): a service to be monitored
- `agent`: the datadog agent
- Some databases: `mongo`, `redis`, ...
- `request-generator-testrand`: kind of a legacy service that generate traces continuously
(until some of its subprocess dies, probably).


## Development

You can run all the demo project in development with docker-compose.

You need a datadog account to send these metrics to. Create a trial account, get the API key.
You can then run the project:

```
DEMO_DD_API_KEY=<YOUR_TRIAL_API_KEY> DEMO_DD_ENV=<YOUR_DEMO_ENV> docker-compose up
```

It starts every services, and a cron container generating requests to these services.


## Add a new service

### Add the code

Create a directory `<service-name>/` and add the code under this directory.
Make sure your service has an endpoint `/_health` that will be used to monitor the health of your service.
It should return an HTTP 200 if the service is running fine.
You will be able to use this endpoint in the readinessProbe and the livenessProbe of the k8s deployment.

### Create the docker registries

Create a PR on cloudops to add the registry `dd-trace-demo-<service_name>` to each environment.

See this PR: [https://github.com/DataDog/cloudops/pull/9167/files](https://github.com/DataDog/cloudops/pull/9167/files)

Get it approved and applied from #cloudops, so the registries are created.

### Add the build definition of your service

Add a Dockerfile `Dockerfiles/<service_name>.Dockerfile` compatible with dev and prod

Add entries in [.gitlab-ci.yml](.gitlab-ci.yml) to build your service. You need one entry to build the container, one entry to deploy it to staging, and another one for deploying to prod.

Example for java:

```
# At the top of the file in the `variables:` definition:
  APP_JAVA: "dd-trace-demo-java"  # This is the name of your registry created in the cloudops PR

build-java:
  <<: *build-image-definition
  variables:
    APP: "$APP_JAVA"
    DOCKERFILE: Dockerfiles/java-coffeehouse.Dockerfile

release-java-to-staging:
  <<: *release-to-staging-definition
  variables:
    APP: "$APP_JAVA"

release-java-to-prod:
  <<: *release-to-prod-definition
  variables:
    APP: "$APP_JAVA"
```

With this addition to `.gitlab-ci.yml`, any change on the branch `staging` will trigger a container build & push on the staging registries, and any change on the branch `master` will trigger the same for production.

### Update the development environment to use your service

Add the service in `docker-compose.yml`.

Add a way to generate traces to your service: calls from another service, request generation with `testrand.sh`.


### Add k8s definitions (prod and staging deployments)

Under `k8s/dd-trace-demo/templates/`, add a service `<service_name>-service.yaml` and a deployment `<service_name>-deployment.yaml`.

Don't forget to bump the helm chart version in `k8s/dd-trace-demo/Chart.yaml` after updating anything in `k8s/dd-trace-demo/`.

Reach for help in case you don't know how to configure your deployment.

### Test it in staging

Push your branch to `staging` (merge `staging` into your branch before if needed). Once the CI pipeline has finished
you can run `latest-image staging --cloud aws dd-trace-demo-java` to get the latest image tag. You can then
deploy the app on staging with Spinnaker

### Deploy latest master on production

Use `latest-image` + Spinnaker.
