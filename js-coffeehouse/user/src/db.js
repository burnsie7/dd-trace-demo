'use strict'

const process = require('process')
const retry = require('retry')
const MongoClient = require('mongodb').MongoClient

const ready = new Promise((resolve, reject) => {
  const operation = retry.operation({ forever: true })

  operation.attempt(() => {
    MongoClient.connect('mongodb://' + process.env.MONGO_HOST + ':27017', { useNewUrlParser: true }, (err, client) => {
      if (operation.retry(err)) return

      err ? reject(err) : resolve(client)
    })
  })
})

const db = {
  collection (name) {
    return ready.then(client => {
      const db = client.db('db')
      const col = db.collection(name)

      return col
    })
  }
}

module.exports = db
