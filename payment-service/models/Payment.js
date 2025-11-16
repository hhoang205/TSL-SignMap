const mongoose = require('mongoose');

const paymentSchema = new mongoose.Schema({
  userId: String,
  amount: Number,
  status: { type: String, default: 'pending' },
  transactionId: String
});

module.exports = mongoose.model('Payment', paymentSchema);
