package coffeehouse

import (
	"fmt"
	"strings"
)

type Price struct {
	Dollars int
	Cents   int
}

func (p Price) String() string {
	return fmt.Sprintf("$%d.%d", p.Dollars, p.Cents)
}

type Drink struct {
	Name  string
	Price Price
}

var drinkMenu = []Drink{
	{"espresso", Price{2, 99}},
	{"mocha", Price{3, 99}},
}

func getAvailableDrinks(drinkMenu []Drink) map[string]Drink {
	availableDrinks := make(map[string]Drink)
	for _, drink := range drinkMenu {
		availableDrinks[drink.Name] = drink
	}
	return availableDrinks
}

var availableDrinks map[string]Drink = getAvailableDrinks(drinkMenu)

func DisplayMenu() string {
	var b strings.Builder
	for _, drink := range availableDrinks {
		b.WriteString(fmt.Sprintf("%s: %s\n", drink.Name, drink.Price))
	}
	return b.String()
}
