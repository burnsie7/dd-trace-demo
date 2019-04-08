package com.datadog.example.demo;

import java.lang.Integer;

public class EnvSettings {
  private static String defaultBeanserverUrl = "http://java-coffeehouse-beanserver:8080";
  private static String defaultNodeGatewayApiUrl = "http://js-coffeehouse-api-gateway:8080";
  private static String defaultPhpGatewayApiUrl = "http://php-coffeehouse-frontend";
  private static String defaultDotnetGatewayApiUrl = "http://dotnet-coffeehouse:8084";

  private static String defaultMongoHost = "mongo";
  private static int defaultMongoPort = 27017;
  private static boolean defaultUseNodeService = true;

  public static String getBeanserverUrl() {
    String url = System.getenv("BEANSERVER_URL");
    if (url == null) {
      return defaultBeanserverUrl;
    }
    return url;
  }

  public static String getNodeApiGatewayUrl() {
    String url = System.getenv("NODE_API_GATEWAY_URL");
    if (url == null) {
      return defaultNodeGatewayApiUrl;
    }
    return url;
  }


  public static String getPhpApiGatewayUrl() {
    String url = System.getenv("PHP_API_GATEWAY_URL");
    if (url == null) {
      return defaultPhpGatewayApiUrl;
    }
    return url;
  }


  public static String getDotnetApiGatewayUrl() {
    String url = System.getenv("DOTNET_API_GATEWAY_URL");
    if (url == null) {
      return defaultDotnetGatewayApiUrl;
    }
    return url;
  }

  public static boolean getUseNodeService() {
    String useNodeService = System.getenv("USE_NODE_SERVICE");
    if (useNodeService == null) {
      return defaultUseNodeService;
    }
    return useNodeService.equals("true");
  }

  public static String getMongoHost() {
    String host = System.getenv("MONGO_HOST");
    if (host == null) {
      return defaultMongoHost;
    }
    return host;
  }

  public static int getMongoPort() {
    String port = System.getenv("MONGO_PORT");
    if (port == null) {
      return defaultMongoPort;
    }
    return Integer.parseInt(port);
  }

}
