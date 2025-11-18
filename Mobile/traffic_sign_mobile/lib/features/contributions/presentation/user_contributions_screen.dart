import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../application/user_contributions_controller.dart';

class UserContributionsScreen extends ConsumerWidget {
  const UserContributionsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final contributionsAsync = ref.watch(userContributionsControllerProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Đóng góp của tôi'),
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () => context.push('/home/contribution/new'),
          ),
        ],
      ),
      body: contributionsAsync.when(
        data: (items) {
          if (items.isEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Text('Bạn chưa có đóng góp nào.'),
                  const SizedBox(height: 12),
                  ElevatedButton(
                    onPressed: () => context.push('/home/contribution/new'),
                    child: const Text('Tạo đóng góp mới'),
                  ),
                ],
              ),
            );
          }

          final formatter = DateFormat('dd/MM/yyyy HH:mm');
          return RefreshIndicator(
            onRefresh: () => ref
                .read(userContributionsControllerProvider.notifier)
                .refresh(),
            child: ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: items.length,
              itemBuilder: (context, index) {
                final contribution = items[index];
                return Card(
                  margin: const EdgeInsets.only(bottom: 12),
                  child: ListTile(
                    title: Text(contribution.description),
                    subtitle: Text(
                      '${contribution.action} | ${contribution.status}\n${formatter.format(contribution.createdAt)}',
                    ),
                    trailing: const Icon(Icons.chevron_right),
                  ),
                );
              },
            ),
          );
        },
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, stack) => Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text('Không thể tải dữ liệu: $err'),
              const SizedBox(height: 8),
              ElevatedButton(
                onPressed: () => ref
                    .read(userContributionsControllerProvider.notifier)
                    .refresh(),
                child: const Text('Thử lại'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
