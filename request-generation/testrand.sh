#!/bin/bash

function do_orders {
  for i in `seq 1 10`; do
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/cupcake >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/muffin >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/juice >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/toast >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/eggs >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/pancakes >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/apples >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/grapes >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/omelette >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/milk >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/bacon >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/sausage >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/hashbrowns >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/cereal >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/frenchtoast >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/ham >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/doughnut >/dev/null 2>&1
      curl $JAVA_COFFEEHOUSE_ENDPOINT/orders/crepe >/dev/null 2>&1
  done
}

while true; do
  do_orders &
  num_connections=$(grep -m1 -ao '[5-9]' /dev/urandom | sed s/0/10/ | head -n1)
  $WRK -t $num_connections -c $num_connections -d 30s $JAVA_COFFEEHOUSE_ENDPOINT/coffeehouse
  wait
done
