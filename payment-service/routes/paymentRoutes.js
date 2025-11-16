const express = require('express');
const router = express.Router();
const Payment = require('../models/Payment');

router.get('/', async (req, res) => {
  const payments = await Payment.find();
  res.json(payments);
});

router.get('/:id', async (req, res) => {
  const payment = await Payment.findById(req.params.id);
  if (!payment) return res.status(404).send('Not found');
  res.json(payment);
});

router.post('/', async (req, res) => {
  const payment = new Payment(req.body);
  await payment.save();
  res.status(201).json(payment);
});

router.put('/:id', async (req, res) => {
  const payment = await Payment.findByIdAndUpdate(req.params.id, req.body, { new: true });
  if (!payment) return res.status(404).send('Not found');
  res.json(payment);
});

router.delete('/:id', async (req, res) => {
  const result = await Payment.findByIdAndDelete(req.params.id);
  if (!result) return res.status(404).send('Not found');
  res.json({ message: 'Deleted successfully' });
});

module.exports = router;
