#!/bin/bash
# Generate requests
# Use: ./generate-requests.sh <WRK_PATH> <DURATION> <TARGET_URL>
WRK=$1
DURATION=$2
TARGET_URL=$3

var1=$RANDOM
var2=$[5 + $var1 % (9 + 1 - 5)]

THREADS="${THREADS:-$var2}"
CONNECTIONS="${CONNECTIONS:-$var2}"

$WRK --threads $THREADS --connections $CONNECTIONS --duration $DURATION $TARGET_URL
