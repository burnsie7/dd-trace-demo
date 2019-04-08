package com.datadog.example.demo;

import java.util.List;

import org.springframework.data.repository.CrudRepository;
import org.springframework.data.repository.query.Param;

public interface UserRepository extends CrudRepository<User, Long> {
  List<User> findByName(@Param("name") String name);

  User getRandomUser();
}
