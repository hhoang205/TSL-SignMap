const express = require('express');
const mongoose = require('mongoose');
const cors = require('cors');
const contributionRoutes = require('./routes/contributionRoutes');

const app = express();
app.use(express.json());
app.use(cors());

mongoose.connect('mongodb://mongo:27017/signmap_db', {
  useNewUrlParser: true,
  useUnifiedTopology: true
}).then(() => console.log('MongoDB connected for Contribution Service'))
  .catch(err => console.error(err));

app.use('/api/contributions', contributionRoutes);

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Contribution Service running on port ${PORT}`));
