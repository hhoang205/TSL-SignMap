import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../auth/application/auth_controller.dart';
import '../data/wallet_repository.dart';
import '../data/wallet_transaction.dart';

class TransactionController extends AutoDisposeAsyncNotifier<List<WalletTransaction>> {
  @override
  Future<List<WalletTransaction>> build() async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return [];

    final repository = ref.read(walletRepositoryProvider);
    return repository.getTransactions(userId: user.id);
  }

  Future<void> refresh() async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(() async {
      final user = ref.read(authControllerProvider).user;
      if (user == null) return <WalletTransaction>[];
      final repository = ref.read(walletRepositoryProvider);
      return repository.getTransactions(userId: user.id);
    });
  }

  Future<void> loadMore({
    int? page,
    String? type,
    String? status,
  }) async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return;

    final repository = ref.read(walletRepositoryProvider);
    final newTransactions = await repository.getTransactions(
      userId: user.id,
      page: page,
      type: type,
      status: status,
    );

    final currentTransactions = state.value ?? [];
    state = AsyncData([...currentTransactions, ...newTransactions]);
  }
}

final transactionControllerProvider =
    AutoDisposeAsyncNotifierProvider<TransactionController, List<WalletTransaction>>(
      TransactionController.new,
    );

