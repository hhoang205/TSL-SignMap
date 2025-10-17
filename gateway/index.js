const express = require('express');
const axios = require('axios');
const bodyParser = require('body-parser');
const cors = require('cors');

const app = express();
app.use(bodyParser.json());
app.use(cors());

const services = {
  user: 'http://localhost:4001',
  map: 'http://localhost:4002',
  feedback: 'http://localhost:4003',
  admin: 'http://localhost:4004'
};

// USER
app.use('/user', (req, res) => {
  const url = services.user + req.path.replace('/user', '');
  axios({ method: req.method, url, data: req.body })
    .then(r => res.json(r.data))
    .catch(e => res.status(500).json({ msg: e.message }));
});

// MAP
app.use('/map', (req, res) => {
  const url = services.map + req.path.replace('/map', '');
  axios({ method: req.method, url, data: req.body })
    .then(r => res.json(r.data))
    .catch(e => res.status(500).json({ msg: e.message }));
});

// FEEDBACK
app.use('/feedback', (req, res) => {
  const url = services.feedback + req.path.replace('/feedback', '');
  axios({ method: req.method, url, data: req.body })
    .then(r => res.json(r.data))
    .catch(e => res.status(500).json({ msg: e.message }));
});

// ADMIN
app.use('/admin', (req, res) => {
  const url = services.admin + req.path.replace('/admin', '');
  axios({ method: req.method, url, data: req.body })
    .then(r => res.json(r.data))
    .catch(e => res.status(500).json({ msg: e.message }));
});

app.listen(3000, () => console.log('Gateway chạy tại cổng 3000'));
