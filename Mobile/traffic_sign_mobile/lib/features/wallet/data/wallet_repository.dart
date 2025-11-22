import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/api_client.dart';
import 'wallet_balance.dart';
import 'wallet_transaction.dart';

class WalletRepository {
  WalletRepository(this._apiClient);

  final ApiClient _apiClient;

  Future<WalletBalance> getBalance(int userId) async {
    final response = await _apiClient.get<Map<String, dynamic>>(
      '/api/users/$userId/wallet/balance',
    );
    return WalletBalance.fromJson(response.data!);
  }

  /// Get transaction history for a user
  /// Note: This endpoint may need to be implemented in the backend
  /// Expected endpoint: GET /api/wallets/user/{userId}/transactions
  Future<List<WalletTransaction>> getTransactions({
    required int userId,
    int? page,
    int? pageSize,
    String? type,
    String? status,
  }) async {
    try {
      final queryParams = <String, dynamic>{};
      if (page != null) queryParams['page'] = page;
      if (pageSize != null) queryParams['pageSize'] = pageSize;
      if (type != null) queryParams['type'] = type;
      if (status != null) queryParams['status'] = status;

      final response = await _apiClient.get<Map<String, dynamic>>(
        '/api/wallets/user/$userId/transactions',
        queryParameters: queryParams,
      );

      final data = response.data;
      if (data == null) return [];

      // Handle different response formats
      if (data['transactions'] != null) {
        final transactions = data['transactions'] as List;
        return transactions
            .map((json) => WalletTransaction.fromJson(json as Map<String, dynamic>))
            .toList();
      } else if (data is List) {
        return (data as List)
            .map((json) => WalletTransaction.fromJson(json as Map<String, dynamic>))
            .toList();
      }

      return [];
    } catch (e) {
      // If endpoint doesn't exist yet, return empty list
      // This allows the UI to work while backend is being implemented
      return [];
    }
  }
}

final walletRepositoryProvider = Provider<WalletRepository>((ref) {
  final apiClient = ref.watch(apiClientProvider);
  return WalletRepository(apiClient);
});
