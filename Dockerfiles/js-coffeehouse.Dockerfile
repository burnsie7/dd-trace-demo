FROM node:8-alpine

RUN apk add --no-cache tini

ARG SOURCE_DIR=js-coffeehouse/api-gateway

ENV NODE_ENV=production
ENV DD_TRACE_AGENT_PORT 8126

WORKDIR /usr/src/app

COPY ${SOURCE_DIR}/package.json ${SOURCE_DIR}/yarn.lock /usr/src/app/
RUN yarn --pure-lockfile && yarn cache clean

COPY ${SOURCE_DIR}/src /usr/src/app/src/
COPY ${SOURCE_DIR}/server.js /usr/src/app/

EXPOSE 8080
USER node

ENTRYPOINT ["/sbin/tini", "--"]
CMD [ "npm", "start" ]
