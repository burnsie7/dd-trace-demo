'use strict'

const axios = require('axios')
const express = require('express')
const process = require('process')

const app = express()

app.get('/users', (req, res) => {
  axios.get(process.env.AUTH_SERVER_URL + '/.well-known/jwks.json')
    .then(() => {
      return axios.post(process.env.USER_SERVER_URL + '/graphql', {
        query: `{ users { name age } }`
      }, {
        headers: { 'Content-Type': 'application/json' }
      })
    })
    .then(response => {
      res.status(200).send(response.data.data)
    })
    .catch(() => {
      res.status(502).send()
    })
})
.get('/health', (req, res) => {
    res.status(200).send('OK')
})

module.exports = app
