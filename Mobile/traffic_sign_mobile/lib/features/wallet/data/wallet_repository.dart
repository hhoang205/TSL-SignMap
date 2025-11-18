import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/api_client.dart';
import 'wallet_balance.dart';

class WalletRepository {
  WalletRepository(this._apiClient);

  final ApiClient _apiClient;

  Future<WalletBalance> getBalance(int userId) async {
    final response = await _apiClient.get<Map<String, dynamic>>(
      '/api/users/$userId/wallet/balance',
    );
    return WalletBalance.fromJson(response.data!);
  }
}

final walletRepositoryProvider = Provider<WalletRepository>((ref) {
  final apiClient = ref.watch(apiClientProvider);
  return WalletRepository(apiClient);
});
