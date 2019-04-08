package com.datadog.example.demo;

import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.Collections;
import java.util.List;
import java.util.Arrays;
import java.util.Random;
import java.util.concurrent.*;

import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.HttpClientBuilder;
import org.apache.http.util.EntityUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.MDC;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;

import datadog.trace.api.CorrelationIdentifier;
import datadog.trace.api.Trace;
import datadog.trace.api.interceptor.MutableSpan;
import datadog.trace.common.sampling.PrioritySampling;

import io.opentracing.Scope;
import io.opentracing.Span;
import io.opentracing.tag.Tags;
import io.opentracing.util.GlobalTracer;

@RestController
@RequestMapping("/coffeehouse")
public class CoffeeHouse {
    private final Random random = new Random(System.currentTimeMillis());

    static Logger log = LoggerFactory.getLogger(CoffeeHouse.class.getName());

    private final HttpClientBuilder builder = HttpClientBuilder.create();
    private final HttpClient client = builder.build();

    private final UserRepository repo;

    // customer_id 58325 must have higher probability
    private final List<Integer> customersIds = Arrays.asList(10217, 2855, 58325, 58325, 58325);
    private final ExecutorService executorService;

    public CoffeeHouse(UserRepository repo) {
        this.repo = repo;
        this.executorService = Executors.newFixedThreadPool(10);
    }

    @RequestMapping
    public @ResponseBody String coffeeHouseIndex() {
        // time the request
        long time = System.currentTimeMillis();

        // Assign the Customer ID
        int customerId = getCustomerId();
        MutableSpan requestSpan = (MutableSpan) GlobalTracer.get().activeSpan();
        if (requestSpan != null) {
            requestSpan.getRootSpan().setTag("customer_id", customerId);
        }

        Span span = GlobalTracer.get()
            .buildSpan("OpenTracingBrewInfo")
            .asChildOf(GlobalTracer.get().activeSpan())
            .start();
        Scope scope = GlobalTracer.get().scopeManager().activate(span, true);

        // Set priority sampling
        MutableSpan mspan = (MutableSpan) GlobalTracer.get().activeSpan();
        mspan.setSamplingPriority(PrioritySampling.USER_KEEP);

        // Ensure all logs have the trace ID bound to it
        MDC.put("dd.trace_id", String.valueOf(CorrelationIdentifier.getTraceId()));
        MDC.put("dd.span_id", String.valueOf(CorrelationIdentifier.getSpanId()));

        try {
            String result = this.coffeeHouseIndexImpl(customerId);
            time = System.currentTimeMillis() - time;
            log.info(String.format("GET http://java-coffeehouse:8080/coffeehouse completed with status code 200 in %d ms", time));
            return result;
        } finally {
            MDC.remove("dd.trace_id");
            scope.close();
        }
    }

    private int getCustomerId() {
        return this.customersIds.get(this.random.nextInt(this.customersIds.size()));
    }

    /**
     * @throws IOException
     * @throws URISyntaxException
     * @throws ExecutionException
     * @throws InterruptedException
     */
    private void callExternalWebServices() throws URISyntaxException, InterruptedException, ExecutionException {
        String phpEndpoint = EnvSettings.getPhpApiGatewayUrl() + "/coffees";
        String dotnetEndpoint = EnvSettings.getDotnetApiGatewayUrl() + "/users";

        Span span = GlobalTracer.get().activeSpan();

        // Add other demo calls here.
        Future<?> phpFuture = this.executorService.submit(new DemoRequest(span, new URI(phpEndpoint)));
        Future<?> dotnetFuture = this.executorService.submit(new DemoRequest(span, new URI(dotnetEndpoint)));
        phpFuture.get();
        dotnetFuture.get();
    }

    /**
     * Request to a demo app.
     */
    private class DemoRequest implements Runnable {

        private final Span span;
        private final URI uri;
        private final HttpClient client;

        DemoRequest(Span span, URI uri){
            this.span = span;
            this.uri = uri;
            this.client = HttpClientBuilder.create().build();
        }

        @Override
        public void run() {
            try (Scope scope = GlobalTracer.get().scopeManager().activate(span, false)) {
                this.client.execute(new HttpGet(uri));
            } catch (Throwable t) {
                Span span = GlobalTracer.get().activeSpan();
                Tags.ERROR.set(span, Boolean.TRUE);
                span.log(Collections.singletonMap("error.object", t));
            }
        }
    }

