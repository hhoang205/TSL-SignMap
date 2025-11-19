import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/material.dart';

class UserScreen extends StatelessWidget {
  const UserScreen({super.key});

  @override
  Widget build(BuildContext context) {
    // Lấy user hiện tại từ Firebase Authentication
    final user = FirebaseAuth.instance.currentUser;

    if (user == null) {
      // Nếu chưa đăng nhập, có thể redirect hoặc hiển thị thông báo
      return Scaffold(
        appBar: AppBar(title: const Text("Thông tin cá nhân")),
        body: const Center(child: Text("Vui lòng đăng nhập để xem thông tin")),
      );
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text("Thông tin cá nhân"),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: StreamBuilder<DocumentSnapshot>(
          // Lấy dữ liệu user từ Firestore (giả sử collection 'users' với doc ID là uid)
          stream: FirebaseFirestore.instance.collection('users').doc(user.uid).snapshots(),
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return const Center(child: CircularProgressIndicator());
            }
            if (snapshot.hasError) {
              return const Center(child: Text("Lỗi khi tải dữ liệu"));
            }
            if (!snapshot.hasData || !snapshot.data!.exists) {
              return const Center(child: Text("Không tìm thấy thông tin user"));
            }

            // Lấy dữ liệu từ snapshot
            final userData = snapshot.data!.data() as Map<String, dynamic>;
            final name = userData['name'] ?? 'User Name'; // Giá trị mặc định nếu null
            final email = user.email ?? 'user@gmail.com';
            final createdAt = (userData['createdAt'] as Timestamp?)?.toDate().toString() ?? '2025-01-01';

            return Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const CircleAvatar(
                  radius: 40,
                  child: Icon(Icons.person, size: 40),
                ),
                const SizedBox(height: 20),
                Text("Tên: $name"),
                Text("Email: $email"),
                Text("Ngày tạo tài khoản: $createdAt"),
                const SizedBox(height: 20),
                const Text("Cài đặt tài khoản..."),
              ],
            );
          },
        ),
      ),
    );
  }
}