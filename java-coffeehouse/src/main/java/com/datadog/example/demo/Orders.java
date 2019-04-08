package com.datadog.example.demo;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;

import java.util.Random;

@RestController
@RequestMapping("/orders")
public class Orders {
  private final Random random = new Random(System.currentTimeMillis());

  @RequestMapping("/cupcake")
  public @ResponseBody ResponseEntity cupcake() {
    if (random.nextInt(10) < 5) {
      return new ResponseEntity<>(HttpStatus.INTERNAL_SERVER_ERROR);
    }
    return ResponseEntity.ok("Here's your cupcake");
  }

  @RequestMapping("/muffin")
  public @ResponseBody String muffin() {
    return "Here's your muffin";
  }

  @RequestMapping("/juice")
  public @ResponseBody String juice() {
    return "Here's your juice";
  }

  @RequestMapping("/toast")
  public @ResponseBody String toast() {
    return "Here's your toast";
  }

  @RequestMapping("/eggs")
  public @ResponseBody String eggs() {
    return "Here's your eggs";
  }

  @RequestMapping("/pancakes")
  public @ResponseBody String pancakes() {
    return "Here's your pancakes";
  }

  @RequestMapping("/apples")
  public @ResponseBody String apples() {
    return "Here's your apples";
  }

  @RequestMapping("/grapes")
  public @ResponseBody String grapes() {
    return "Here's your grapes";
  }

  @RequestMapping("/omelette")
  public @ResponseBody String omelette() {
    return "Here's your omelette";
  }

  @RequestMapping("/milk")
  public @ResponseBody String milk() {
    return "Here's your milk";
  }

  @RequestMapping("/bacon")
  public @ResponseBody String bacon() {
    return "Here's your bacon";
  }

  @RequestMapping("/sausage")
  public @ResponseBody String sausage() {
    return "Here's your sausage";
  }

  @RequestMapping("/hashbrowns")
  public @ResponseBody String hashbrowns() {
    return "Here's your hashbrowns";
  }

  @RequestMapping("/cereal")
  public @ResponseBody String cereal() {
    return "Here's your cereal";
  }

  @RequestMapping("/frenchtoast")
  public @ResponseBody String frenchtoast() {
    return "Here's your frenchtoast";
  }

  @RequestMapping("/ham")
  public @ResponseBody String ham() {
    return "Here's your ham";
  }

  @RequestMapping("/doughnut")
  public @ResponseBody String doughnut() {
    return "Here's your doughnut";
  }

  @RequestMapping("/crepe")
  public @ResponseBody String crepe() {
    return "Here's your crepe";
  }
}
