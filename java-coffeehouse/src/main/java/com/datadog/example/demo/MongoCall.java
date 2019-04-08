package com.datadog.example.demo;

import com.mongodb.BasicDBObject;
import com.mongodb.MongoClient;
import com.mongodb.client.MongoCollection;
import com.mongodb.client.MongoDatabase;

import org.bson.Document;

public class MongoCall {
  public static final String MONGO_DB_NAME = "demo";

  private static MongoClient client;
  private static final String collectionName = "testCollection";

  static {
    try {
      client = new MongoClient(EnvSettings.getMongoHost(), EnvSettings.getMongoPort());
    } catch (Exception e) {
      throw new RuntimeException(e);
    }
  }

  public static void doMongoCall(final long queryTimeMS) {
    final MongoDatabase db = client.getDatabase(MONGO_DB_NAME);
    final BasicDBObject command = new BasicDBObject("$eval", "sleep("+queryTimeMS+")").append("find", new BasicDBObject("group", "admin"));
    db.runCommand(command);
  }

}
