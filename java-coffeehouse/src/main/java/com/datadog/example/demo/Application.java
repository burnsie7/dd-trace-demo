package com.datadog.example.demo;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication
public class Application {

  public static void main(String[] args) {
    MongoCall.doMongoCall(0);
    SpringApplication.run(Application.class, args);
  }
}
