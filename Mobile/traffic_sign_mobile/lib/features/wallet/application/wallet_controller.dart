import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../auth/application/auth_controller.dart';
import '../data/wallet_balance.dart';
import '../data/wallet_repository.dart';

class WalletController extends AutoDisposeAsyncNotifier<WalletBalance?> {
  @override
  Future<WalletBalance?> build() async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return null;
    final repository = ref.read(walletRepositoryProvider);
    return repository.getBalance(user.id);
  }

  Future<void> refresh() async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(() async {
      final user = ref.read(authControllerProvider).user;
      if (user == null) return null;
      final repository = ref.read(walletRepositoryProvider);
      return repository.getBalance(user.id);
    });
  }
}

final walletControllerProvider =
    AutoDisposeAsyncNotifierProvider<WalletController, WalletBalance?>(
      WalletController.new,
    );
