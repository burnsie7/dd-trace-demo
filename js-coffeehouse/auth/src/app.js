'use strict'

const express = require('express')
const cache = require('./cache')

const app = express()

app.get('/.well-known/jwks.json', (req, res) => {
  cache.get('jwks', (err, value) => {
    if (err) {
      return res.status(500).send()
    }

    res.status(200).send(value)
  })
})
.get('/health', (req, res) => {
    res.status(200).send('OK')
})

module.exports = app
