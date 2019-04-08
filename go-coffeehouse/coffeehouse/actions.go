package coffeehouse

import (
	"fmt"
	"net/http"

	httptrace "gopkg.in/DataDog/dd-trace-go.v1/contrib/net/http"
)

func welcome(w http.ResponseWriter, r *http.Request) {
	fmt.Fprintf(w, "Welcome to the coffeehouse! What would you like to order today?")
}

func order(w http.ResponseWriter, r *http.Request) {
	orderName := r.URL.Path[len("/orders/"):]
	drink, ok := availableDrinks[orderName]
	if !ok {
		msg := fmt.Sprintf("We don't have anything like this. You can order:\n%s", DisplayMenu())
		http.Error(w, msg, 404)
		return
	}
	if r.Method == "POST" {
		postOrder(w, drink)
	} else if r.Method == "GET" {
		describeOrder(w, drink)
	} else {
		http.Error(w, fmt.Sprintf("Invalid Method %s", r.Method), 400)
	}
}

func postOrder(w http.ResponseWriter, d Drink) {
	// TODO actually do account update
	fmt.Fprintf(w, "You just ordered %s, removing %s from your account.\n", d.Name, d.Price)
}

func describeOrder(w http.ResponseWriter, d Drink) {
	fmt.Fprintf(w, "You'd like to order %s, it'll cost %s\n", d.Name, d.Price)
}

func RegisterActions(mux *httptrace.ServeMux) {
	mux.HandleFunc("/", welcome)
	mux.HandleFunc("/orders/", order)
}
