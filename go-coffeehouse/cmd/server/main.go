package main

import (
	"fmt"
	"log"
	"net/http"
	"os"

	httptrace "gopkg.in/DataDog/dd-trace-go.v1/contrib/net/http"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"

	"github.com/DataDog/dd-trace-demo/go-coffeehouse/coffeehouse"
)

const defaultPort = "8080"
const defaultAgentPort = "8126"
const defaultService = "coffee-house-go"

func main() {
	if agentHost, ok := os.LookupEnv("DD_AGENT_HOST"); ok { // Custom agent host
		var agentPort string
		if envAgentPort, ok := os.LookupEnv("DD_TRACE_AGENT_PORT"); ok {
			agentPort = envAgentPort
		} else {
			agentPort = defaultAgentPort
		}
		agentAddr := fmt.Sprintf("%s:%s", agentHost, agentPort)
		tracer.Start(tracer.WithAgentAddr(agentAddr), tracer.WithServiceName(defaultService))
	} else {
		tracer.Start()
	}
	defer tracer.Stop()
	port := os.Getenv("PORT")
	if port == "" {
		port = defaultPort
	}
	mux := httptrace.NewServeMux(httptrace.WithServiceName(defaultService))
	coffeehouse.RegisterActions(mux)
	log.Fatal(http.ListenAndServe(fmt.Sprintf("0.0.0.0:%s", port), mux))
}
