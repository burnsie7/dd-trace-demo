'use strict'

const process = require('process')

const redis = require('redis')
const client = redis.createClient({ host: process.env.REDIS_HOST })

client.on('error', () => {})
client.on('ready', () => {
  client.set('jwks', '{}')
})

module.exports = client
