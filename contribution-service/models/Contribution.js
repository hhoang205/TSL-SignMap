const mongoose = require('mongoose');

const contributionSchema = new mongoose.Schema({
  title: String,
  description: String,
  imageUrl: String,
  status: { type: String, default: 'pending' }
});

module.exports = mongoose.model('Contribution', contributionSchema);
