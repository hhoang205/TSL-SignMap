const express = require('express');
const router = express.Router();
const Contribution = require('../models/Contribution');

router.get('/', async (req, res) => {
  const items = await Contribution.find();
  res.json(items);
});

router.get('/:id', async (req, res) => {
  const item = await Contribution.findById(req.params.id);
  if (!item) return res.status(404).send('Not found');
  res.json(item);
});

router.post('/', async (req, res) => {
  const item = new Contribution(req.body);
  await item.save();
  res.status(201).json(item);
});

router.put('/:id', async (req, res) => {
  const item = await Contribution.findByIdAndUpdate(req.params.id, req.body, { new: true });
  if (!item) return res.status(404).send('Not found');
  res.json(item);
});

router.delete('/:id', async (req, res) => {
  const result = await Contribution.findByIdAndDelete(req.params.id);
  if (!result) return res.status(404).send('Not found');
  res.json({ message: 'Deleted successfully' });
});

module.exports = router;
