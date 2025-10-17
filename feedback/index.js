const express = require('express');
const bodyParser = require('body-parser');
const app = express();
app.use(bodyParser.json());

let feedbacks = [];
let votes = [];
let notifications = [];

app.post('/contribute', (req, res) => {
  const { username, content } = req.body;
  feedbacks.push({ username, content, date: new Date() });
  res.json({ msg: 'Góp ý thành công' });
});

app.post('/vote', (req, res) => {
  const { username, contributionId, value } = req.body;
  votes.push({ username, contributionId, value });
  res.json({ msg: 'Bỏ phiếu thành công' });
});

app.post('/notify', (req, res) => {
  const { message } = req.body;
  notifications.push({ message, date: new Date() });
  res.json({ msg: 'Thông báo đã được gửi' });
});

app.post('/feedback', (req, res) => {
  const { username, report } = req.body;
  feedbacks.push({ username, report, date: new Date() });
  res.json({ msg: 'Đã gửi phản hồi / báo cáo lỗi' });
});

app.listen(4003, () => console.log('Feedback Service chạy tại cổng 4003'));
