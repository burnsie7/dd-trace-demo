FROM golang:1.11.2

WORKDIR /go/src/github.com/DataDog/dd-trace-demo/go-coffeehouse/

RUN go get -d -v gopkg.in/DataDog/dd-trace-go.v1/ddtrace
RUN go get -d -v gopkg.in/DataDog/dd-trace-go.v1/contrib/net/http
COPY go-coffeehouse/ .

RUN CGO_ENABLED=0 GOOS=linux go build -a -o cmd/server/main cmd/server/main.go

FROM alpine:latest
RUN apk --no-cache add ca-certificates
WORKDIR /root/
COPY --from=0 /go/src/github.com/DataDog/dd-trace-demo/go-coffeehouse/cmd/server/main .
CMD ["./main"]
