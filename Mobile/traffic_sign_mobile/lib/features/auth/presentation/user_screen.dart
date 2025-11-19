import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

class UserScreen extends StatelessWidget {
  const UserScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final user = FirebaseAuth.instance.currentUser;

    if (user == null) {
      return Scaffold(
        appBar: AppBar(title: const Text("Thông tin cá nhân")),
        body: const Center(child: Text("Vui lòng đăng nhập để xem thông tin")),
      );
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text("Thông tin cá nhân"),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () async {
              await FirebaseAuth.instance.signOut();
              if (context.mounted) {
                context.go('/login'); // điều hướng về trang login
              }
            },
          )
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: StreamBuilder<DocumentSnapshot>(
          stream: FirebaseFirestore.instance
              .collection('users')
              .doc(user.uid)
              .snapshots(),
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

            final userData = snapshot.data!.data() as Map<String, dynamic>;
            final name = userData['name'] ?? 'User Name';
            final email = user.email ?? 'user@gmail.com';
            final createdAt = (userData['createdAt'] as Timestamp?)
                    ?.toDate()
                    .toString() ??
                '2025-01-01';

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
                const SizedBox(height: 30),

                // 🔥 Nút đăng xuất
                Center(
                  child: ElevatedButton.icon(
                    onPressed: () async {
                      await FirebaseAuth.instance.signOut();
                      if (context.mounted) {
                        context.go('/login'); // điều hướng về login
                      }
                    },
                    icon: const Icon(Icons.logout),
                    label: const Text("Đăng xuất"),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.redAccent,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(
                          horizontal: 20, vertical: 12),
                    ),
                  ),
                ),
              ],
            );
          },
        ),
      ),
    );
  }
}