    private String coffeeHouseIndexImpl(int customerId) {
        Span span = GlobalTracer.get().activeSpan();

        // Get random user, as if he was doing the request.
        User user = this.getRandomUser();

        // Add user info to span
        span.setTag("custom.user.name", user.getName());
        span.setTag("custom.user.group", user.getGroupName());

        // Authentication step
        if (!this.authenticate(user)) {
            throw new IllegalStateException("Cannot authenticate user.");
        }

        log.info("GET /api/auth/ (10.8.4.7) - 200 OK - Authentication successful");

        // Authorization step
        if (!this.authorize(user)) {
            throw new IllegalStateException("User not allowed to order a hot brew.");
        }

        // Get the order
        log.info("Monitor thread successfully connected to server with description ServerDescription{address=mongodb:27017, type=STANDALONE, state=CONNECTED, ok=true, version=ServerVersion{versionList=[3, 4, 17]}, minWireVersion=0, maxWireVersion=5, maxDocumentSize=16777216, logicalSessionTimeoutMinutes=null, roundTripTimeNanos=1277564");
        getOrder(customerId);

        // Asynchronously record the order
        recordOrderAsync();

        // Prepare order. Raises if it fails.
        this.prepareOrder();

        return String.format("Your order is ready %s", user.getName());
    }

    @Trace
    private User getRandomUser() {
        final long s = System.currentTimeMillis();
        User usr = this.repo.getRandomUser();
        final long totalRequestTime = System.currentTimeMillis() - s;

        // TODO[vlad]: log interaction with repo

        return usr;
    }

    @Trace
    private boolean authenticate(User user) {
        if (!EnvSettings.getUseNodeService()) {
            // Fake successful authentication
            try {
                Thread.sleep(15);
            } catch (Exception e) {}
            return true;
        }

        try {
            String baseUrl = EnvSettings.getNodeApiGatewayUrl();
            final HttpResponse response = client.execute(new HttpGet(new URI(baseUrl + "/users")));
            final int code = response.getStatusLine().getStatusCode();

            EntityUtils.consume(response.getEntity());

            return code == 200;

        } catch (Throwable t) {
            Span span = GlobalTracer.get().activeSpan();
            Tags.ERROR.set(span, Boolean.TRUE);
            span.log(Collections.singletonMap("error.object", t));
        }

        return false;
    }

    @Trace
    private boolean authorize(User user) {
        // Fake successful authorization
        try {
            Thread.sleep(5);
        }  catch (Exception e) {}

        return true;
    }

    @Trace
    private Object getOrder(int customerId) {
        try {
            // customer_id 58325 must have higher duration
            if (customerId == 58325) {
                Thread.sleep(800);
            }

            callExternalWebServices();
            throw new InterruptedException("Thread interrupted for external calls timeout.");
        }  catch (Throwable t) {
            Span span = GlobalTracer.get().activeSpan();
            Tags.ERROR.set(span, Boolean.TRUE);
            span.log(Collections.singletonMap("error.object", t));
            log.error("java.lang.InterruptedException: Thread interrupted for external calls timeout - 500");
        }

        try {
            Thread.sleep(300);
            String baseUrl = EnvSettings.getBeanserverUrl();
            final HttpResponse response = client.execute(new HttpGet(new URI(baseUrl + "/beanserver")));
            final int code = response.getStatusLine().getStatusCode();

            EntityUtils.consume(response.getEntity());
            MongoCall.doMongoCall(10);
        } catch (Throwable t) {
            Span span = GlobalTracer.get().activeSpan();
            Tags.ERROR.set(span, Boolean.TRUE);
            span.log(Collections.singletonMap("error.object", t));
        }

        return null; // TODO: return an actual order
    }

    @Trace
    private void prepareOrder(/* TODO: take an order as parameter */) {
        final long s = System.currentTimeMillis();

        // Fake successful preparation
        try {
            Thread.sleep(30);
        }  catch (Exception e) {}

        final long totalPreparationTime = System.currentTimeMillis() - s;

        // TODO[vlad]: Log the order
    }

    public void recordOrderAsync(/* TODO: take an order as parameter */) {
        Span span = GlobalTracer.get()
            .buildSpan("RecordOrder")
            .asChildOf(GlobalTracer.get().activeSpan())
            .start();

        Thread t = new Thread() {
                @Override
                public void run() {
                    Scope scope = GlobalTracer.get().scopeManager().activate(span, true);

                    // Fake async operation
                    try {
                        Thread.sleep(40);
                    }  catch (Exception e) {}

                    scope.close();
                }
            };

        t.start();
    }
}
