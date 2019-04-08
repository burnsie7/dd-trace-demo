package com.datadog.example.demo;

import java.util.Random;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.servlet.view.RedirectView;

import datadog.trace.api.Trace;

@RestController
@RequestMapping("/beanserver")
public class BeanController {
  private final Random random = new Random(System.currentTimeMillis());

  @RequestMapping
  public RedirectView beanServerIndex() {
    String baseUrl = EnvSettings.getBeanserverUrl();
    return new RedirectView(baseUrl + "/beanserver/arabica");
  }

  @RequestMapping(value = "/arabica")
  public @ResponseBody String redirect() {
    beanServerImpl();
    return "Here's your bean: arabica";
  }

  @Trace
  public void beanServerImpl() {
      try {
          Thread.sleep(random.nextInt(5) + 5l);
          MongoCall.doMongoCall(random.nextInt(1) + 5l);
          Thread.sleep(random.nextInt(5) + 5l);
      } catch (InterruptedException e) {
          e.printStackTrace();
      }
  }
}
