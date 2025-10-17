const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const { users, feedbacks, aiModels } = require('./data');

const app = express();
app.use(bodyParser.json());
app.use(cors());

app.get('/review/list', (req, res) => {
  const pending = feedbacks.filter(f => f.status === 'pending');
  res.json(pending);
});

app.post('/review/:id', (req, res) => {
  const id = parseInt(req.params.id);
  const { action } = req.body;
  const fb = feedbacks.find(f => f.id === id);
  if (!fb) return res.status(404).json({ msg: 'Không tìm thấy góp ý' });

  fb.status = action === 'approve' ? 'approved' : 'rejected';
  res.json({ msg: `Đã ${fb.status === 'approved' ? 'phê duyệt' : 'từ chối'} góp ý`, feedback: fb });
});

app.get('/users', (req, res) => {
  res.json(users);
});

app.post('/users/:username/topup', (req, res) => {
  const { amount } = req.body;
  const user = users.find(u => u.username === req.params.username);
  if (!user) return res.status(404).json({ msg: 'Không tìm thấy người dùng' });
  user.balance += amount;
  res.json({ msg: `Đã nạp ${amount} xu cho ${user.username}`, balance: user.balance });
});

app.post('/users/:username/ban', (req, res) => {
  const user = users.find(u => u.username === req.params.username);
  if (!user) return res.status(404).json({ msg: 'Không tìm thấy người dùng' });
  user.isActive = false;
  res.json({ msg: `Đã khóa tài khoản ${user.username}` });
});

app.get('/stats', (req, res) => {
  const totalUsers = users.length;
  const activeUsers = users.filter(u => u.isActive).length;
  const totalFeedbacks = feedbacks.length;
  const pendingFeedbacks = feedbacks.filter(f => f.status === 'pending').length;
  const totalCoins = users.reduce((sum, u) => sum + u.balance, 0);

  res.json({
    totalUsers,
    activeUsers,
    totalFeedbacks,
    pendingFeedbacks,
    totalCoins
  });
});

app.get('/ai/list', (req, res) => {
  res.json(aiModels);
});

app.post('/ai/add', (req, res) => {
  const { name, version } = req.body;
  const newModel = { id: aiModels.length + 1, name, version, status: 'inactive' };
  aiModels.push(newModel);
  res.json({ msg: 'Đã thêm mô hình AI', model: newModel });
});

app.post('/ai/:id/activate', (req, res) => {
  const id = parseInt(req.params.id);
  const model = aiModels.find(m => m.id === id);
  if (!model) return res.status(404).json({ msg: 'Không tìm thấy mô hình' });
  model.status = 'active';
  res.json({ msg: `Đã kích hoạt mô hình ${model.name}` });
});

app.delete('/ai/:id', (req, res) => {
  const id = parseInt(req.params.id);
  const index = aiModels.findIndex(m => m.id === id);
  if (index === -1) return res.status(404).json({ msg: 'Không tìm thấy mô hình' });
  aiModels.splice(index, 1);
  res.json({ msg: 'Đã xóa mô hình AI' });
});

app.listen(4004, () => console.log('Admin Service chạy tại cổng 4004'));
