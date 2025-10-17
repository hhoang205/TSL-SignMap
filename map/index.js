const express = require('express');
const bodyParser = require('body-parser');
const app = express();
app.use(bodyParser.json());

let signs = [
  { id: 1, name: 'Cấm dừng đỗ', category: 'Cấm' },
  { id: 2, name: 'Giới hạn tốc độ 60', category: 'Giới hạn tốc độ' },
  { id: 3, name: 'Cấm rẽ trái', category: 'Cấm' }
];

// Xem tất cả biển báo
app.get('/signs', (req, res) => res.json(signs));

// Tìm kiếm biển báo
app.get('/search', (req, res) => {
  const { q } = req.query;
  const result = signs.filter(s => s.name.toLowerCase().includes(q.toLowerCase()));
  res.json(result);
});

app.listen(4002, () => console.log('Map Service chạy tại cổng 4002'));