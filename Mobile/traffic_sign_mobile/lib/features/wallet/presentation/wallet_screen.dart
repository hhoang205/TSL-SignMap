import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../auth/application/auth_controller.dart';
import '../application/wallet_controller.dart';

class WalletScreen extends ConsumerWidget {
  const WalletScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final walletAsync = ref.watch(walletControllerProvider);
    final formatter = NumberFormat.currency(locale: 'vi_VN', symbol: '₫');
    final user = ref.watch(authControllerProvider).user;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Ví coin'),
        actions: [
          IconButton(
            onPressed: () =>
                ref.read(walletControllerProvider.notifier).refresh(),
            icon: const Icon(Icons.refresh),
          ),
        ],
      ),
      body: walletAsync.when(
        data: (balance) {
          if (balance == null) {
            return const Center(child: Text('Không có dữ liệu ví.'));
          }

          return Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Card(
                  child: Padding(
                    padding: const EdgeInsets.all(24),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Xin chào, ${user?.username ?? ''}',
                          style: Theme.of(context).textTheme.titleMedium,
                        ),
                        const SizedBox(height: 12),
                        Text(
                          'Số dư hiện tại',
                          style: Theme.of(context).textTheme.labelLarge,
                        ),
                        Text(
                          '${balance.balance.toStringAsFixed(1)} coins',
                          style: Theme.of(context).textTheme.displaySmall,
                        ),
                      ],
                    ),
                  ),
                ),
                const SizedBox(height: 24),
                ElevatedButton(
                  onPressed: () {
                    ScaffoldMessenger.of(context).showSnackBar(
                      SnackBar(
                        content: Text(
                          'Tính năng nạp coin sẽ tích hợp với cổng thanh toán trong giai đoạn tiếp theo.\nTạm thời bạn có thể liên hệ admin.',
                        ),
                      ),
                    );
                  },
                  child: const Text('Nạp thêm coin'),
                ),
                const SizedBox(height: 12),
                Text(
                  'Quy đổi tham khảo: 1 USD ≈ ${formatter.format(24000)} cho 10 coins',
                  style: Theme.of(context).textTheme.bodySmall,
                ),
              ],
            ),
          );
        },
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, stack) => Center(child: Text('Không thể tải ví: $err')),
      ),
    );
  }
}
