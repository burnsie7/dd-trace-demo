'use strict'

const express = require('express')
const graphql = require('graphql')
const graphqlHTTP = require('express-graphql')
const userRepository = require('./user_repository')

const app = express()

const schema = graphql.buildSchema(`
  type Query {
    users: [User]
  }

  type User {
    name: String
    age: Int
  }
`)

const rootValue = {
  users: () => userRepository.all()
}

app.use('/graphql', graphqlHTTP({
  schema,
  rootValue,
  graphiql: true
}))
.get('/health', (req, res) => {
    res.status(200).send('OK')
})

module.exports = app
