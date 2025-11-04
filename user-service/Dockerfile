# Dockerfile for User Service (Node.js + MongoDB)
FROM node:18-alpine

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

EXPOSE 5001

CMD ["npm", "run", "dev"]
