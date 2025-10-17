
const users = [
  { username: 'huy', balance: 100, isActive: true },
  { username: 'anh', balance: 50, isActive: true },
  { username: 'vien', balance: 0, isActive: false }
];

const feedbacks = [
  { id: 1, content: 'Ứng dụng chạy ổn định', status: 'approved' },
  { id: 2, content: 'Cần thêm tính năng lọc', status: 'pending' },
  { id: 3, content: 'Giao diện hơi rối', status: 'pending' }
];

const aiModels = [
  { id: 1, name: 'TrafficVision', version: '1.0', status: 'active' },
  { id: 2, name: 'RoadGuard', version: '0.9', status: 'inactive' }
];

module.exports = { users, feedbacks, aiModels };
