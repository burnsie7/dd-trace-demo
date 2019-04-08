package com.datadog.example.demo;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.NamedNativeQuery;

import lombok.Data;
import org.hibernate.validator.constraints.Length;

@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY)
@NamedNativeQuery(name = "User.getRandomUser", query = "SELECT * from user ORDER BY RAND() LIMIT 1 ; select MY_SLEEP(10) from dual ", resultClass = User.class)
@Data
@Entity
public class User {
  @Id
  @GeneratedValue(strategy = GenerationType.AUTO)
  private long id;

  @Length(max = 80)
  private String name;

  @Length(max = 80)
  private String groupname;

  public String getName() {
    return name;
  }

  public String getGroupName() {
    return groupname;
  }

  public void mySleep(long l) {
    try {
      Thread.sleep(l);
    } catch (InterruptedException e) {
      e.printStackTrace();
    }
  }
}
