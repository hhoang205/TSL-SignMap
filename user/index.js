const express = require('express');
const bodyParser = require('body-parser');
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');

const app = express();
app.use(bodyParser.json());

let users = []; 
const SECRET = 'supersecret';

app.post('/register', (req, res) => {
  const { username, password } = req.body;
  if (users.find(u => u.username === username))
    return res.status(400).json({ msg: 'Tài khoản đã tồn tại' });
  
  const hashed = bcrypt.hashSync(password, 8);
  const newUser = { username, password: hashed, balance: 0, profile: {} };
  users.push(newUser);
  res.json({ msg: 'Đăng ký thành công', user: username });
});

app.post('/login', (req, res) => {
  const { username, password } = req.body;
  const user = users.find(u => u.username === username);
  if (!user || !bcrypt.compareSync(password, user.password))
    return res.status(401).json({ msg: 'Sai tài khoản hoặc mật khẩu' });

  const token = jwt.sign({ username }, SECRET, { expiresIn: '2h' });
  res.json({ msg: 'Đăng nhập thành công', token });
});

app.get('/profile', (req, res) => {
  const username = req.query.username;
  const user = users.find(u => u.username === username);
  if (!user) return res.status(404).json({ msg: 'Không tìm thấy người dùng' });
  res.json(user.profile);
});

app.post('/profile', (req, res) => {
  const { username, profile } = req.body;
  const user = users.find(u => u.username === username);
  if (!user) return res.status(404).json({ msg: 'Không tìm thấy người dùng' });
  user.profile = profile;
  res.json({ msg: 'Cập nhật hồ sơ thành công' });
});

app.post('/topup', (req, res) => {
  const { username, amount } = req.body;
  const user = users.find(u => u.username === username);
  if (!user) return res.status(404).json({ msg: 'Không tìm thấy người dùng' });
  user.balance += amount;
  res.json({ msg: `Đã nạp ${amount} xu`, balance: user.balance });
});

app.listen(4001, () => console.log('User Service chạy tại cổng 4001'));
