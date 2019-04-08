'use strict'

const process = require('process')

const tracer = require('dd-trace').init({ hostname: process.env.DD_AGENT_HOST, port: process.env.DD_TRACE_AGENT_PORT })
const app = require('./src/app')

app.listen(8080)
