import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../../auth/application/auth_controller.dart';
import '../../auth/presentation/user_profile_screen.dart';
import '../application/transaction_controller.dart';
import '../application/wallet_controller.dart';
import '../data/wallet_transaction.dart';

class WalletScreen extends ConsumerWidget {
  const WalletScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final walletAsync = ref.watch(walletControllerProvider);
    final formatter = NumberFormat.currency(locale: 'vi_VN', symbol: '₫');
    final user = ref.watch(authControllerProvider).user;

    return Scaffold(
      body: walletAsync.when(
        data: (balance) {
          if (balance == null) {
            return const Center(child: Text('Không có dữ liệu ví.'));
          }

          return Column(
            children: [
              Padding(
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
              ),
              const Divider(),
              Expanded(
                child: _TransactionHistoryList(userId: user?.id ?? 0),
              ),
            ],
          );
        },
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, stack) => Center(child: Text('Không thể tải ví: $err')),
      ),
    );
  }
}

class _TransactionHistoryList extends ConsumerWidget {
  const _TransactionHistoryList({required this.userId});

  final int userId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final transactionsAsync = ref.watch(transactionControllerProvider);

    return transactionsAsync.when(
      data: (transactions) {
        if (transactions.isEmpty) {
          return Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(
                  Icons.history,
                  size: 64,
                  color: Theme.of(context).colorScheme.outline,
                ),
                const SizedBox(height: 16),
                Text(
                  'Chưa có giao dịch nào',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        color: Theme.of(context).colorScheme.outline,
                      ),
                ),
                const SizedBox(height: 8),
                Text(
                  'Lịch sử giao dịch sẽ hiển thị ở đây',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Theme.of(context).colorScheme.outline,
                      ),
                ),
              ],
            ),
          );
        }

        return RefreshIndicator(
          onRefresh: () async {
            await ref.read(transactionControllerProvider.notifier).refresh();
          },
          child: ListView.builder(
            padding: const EdgeInsets.all(16),
            itemCount: transactions.length,
            itemBuilder: (context, index) {
              final transaction = transactions[index];
              return _TransactionItem(transaction: transaction);
            },
          ),
        );
      },
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, stack) => Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.error_outline,
              size: 64,
              color: Theme.of(context).colorScheme.error,
            ),
            const SizedBox(height: 16),
            Text(
              'Không thể tải lịch sử giao dịch',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 8),
            Text(
              err.toString(),
              style: Theme.of(context).textTheme.bodySmall,
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: () {
                ref.read(transactionControllerProvider.notifier).refresh();
              },
              icon: const Icon(Icons.refresh),
              label: const Text('Thử lại'),
            ),
          ],
        ),
      ),
    );
  }
}

class _TransactionItem extends StatelessWidget {
  const _TransactionItem({required this.transaction});

  final WalletTransaction transaction;

  @override
  Widget build(BuildContext context) {
    final isCredit = transaction.isCredit;
    final color = isCredit
        ? Theme.of(context).colorScheme.primary
        : Theme.of(context).colorScheme.error;

    IconData icon;
    switch (transaction.type) {
      case TransactionType.credit:
      case TransactionType.payment:
        icon = Icons.add_circle_outline;
        break;
      case TransactionType.debit:
        icon = Icons.remove_circle_outline;
        break;
      case TransactionType.contribution:
        icon = Icons.location_on;
        break;
      case TransactionType.voting:
        icon = Icons.how_to_vote;
        break;
      case TransactionType.adjustment:
        icon = Icons.tune;
        break;
      default:
        icon = Icons.swap_horiz;
    }

    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: color.withOpacity(0.1),
          child: Icon(icon, color: color),
        ),
        title: Text(
          transaction.description.isNotEmpty
              ? transaction.description
              : _getDefaultDescription(transaction.type),
          style: Theme.of(context).textTheme.bodyLarge,
        ),
        subtitle: Text(
          transaction.formattedDate,
          style: Theme.of(context).textTheme.bodySmall,
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Text(
              transaction.formattedAmount,
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    color: color,
                    fontWeight: FontWeight.bold,
                  ),
            ),
            if (transaction.status != TransactionStatus.completed)
              Container(
                margin: const EdgeInsets.only(top: 4),
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                decoration: BoxDecoration(
                  color: _getStatusColor(context, transaction.status).withOpacity(0.1),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Text(
                  _getStatusText(transaction.status),
                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                        color: _getStatusColor(context, transaction.status),
                        fontSize: 10,
                      ),
                ),
              ),
          ],
        ),
      ),
    );
  }

  String _getDefaultDescription(TransactionType type) {
    switch (type) {
      case TransactionType.credit:
        return 'Nhận coin';
      case TransactionType.debit:
        return 'Chi tiêu coin';
      case TransactionType.payment:
        return 'Thanh toán';
      case TransactionType.contribution:
        return 'Đóng góp biển báo';
      case TransactionType.voting:
        return 'Bỏ phiếu';
      case TransactionType.adjustment:
        return 'Điều chỉnh';
      default:
        return 'Giao dịch';
    }
  }

  String _getStatusText(TransactionStatus status) {
    switch (status) {
      case TransactionStatus.pending:
        return 'Đang xử lý';
      case TransactionStatus.completed:
        return 'Hoàn thành';
      case TransactionStatus.failed:
        return 'Thất bại';
      case TransactionStatus.cancelled:
        return 'Đã hủy';
    }
  }

  Color _getStatusColor(BuildContext context, TransactionStatus status) {
    switch (status) {
      case TransactionStatus.pending:
        return Theme.of(context).colorScheme.primary;
      case TransactionStatus.completed:
        return Colors.green;
      case TransactionStatus.failed:
        return Theme.of(context).colorScheme.error;
      case TransactionStatus.cancelled:
        return Theme.of(context).colorScheme.outline;
    }
  }
}

